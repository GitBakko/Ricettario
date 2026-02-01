import { Component, ChangeDetectionStrategy, input, computed, inject, signal, effect } from '@angular/core';
import { CommonModule, NgClass, DecimalPipe } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { RecipeService } from '../../services/recipe';
import { DialogService } from '../../services/dialog.service';
import { toSignal, toObservable } from '@angular/core/rxjs-interop';
import { catchError, map, of, startWith, switchMap } from 'rxjs';
import { Recipe } from '../../models/recipe';

import { TimeFormatPipe } from '../../pipes/time-format.pipe';

@Component({
  selector: 'app-recipe-detail',
  standalone: true,
  imports: [NgClass, DecimalPipe, RouterLink, FormsModule, TimeFormatPipe],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './recipe-detail.html',
  styleUrl: './recipe-detail.scss'
})
export class RecipeDetailComponent {
  id = input.required<string>();
  private service = inject(RecipeService);
  private sanitizer = inject(DomSanitizer);
  private router = inject(Router);
  private dialogService = inject(DialogService);

  // State signals
  isDeleting = signal(false);

  // Calculator Signals
  desiredPieces = signal<number>(0);
  desiredPieceWeight = signal<number>(0);
  
  numericId = computed(() => {
    const val = parseInt(this.id());
    return isNaN(val) ? null : val;
  });

  recipeResponse = toSignal(
    toObservable(this.numericId).pipe(
      switchMap(id => {
        if (!id) return of({ data: null, error: 'ID non valido', loading: false });
        return this.service.getRecipe(id).pipe(
          map(data => {
              // Initialize calculator with defaults
              this.desiredPieces.set(data.servingPieces || 1);
              this.desiredPieceWeight.set(data.pieceWeight || 0);
              return { data, error: null, loading: false };
          }),
          startWith({ data: null, error: null, loading: true }),
          catchError(err => of({ data: null, error: err.message || 'Errore', loading: false }))
        );
      })
    ),
    { initialValue: { data: null, error: null, loading: true } }
  );

  isLoading = computed(() => this.recipeResponse().loading);
  error = computed(() => this.recipeResponse().error);
  recipe = computed(() => this.recipeResponse().data);

  // Computed Scaled Ingredients (Main List)
  scaledIngredients = computed(() => {
     const r = this.recipe();
     if (!r) return [];
     
     const currentPieces = this.desiredPieces();
     const currentWeight = this.desiredPieceWeight();
     
     const originalTotalWeight = r.ingredients.reduce((acc, i) => acc + i.quantity, 0);
     const targetTotalWeight = currentPieces * currentWeight;

     if (targetTotalWeight <= 0 || originalTotalWeight <= 0) return r.ingredients;
     
     const ratio = targetTotalWeight / originalTotalWeight;

     // Scale each ingredient
     const scaled = r.ingredients.map(ing => ({
         ...ing,
         quantity: ing.quantity * ratio
     }));
     
     // Adjust to match exact target (distribute rounding error to largest ingredient)
     const scaledSum = scaled.reduce((acc, i) => acc + i.quantity, 0);
     const diff = targetTotalWeight - scaledSum;
     
     if (Math.abs(diff) > 0.01 && scaled.length > 0) {
        // Find largest ingredient and adjust it
        let maxIdx = 0;
        let maxQty = 0;
        scaled.forEach((ing, idx) => {
            if (ing.quantity > maxQty) {
                maxQty = ing.quantity;
                maxIdx = idx;
            }
        });
        scaled[maxIdx].quantity += diff;
     }

     return scaled;
  });

  // Scaling Factor
  scalingRatio = computed(() => {
     const r = this.recipe();
     if (!r) return 1;
     const currentPieces = this.desiredPieces();
     const currentWeight = this.desiredPieceWeight();
     const originalTotalWeight = r.ingredients.reduce((acc, i) => acc + i.quantity, 0);
     const targetTotalWeight = currentPieces * currentWeight;
     
     if (targetTotalWeight <= 0 || originalTotalWeight <= 0) return 1;
     return targetTotalWeight / originalTotalWeight;
  });

  // Computed Scaled Phases (Timeline)
  scaledPhases = computed(() => {
      const r = this.recipe();
      const ratio = this.scalingRatio();

      if (!r || !r.phases) return [];
      if (Math.abs(ratio - 1) < 0.0001) return r.phases;

      return r.phases.map((phase: any) => ({
          ...phase,
          phaseIngredients: phase.phaseIngredients ? phase.phaseIngredients.map((pi: any) => ({
              ...pi,
              quantity: pi.quantity * ratio
          })) : []
      }));
  });

  totalTime = computed(() => {
    const r = this.recipe();
    if (!r || !r.phases) return 0;
    return r.phases.reduce((acc: number, p: any) => acc + (p.durationMinutes || 0), 0);
  });

  activePrepTime = computed(() => {
    const r = this.recipe();
    if (!r) return 0;
    
    if (r.phases && r.phases.length > 0 && r.phases.some((p: any) => p.recipePhaseType)) {
        return r.phases.reduce((acc: number, p: any) => {
            return (p.recipePhaseType?.isActiveWork) ? acc + (p.durationMinutes || 0) : acc;
        }, 0);
    }
    return r.prepTimeMinutes || 0;
  });

  downloadPdf() {
    const id = this.numericId();
    if (id) {
       this.service.downloadPdf(id, this.desiredPieces(), this.desiredPieceWeight());
    }
  }

  async deleteRecipe() {
    const r = this.recipe();
    if (!r) return;

    const confirmed = await this.dialogService.confirm(
      `Sei sicuro di voler eliminare la ricetta "${r.title}"? Questa azione non può essere annullata.`,
      'Elimina Ricetta',
      'Sì, elimina',
      'Annulla'
    );

    if (!confirmed) return;

    this.isDeleting.set(true);
    this.service.deleteRecipe(r.id!).subscribe({
      next: () => {
        this.dialogService.success('Ricetta eliminata con successo');
        this.router.navigate(['/']);
      },
      error: (err) => {
        this.isDeleting.set(false);
        if (err.status === 403) {
          this.dialogService.error('Non hai i permessi per eliminare questa ricetta');
        } else {
          this.dialogService.error('Errore durante l\'eliminazione della ricetta');
        }
      }
    });
  }

  getDifficultyInfo(level: string | null | undefined) {
      switch (level) {
          case 'Easy': return { label: 'Facile', icon: 'fa-solid fa-egg', class: 'text-success' };
          case 'Medium': return { label: 'Media', icon: 'fa-solid fa-chart-simple', class: 'text-warning' };
          case 'Hard': return { label: 'Difficile', icon: 'fa-solid fa-bolt', class: 'text-danger' };
          case 'Professional': return { label: 'Pro', icon: 'fa-solid fa-graduation-cap', class: 'text-info' };
          default: return { label: level || 'Media', icon: 'fa-solid fa-chart-simple', class: 'text-warning' };
      }
  }

  getPhaseIcon(phase: any): string {
      return phase?.recipePhaseType?.icon || 'fa-solid fa-circle';
  }

  getPhaseColor(phase: any): string {
      return phase?.recipePhaseType?.color || '#0d6efd';
  }

  resolveIngredientName(name: string): string {
      if (name && name.startsWith('PHASE:')) {
          const idStr = name.split(':')[1];
          const index = parseInt(idStr);
          if (!isNaN(index)) {
              // App uses Index-based references for Phases (PHASE:0, PHASE:1...)
              const phases = this.recipe()?.phases;
              if (phases && phases[index]) {
                  return phases[index].title;
              }
          }
      }
      return name;
  }
  
  isPhaseRef(name: string): boolean {
      return !!name && name.startsWith('PHASE:');
  }

  getSafeVideoUrl(url: string): SafeResourceUrl | null {
      if (!url) return null;
      let embedUrl = '';
      if (url.includes('youtube.com') || url.includes('youtu.be')) {
          const videoId = url.split('v=')[1]?.split('&')[0] || url.split('/').pop();
          if (videoId) embedUrl = `https://www.youtube.com/embed/${videoId}`;
      } else if (url.includes('vimeo.com')) {
          const videoId = url.split('/').pop();
          if (videoId) embedUrl = `https://player.vimeo.com/video/${videoId}`;
      }
      return embedUrl ? this.sanitizer.bypassSecurityTrustResourceUrl(embedUrl) : null;
  }
}
