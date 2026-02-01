import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NgSelectModule } from '@ng-select/ng-select';
import { IngredientArchiveService } from '../../services/ingredient-archive.service';
import { IconPickerComponent, IconOption } from '../../shared/components/icon-picker/icon-picker';
import {
  IngredientArchive,
  IngredientArchiveCreate,
  IngredientConversion,
  IngredientConversionCreate,
  IngredientCategory,
  IngredientCategories,
  IngredientCategoryLabels,
  IngredientCategoryIcons,
  IngredientCustomProperty
} from '../../models/ingredient-archive.model';

@Component({
  selector: 'app-ingredient-archive-manager',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, NgSelectModule, IconPickerComponent],
  templateUrl: './ingredient-archive-manager.html',
  styleUrl: './ingredient-archive-manager.scss'
})
export class IngredientArchiveManagerComponent implements OnInit {
  private service = inject(IngredientArchiveService);

  // State signals
  ingredients = signal<IngredientArchive[]>([]);
  loading = signal(false);
  saving = signal(false);
  editingId = signal<number | null>(null);
  selectedIngredient = signal<IngredientArchive | null>(null);
  conversions = signal<IngredientConversion[]>([]);
  loadingConversions = signal(false);
  filterCategory = signal<IngredientCategory | null>(null);
  searchQuery = signal('');

  // Form model
  formModel: IngredientArchiveCreate = this.getEmptyForm();

  // Conversion form
  conversionForm: IngredientConversionCreate = {
    toIngredientId: 0,
    conversionRatio: 1,
    notes: ''
  };
  showConversionForm = signal(false);

  // Custom properties for form
  customProperties: IngredientCustomProperty[] = [];

  // Available icons for picker
  availableIcons: IconOption[] = [
    { value: 'fa-solid fa-wheat-awn', label: '🌾 Farina' },
    { value: 'fa-solid fa-cubes', label: '🧊 Lievito' },
    { value: 'fa-solid fa-cubes-stacked', label: '📦 Lievito secco' },
    { value: 'fa-solid fa-bread-slice', label: '🍞 Lievito madre' },
    { value: 'fa-solid fa-droplet', label: '💧 Liquido' },
    { value: 'fa-solid fa-oil-can', label: '🛢️ Olio/Grasso' },
    { value: 'fa-solid fa-jar', label: '🫙 Burro/Strutto' },
    { value: 'fa-solid fa-candy-cane', label: '🍬 Zucchero' },
    { value: 'fa-solid fa-mortar-pestle', label: '🧂 Sale' },
    { value: 'fa-solid fa-egg', label: '🥚 Uova' },
    { value: 'fa-solid fa-cheese', label: '🧀 Latticini' },
    { value: 'fa-solid fa-seedling', label: '🌱 Semi' },
    { value: 'fa-solid fa-leaf', label: '🌿 Erbe' },
    { value: 'fa-solid fa-lemon', label: '🍋 Agrumi' },
    { value: 'fa-solid fa-bowl-food', label: '🍲 Altro' },
  ];

  // Categories for dropdown (using string-based categories)
  categories = IngredientCategories.map(cat => ({
    value: cat,
    label: IngredientCategoryLabels[cat],
    icon: IngredientCategoryIcons[cat]
  }));

  // Computed: filtered ingredients
  filteredIngredients = computed(() => {
    let list = this.ingredients();
    const cat = this.filterCategory();
    const search = this.searchQuery().toLowerCase();

    if (cat !== null) {
      list = list.filter(i => i.category === cat);
    }
    if (search) {
      list = list.filter(i => i.name.toLowerCase().includes(search));
    }
    return list;
  });

  // Computed: grouped by category
  groupedIngredients = computed(() => {
    const filtered = this.filteredIngredients();
    const groups: { category: IngredientCategory; label: string; icon: string; items: IngredientArchive[] }[] = [];
    
    const categoryOrder: IngredientCategory[] = [
      'Farina',
      'Lievito',
      'Liquido',
      'Grasso',
      'Dolcificante',
      'Sale',
      'Uova',
      'Latticini',
      'Altro'
    ];

    for (const cat of categoryOrder) {
      const items = filtered.filter(i => i.category === cat);
      if (items.length > 0) {
        groups.push({
          category: cat,
          label: IngredientCategoryLabels[cat],
          icon: IngredientCategoryIcons[cat],
          items
        });
      }
    }
    return groups;
  });

  // Computed: available ingredients for conversion (exclude current)
  availableForConversion = computed(() => {
    const selected = this.selectedIngredient();
    if (!selected) return [];
    
    const existingIds = new Set(this.conversions().map(c => c.toIngredientId));
    return this.ingredients().filter(i => i.id !== selected.id && !existingIds.has(i.id));
  });

  ngOnInit() {
    this.loadIngredients();
  }

  loadIngredients() {
    this.loading.set(true);
    this.service.getIngredients().subscribe({
      next: (data) => {
        this.ingredients.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading ingredients:', err);
        this.loading.set(false);
      }
    });
  }

  selectIngredient(ingredient: IngredientArchive) {
    this.selectedIngredient.set(ingredient);
    this.loadConversions(ingredient.id);
  }

  loadConversions(id: number) {
    this.loadingConversions.set(true);
    this.service.getConversions(id).subscribe({
      next: (data) => {
        this.conversions.set(data);
        this.loadingConversions.set(false);
      },
      error: (err) => {
        console.error('Error loading conversions:', err);
        this.loadingConversions.set(false);
      }
    });
  }

  edit(ingredient: IngredientArchive) {
    this.editingId.set(ingredient.id);
    this.formModel = {
      name: ingredient.name,
      description: ingredient.description,
      defaultUnit: ingredient.defaultUnit,
      category: ingredient.category,
      icon: ingredient.icon,
      color: ingredient.color,
      sortOrder: ingredient.sortOrder
    };
    this.customProperties = [...(ingredient.customProperties || [])];
  }

  cancel() {
    this.editingId.set(null);
    this.resetForm();
  }

  resetForm() {
    this.formModel = this.getEmptyForm();
    this.customProperties = [];
  }

  getEmptyForm(): IngredientArchiveCreate {
    return {
      name: '',
      description: '',
      defaultUnit: 'g',
      category: 'Altro',
      icon: 'fa-solid fa-bowl-food',
      color: '#6c757d',
      sortOrder: 0
    };
  }

  save() {
    if (!this.formModel.name.trim()) return;

    this.saving.set(true);
    
    const data = {
      ...this.formModel,
      customProperties: this.customProperties.filter(p => p.label.trim())
    };

    const editId = this.editingId();
    if (editId) {
      this.service.updateIngredient(editId, data).subscribe({
        next: () => {
          this.loadIngredients();
          this.cancel();
          this.saving.set(false);
          // Refresh selected if editing current
          if (this.selectedIngredient()?.id === editId) {
            this.service.getIngredient(editId).subscribe(i => this.selectedIngredient.set(i));
          }
        },
        error: (err) => {
          console.error('Error updating ingredient:', err);
          this.saving.set(false);
        }
      });
    } else {
      this.service.createIngredient(data).subscribe({
        next: (created) => {
          this.loadIngredients();
          this.resetForm();
          this.saving.set(false);
          this.selectIngredient(created);
        },
        error: (err) => {
          console.error('Error creating ingredient:', err);
          this.saving.set(false);
        }
      });
    }
  }

  delete(ingredient: IngredientArchive) {
    if (ingredient.isSystemDefault) {
      alert('Gli ingredienti di sistema non possono essere eliminati');
      return;
    }
    if (!confirm(`Eliminare "${ingredient.name}"?`)) return;

    this.service.deleteIngredient(ingredient.id).subscribe({
      next: () => {
        this.loadIngredients();
        if (this.selectedIngredient()?.id === ingredient.id) {
          this.selectedIngredient.set(null);
          this.conversions.set([]);
        }
      },
      error: (err) => {
        console.error('Error deleting ingredient:', err);
        alert(err.error?.message || 'Errore durante l\'eliminazione');
      }
    });
  }

  // Custom Properties management
  addCustomProperty() {
    this.customProperties.push({ label: '', value: '' });
  }

  removeCustomProperty(index: number) {
    this.customProperties.splice(index, 1);
  }

  // Conversion management
  openConversionForm() {
    this.conversionForm = { toIngredientId: 0, conversionRatio: 1, notes: '' };
    this.showConversionForm.set(true);
  }

  cancelConversionForm() {
    this.showConversionForm.set(false);
  }

  saveConversion() {
    const selected = this.selectedIngredient();
    if (!selected || !this.conversionForm.toIngredientId) return;

    this.service.addConversion(selected.id, this.conversionForm).subscribe({
      next: () => {
        this.showConversionForm.set(false);
        this.loadConversions(selected.id);
      },
      error: (err) => {
        console.error('Error adding conversion:', err);
        alert(err.error?.message || 'Errore durante il salvataggio');
      }
    });
  }

  deleteConversion(conversion: IngredientConversion) {
    if (conversion.isReverse) {
      alert('Questa è una conversione inversa. Elimina la conversione originale.');
      return;
    }
    if (!confirm('Eliminare questa conversione?')) return;

    this.service.deleteConversion(conversion.id).subscribe({
      next: () => {
        const selected = this.selectedIngredient();
        if (selected) this.loadConversions(selected.id);
      },
      error: (err) => console.error('Error deleting conversion:', err)
    });
  }

  // Helpers
  getCategoryLabel(cat: IngredientCategory): string {
    return IngredientCategoryLabels[cat] || 'Altro';
  }

  getCategoryIcon(cat: IngredientCategory): string {
    return IngredientCategoryIcons[cat] || 'fa-solid fa-ellipsis';
  }

  formatRatio(ratio: number): string {
    if (ratio === 1) return '1:1';
    if (ratio < 1) return `1:${(1/ratio).toFixed(2)}`;
    return `${ratio.toFixed(2)}:1`;
  }

  trackById(index: number, item: { id: number }) {
    return item.id;
  }
}
