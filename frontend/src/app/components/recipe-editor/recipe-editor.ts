import { Component, OnInit, ChangeDetectionStrategy, signal, inject } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, FormControl, ReactiveFormsModule, FormsModule, Validators, AbstractControl } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { DecimalPipe, NgClass } from '@angular/common'; 
import { NgSelectModule } from '@ng-select/ng-select'; 
import { RecipeService } from '../../services/recipe';
import { DialogService } from '../../services/dialog.service';
import { PhaseTypeService } from '../../services/phase-type.service';
import { CategoryService } from '../../services/category';
import { PhaseType } from '../../models/phase-type';
import { Category } from '../../models/category';
import { TagInputComponent } from '../tag-input/tag-input';

@Component({
  selector: 'app-recipe-editor',
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, RouterModule, NgSelectModule, DecimalPipe, NgClass, TagInputComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './recipe-editor.html',
  styleUrl: './recipe-editor.scss'
})
export class RecipeEditorComponent implements OnInit {
  recipeForm: FormGroup;
  activeTab = signal('general');
  editMode = signal(false);
  recipeId = signal<number | null>(null);
  currentImage = signal<string | null>(null);

  private fb = inject(FormBuilder);
  private recipeService = inject(RecipeService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private sanitizer = inject(DomSanitizer);
  private dialog = inject(DialogService);
  private phaseTypeService = inject(PhaseTypeService);
  private categoryService = inject(CategoryService);

  phaseTypesList = signal<PhaseType[]>([]);
  categoriesList = signal<Category[]>([]);

  constructor() {
    this.recipeForm = this.fb.group({
      title: ['', Validators.required],
      description: [''],
      difficulty: ['Medium'],
      imageUrl: [''],
      pieceWeight: [250],
      totalFlourWeight: [1000],
      categoryId: [null],
      tagIds: [[]],
      ingredients: this.fb.array([]),
      phases: this.fb.array([]),
      videos: this.fb.array([])
    });
  }

  ngOnInit() {
    this.phaseTypeService.getPhaseTypes().subscribe(types => this.phaseTypesList.set(types));
    this.categoryService.getCategories().subscribe(cats => this.categoriesList.set(cats));

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
        this.editMode.set(true);
        this.recipeId.set(+id);
        this.loadRecipe(+id);
    } else {
        // Init default only if new
        this.addIngredient('Farina', 1000, 100); 
        this.addIngredient('Acqua', 700, 70);
        this.addIngredient('Sale', 20, 2);
        this.addIngredient('Lievito', 5, 0.5);

        // IDs: 1=Impasto, 2=Lievitazione (Default seeds)
        this.addPhase(1, 'Autolisi');
        this.addPhase(2, 'Puntata');
    }
  }

  loadRecipe(id: number) {
      this.recipeService.getRecipe(id).subscribe(r => {
          this.recipeForm.patchValue({
              title: r.title,
              description: r.description,
              difficulty: r.difficulty || 'Medium',
              imageUrl: r.imageUrl,
              pieceWeight: r.pieceWeight || 250,
              totalFlourWeight: r.totalFlourWeight || 1000,
              categoryId: r.categoryId || null,
              tagIds: r.tagIds || []
          });
          
          if (r.imageUrl) this.currentImage.set(r.imageUrl);

          // Ingredients
          const ingFormArray = this.recipeForm.get('ingredients') as FormArray;
          ingFormArray.clear();
          r.ingredients.forEach(i => {
              ingFormArray.push(this.fb.group({
                  name: [i.name, Validators.required],
                  quantity: [i.quantity],
                  bakersPercentage: [0], // Will be calculated
                  unit: [i.unit]
              }));
          });
          // Recalculate percentages
          const totalFlour = r.totalFlourWeight || 1000;
          ingFormArray.controls.forEach(ctrl => {
              const qty = ctrl.get('quantity')?.value || 0;
              const pct = (qty / totalFlour) * 100;
              ctrl.patchValue({ bakersPercentage: parseFloat(pct.toFixed(2)) }, { emitEvent: false });
          });


          // Videos
          const vidFormArray = this.recipeForm.get('videos') as FormArray;
          vidFormArray.clear();
          if (r.videos) {
              r.videos.forEach(v => {
                  vidFormArray.push(this.fb.group({ url: [v.url, Validators.required] }));
              });
          }

          // Phases
          const phFormArray = this.recipeForm.get('phases') as FormArray;
          phFormArray.clear();
          if (r.phases) {
              r.phases.forEach(p => {
                  const pIngs = new FormArray<FormGroup>([]);
                  if (p.phaseIngredients) {
                      p.phaseIngredients.forEach(pi => {
                          pIngs.push(this.fb.group({
                              ingredientName: [pi.ingredientName, Validators.required],
                              quantity: [pi.quantity],
                              unit: [pi.unit]
                          }));
                      });
                  }

                  phFormArray.push(this.fb.group({
                      phaseType: [p.recipePhaseTypeId, Validators.required],
                      title: [p.title, Validators.required],
                      description: [p.description],
                      durationMinutes: [p.durationMinutes],
                      temperature: [p.temperature],
                      phaseIngredients: pIngs
                  }));
              });
          }
      });
  }
  
  onFileSelected(event: any) {
    const file: File = event.target.files[0];
    if (file) {
        this.recipeService.uploadImage(file).subscribe({
            next: (res) => {
                this.recipeForm.patchValue({ imageUrl: res.url });
                this.currentImage.set(res.url);
            },
            error: (err) => console.error('Upload failed', err)
        });
    }
  }

  removeImage() {
      this.recipeForm.patchValue({ imageUrl: null });
      this.currentImage.set(null);
  }

  onSubmit() {
    this.recipeForm.markAllAsTouched();
    if (this.recipeForm.valid) {
      const allowedTolerance = 0.002; // 0.2%
      
      // Validation: Check if all ingredients are used correctly
      const validationErrors: string[] = [];
      const usageMap = new Map<string, number>();

      // Sum usage from phases
      this.phases.controls.forEach((p) => {
          const pIngs = p.get('phaseIngredients') as FormArray;
          pIngs.controls.forEach((pi) => {
             const name = pi.get('ingredientName')?.value;
             const qty = pi.get('quantity')?.value || 0;
             if (name && !name.startsWith('PHASE:')) {
                 usageMap.set(name, (usageMap.get(name) || 0) + qty);
             }
          });
      });
      
      // Compare with main ingredients
      this.ingredients.controls.forEach(c => {
          const name = c.get('name')?.value;
          const totalQty = c.get('quantity')?.value || 0;
          const usedQty = usageMap.get(name) || 0;
          
          if (totalQty > 0) {
              const diff = Math.abs(totalQty - usedQty);
              const toleranceQty = totalQty * allowedTolerance;
              
              if (diff > toleranceQty) {
                  validationErrors.push(`L'ingrediente '${name}' non è correttamente bilanciato. Totale: ${totalQty}, Usato: ${usedQty}. (Diff: ${diff.toFixed(2)})`);
              }
          }
      });

      if (validationErrors.length > 0) {
          const htmlMessage = '<ul class="text-start">' + validationErrors.map(e => `<li>${e}</li>`).join('') + '</ul>';
          this.dialog.error(htmlMessage, 'Validation Failed');
          return;
      }
      
      const formValue = this.recipeForm.value;
      
      const typesMap = new Map(this.phaseTypesList().map(t => [t.id, t]));
      
      let calcActivePrep = 0;
      let calcCookTime = 0;

      formValue.phases.forEach((p: any) => {
         const duration = p.durationMinutes || 0;
         const typeDef = typesMap.get(p.phaseType);
         if (typeDef) {
             if (typeDef.name === 'Cottura') {
                 calcCookTime += duration;
             } else if (typeDef.isActiveWork) {
                 calcActivePrep += duration;
             }
         }
      });
      
      const recipe: any = {
        title: formValue.title,
        description: formValue.description,
        prepTimeMinutes: calcActivePrep, 
        cookTimeMinutes: calcCookTime, 
        difficulty: formValue.difficulty,
        imageUrl: formValue.imageUrl,
        servingPieces: this.calculateProjectedPieces(), 
        pieceWeight: formValue.pieceWeight,
        totalFlourWeight: formValue.totalFlourWeight,
        categoryId: formValue.categoryId || null,
        tagIds: formValue.tagIds || [],
        ingredients: formValue.ingredients,
        phases: formValue.phases.map((p:any) => ({
             ...p,
             recipePhaseTypeId: p.phaseType // Map form control 'phaseType' to DTO 'recipePhaseTypeId'
        })),
        videoUrls: formValue.videos.map((v:any) => v.url)
      };

      // --- Call Service ---
      if (this.editMode() && this.recipeId()) {
          this.recipeService.updateRecipe(this.recipeId()!, recipe).subscribe({
              next: () => {
                  this.dialog.success('Ricetta aggiornata con successo!');
                  this.router.navigate(['/recipes', this.recipeId()]);
              },
              error: (err) => this.dialog.error('Errore durante l\'aggiornamento della ricetta.', 'Errore')
          });
      } else {
          this.recipeService.createRecipe(recipe).subscribe({
              next: () => {
                   this.dialog.success('Ricetta creata con successo!');
                   this.router.navigate(['/']);
              },
              error: (err) => this.dialog.error('Errore durante il salvataggio della ricetta.', 'Errore')
          });
      }
    }
  }

  cancel() {
      if (this.editMode() && this.recipeId()) {
          this.router.navigate(['/recipes', this.recipeId()]);
      } else {
          this.router.navigate(['/']);
      }
  }

  get ingredients() { return this.recipeForm.get('ingredients') as FormArray; }
  get phases() { return this.recipeForm.get('phases') as FormArray; }
  get videos() { return this.recipeForm.get('videos') as FormArray; }
  get totalFlourControl() { return this.recipeForm.get('totalFlourWeight') as FormControl; }

  getPhaseIngredients(phaseIndex: number) {
    return this.phases.at(phaseIndex).get('phaseIngredients') as FormArray;
  }

  addIngredient(name: string = '', quantity: number = 0, percentage: number = 0) {
    const isBase = this.ingredients.length === 0;
    this.ingredients.push(this.fb.group({
      name: [name, Validators.required],
      quantity: [quantity],
      bakersPercentage: [isBase ? 100 : percentage],
      unit: ['g']
    }));
  }

  removeIngredient(index: number) {
    if (index === 0) return;
    this.ingredients.removeAt(index);
  }

  addPhase(typeId: number = 1, title: string = '') {
      const phaseGroup = this.fb.group({
          phaseType: [typeId, Validators.required],
          title: [title, Validators.required],
          description: [''],
          durationMinutes: [0],
          temperature: [null],
          phaseIngredients: this.fb.array([])
      });
      this.phases.push(phaseGroup);
  }

  removePhase(index: number) {
      this.phases.removeAt(index);
  }

  movePhaseUp(index: number) {
      if (index <= 0) return;
      const phase = this.phases.at(index);
      this.phases.removeAt(index);
      this.phases.insert(index - 1, phase);
      
      this.recipeForm.updateValueAndValidity(); // Force update
  }

  movePhaseDown(index: number) {
      if (index >= this.phases.length - 1) return;
      const phase = this.phases.at(index);
      this.phases.removeAt(index);
      this.phases.insert(index + 1, phase);

      this.recipeForm.updateValueAndValidity(); // Force update
  }

  addPhaseIngredient(phaseIndex: number) {
      this.getPhaseIngredients(phaseIndex).push(this.fb.group({
          ingredientName: ['', Validators.required],
          quantity: [0],
          unit: ['g']
      }));
  }

  addAllIngredientsToPhase(phaseIndex: number) {
      const phaseIngredients = this.getPhaseIngredients(phaseIndex);
      
      // Clear existing phase ingredients
      while (phaseIngredients.length > 0) {
          phaseIngredients.removeAt(0);
      }
      
      // Add all recipe ingredients with their total quantity
      this.ingredients.controls.forEach((ingredient) => {
          const name = ingredient.get('name')?.value;
          const quantity = ingredient.get('quantity')?.value || 0;
          const unit = ingredient.get('unit')?.value || 'g';
          
          if (name) {
              phaseIngredients.push(this.fb.group({
                  ingredientName: [name, Validators.required],
                  quantity: [quantity],
                  unit: [unit]
              }));
          }
      });
  }

  removePhaseIngredient(phaseIndex: number, piIndex: number) {
      this.getPhaseIngredients(phaseIndex).removeAt(piIndex);
  }

  addVideo() {
      this.videos.push(this.fb.group({
          url: ['', Validators.required]
      }));
  }

  removeVideo(index: number) {
      this.videos.removeAt(index);
  }
  
  recalculateIngredients() {
      const totalFlour = this.totalFlourControl?.value || 0;
      this.ingredients.at(0).patchValue({ quantity: totalFlour }, { emitEvent: false });

      for (let i = 1; i < this.ingredients.length; i++) {
          const row = this.ingredients.at(i);
          const pct = row.get('bakersPercentage')?.value || 0;
          const newQty = (totalFlour * pct) / 100;
          row.patchValue({ quantity: parseFloat(newQty.toFixed(1)) }, { emitEvent: false });
      }
  }

  updateQuantityFromPercentage(index: number) {
      if (index === 0) return; 
      const totalFlour = this.ingredients.at(0).get('quantity')?.value || 0;
      const pct = this.ingredients.at(index).get('bakersPercentage')?.value || 0;
      const newQty = (totalFlour * pct) / 100;
      this.ingredients.at(index).patchValue({ quantity: parseFloat(newQty.toFixed(1)) }, { emitEvent: false });
  }

  updatePercentageFromQuantity(index: number) {
       const totalFlour = this.ingredients.at(0).get('quantity')?.value || 0;
       const qty = this.ingredients.at(index).get('quantity')?.value || 0;
       
       if (totalFlour > 0) {
           const newPct = (qty / totalFlour) * 100;
           this.ingredients.at(index).patchValue({ bakersPercentage: parseFloat(newPct.toFixed(2)) }, { emitEvent: false });
       }
  }

  getCurrentTotalWeight(): number {
      return this.ingredients.controls.reduce((acc, ctrl) => acc + (ctrl.get('quantity')?.value || 0), 0);
  }

  calculateProjectedPieces(): number {
      const totalWeight = this.getCurrentTotalWeight();
      const pieceWeight = this.recipeForm.get('pieceWeight')?.value || 0;
      if (pieceWeight <= 0) return 0;
      return Math.floor(totalWeight / pieceWeight);
  }

  calculateWaste(): number {
      const totalWeight = this.getCurrentTotalWeight();
      const pieceWeight = this.recipeForm.get('pieceWeight')?.value || 0;
      if (pieceWeight <= 0) return 0;
      
      const pieces = Math.floor(totalWeight / pieceWeight);
      if (pieces === 0) return 0;

      return totalWeight - (pieces * pieceWeight);
  }

  calculateIdealPieceWeight(): number {
      const totalWeight = this.getCurrentTotalWeight();
      const pieces = this.calculateProjectedPieces();
      if (pieces === 0) return 0;
      return totalWeight / pieces;
  }

  getSafeIdealPieceWeight(): number {
      const ideal = this.calculateIdealPieceWeight();
      if (ideal <= 0) return 0;
      return Math.floor(ideal * 100) / 100;
  }
  
  applyIdealPieceWeight() {
      const safeWeight = this.getSafeIdealPieceWeight();
      if (safeWeight > 0) {
          this.recipeForm.patchValue({ pieceWeight: safeWeight });
      }
  }

  getFormattedTotalTime(): string {
      let totalMin = 0;
      this.phases.controls.forEach(p => totalMin += (p.get('durationMinutes')?.value || 0));
      const h = Math.floor(totalMin / 60);
      const m = Math.floor(totalMin % 60);
      if (h > 0) return `${h}h ${m}m`;
      return `${m} min`;
  }

  getPhaseOptions(currentPhaseIndex: number) {
      const usageMap = new Map<string, number>();
      this.phases.controls.forEach((p) => {
          const pIngs = p.get('phaseIngredients') as FormArray;
          pIngs.controls.forEach((pi) => {
             const name = pi.get('ingredientName')?.value;
             const qty = pi.get('quantity')?.value || 0;
             if (name && !name.startsWith('PHASE:')) {
                 usageMap.set(name, (usageMap.get(name) || 0) + qty);
             }
          });
      });

      const options: any[] = [];
      this.ingredients.controls.forEach(c => {
          const name = c.get('name')?.value;
          if (name) {
             const total = c.get('quantity')?.value || 0;
             const used = usageMap.get(name) || 0;
             const remaining = Math.max(0, total - used); 
             
             options.push({
                 label: name, 
                 value: name,
                 group: 'Ingredienti Principali',
                 type: 'INGREDIENT',
                 total: total,
                 remaining: remaining
             });
          }
      });
      
      this.phases.controls.forEach((c, i) => {
          if (i < currentPhaseIndex) {
              const weight = this.calculatePhaseTotalWeight(i);
              options.push({
                  label: `Fase ${i + 1}: ${c.get('title')?.value || 'Senza Titolo'}`,
                  value: `PHASE:${i}`,
                  group: 'Fasi Precedenti',
                  type: 'PHASE',
                  weight: weight
              });
          }
      });

      return options;
  }

  setPhaseIngredientQuantity(pIndex: number, piIndex: number, amount: number) {
      const group = this.getPhaseIngredients(pIndex).at(piIndex);
      group.patchValue({ quantity: amount });
      group.markAsDirty();
  }

  calculatePhaseTotalWeight(phaseIndex: number): number {
      const p = this.phases.at(phaseIndex);
      if (!p) return 0;
      const ingredients = p.get('phaseIngredients') as FormArray;
      
      let total = 0;
      ingredients.controls.forEach(ctrl => {
          total += (ctrl.get('quantity')?.value || 0);
      });
      return total;
  }

  onPhaseIngredientChange(phaseIndex: number, piIndex: number) {
      const group = this.getPhaseIngredients(phaseIndex).at(piIndex);
      const selectedValue = group.get('ingredientName')?.value;

      if (selectedValue && selectedValue.startsWith('PHASE:')) {
          const refIndex = parseInt(selectedValue.split(':')[1]);
          const weight = this.calculatePhaseTotalWeight(refIndex);
          
          group.patchValue({ quantity: weight });
          group.get('quantity')?.disable();
      } else {
          group.get('quantity')?.enable();
      }
  }

  getIngredientStatus(ingredientName: string, currentPhaseIndex: number, currentPiIndex: number) {
      if (!ingredientName || ingredientName.startsWith('PHASE:')) return null;

      const mainIng = this.ingredients.controls.find(c => c.get('name')?.value === ingredientName);
      if (!mainIng) return null;
      
      const totalAvailable = mainIng.get('quantity')?.value || 0;
      let used = 0;
      this.phases.controls.forEach((phase, pIdx) => {
          const pIngs = phase.get('phaseIngredients') as FormArray;
          pIngs.controls.forEach((pi, piIdx) => {
              // Skip current input being edited
              if (pIdx === currentPhaseIndex && piIdx === currentPiIndex) return;
              
              if (pi.get('ingredientName')?.value === ingredientName) {
                  used += (pi.get('quantity')?.value || 0);
              }
          });
      });

      // Get current input value (it might be invalid if empty, assume 0)
      const currentControl = this.getPhaseIngredients(currentPhaseIndex).at(currentPiIndex).get('quantity');
      const currentInputVal = currentControl?.value || 0;
      
      const remaining = totalAvailable - used; 
      // Check if new total exceeds available (Floating point tolerance)
      const isOver = (used + currentInputVal) > (totalAvailable + 0.01);

      // Force invalid error on the control if over
      if (isOver && currentControl?.valid) {
          currentControl.setErrors({ 'overLimit': true });
      } else if (!isOver && currentControl?.hasError('overLimit')) {
          currentControl.setErrors(null);
      }

      return { total: totalAvailable, used, remaining, currentInputVal, isOver };
  }

  getPhaseIcon(typeId: number): string {
      const type = this.phaseTypesList().find(t => t.id === typeId);
      const name = type ? type.name : '';
      
      switch(name) {
          case 'Pre-Impasto': return 'fa-solid fa-basket-shopping';
          case 'Impasto': return 'fa-solid fa-compact-disc'; 
          case 'Riposo': return 'fa-solid fa-clock-rotate-left';
          case 'Lievitazione': return 'fa-solid fa-hourglass-half';
          case 'Staglio': return 'fa-solid fa-border-all';
          case 'Fermentazione': return 'fa-solid fa-virus';
          case 'Cottura': return 'fa-solid fa-fire';
          case 'Pieghe': return 'fa-solid fa-layer-group';
          case 'Formatura': return 'fa-solid fa-box';
          case 'Appretto': return 'fa-solid fa-rotate-right';
          case 'Autoli': return 'fa-solid fa-droplet';
          default: return 'fa-solid fa-circle';
      }
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

  hasEmbed(url: string): boolean {
      return !!this.getSafeVideoUrl(url);
  }
  
  trackByPhaseOption(item: any) {
      return item.value;
  }
}
