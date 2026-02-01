import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { CategoryService } from '../../services/category';
import { Category, CategoryCreate, CategoryUpdate } from '../../models/category';
import { IconPickerComponent, IconOption } from '../../shared/components/icon-picker/icon-picker';

@Component({
  selector: 'app-category-manager',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, IconPickerComponent],
  templateUrl: './category-manager.html',
  styleUrl: './category-manager.scss'
})
export class CategoryManagerComponent implements OnInit {
  private categoryService = inject(CategoryService);

  categories = signal<Category[]>([]);
  loading = signal(false);
  editingId = signal<number | null>(null);

  formModel: CategoryCreate = {
    name: '',
    description: '',
    icon: 'fa-solid fa-folder',
    color: '#6c757d'
  };

  availableIcons: IconOption[] = [
    { value: 'fa-solid fa-pizza-slice', label: '🍕 Pizza' },
    { value: 'fa-solid fa-bread-slice', label: '🍞 Pane' },
    { value: 'fa-solid fa-mug-hot', label: '☕ Caffetteria' },
    { value: 'fa-solid fa-cake-candles', label: '🎂 Pasticceria' },
    { value: 'fa-solid fa-egg', label: '🥚 Uova' },
    { value: 'fa-solid fa-box', label: '📦 Lievitati' },
    { value: 'fa-solid fa-chart-pie', label: '🥧 Torte' },
    { value: 'fa-solid fa-cookie', label: '🍪 Biscotti' },
    { value: 'fa-solid fa-file', label: '📄 Pasta' },
    { value: 'fa-solid fa-croissant', label: '🥐 Viennoiserie' },
    { value: 'fa-solid fa-table-cells', label: '▦ Focaccia' },
    { value: 'fa-solid fa-star', label: '⭐ Speciale' },
    { value: 'fa-solid fa-folder', label: '📁 Altro' },
  ];

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.loading.set(true);
    this.categoryService.getCategories().subscribe({
      next: (data) => {
        this.categories.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading categories:', err);
        this.loading.set(false);
      }
    });
  }

  edit(category: Category): void {
    this.editingId.set(category.id);
    this.formModel = {
      name: category.name,
      description: category.description,
      icon: category.icon || 'fa-solid fa-folder',
      color: category.color || '#6c757d',
      sortOrder: category.sortOrder
    };
  }

  cancel(): void {
    this.editingId.set(null);
    this.resetForm();
  }

  resetForm(): void {
    this.formModel = {
      name: '',
      description: '',
      icon: 'fa-solid fa-folder',
      color: '#6c757d'
    };
  }

  save(): void {
    if (!this.formModel.name.trim()) return;

    if (this.editingId()) {
      const updateDto: CategoryUpdate = {
        name: this.formModel.name,
        description: this.formModel.description,
        icon: this.formModel.icon,
        color: this.formModel.color,
        sortOrder: this.formModel.sortOrder
      };

      this.categoryService.updateCategory(this.editingId()!, updateDto).subscribe({
        next: () => {
          this.loadCategories();
          this.cancel();
        },
        error: (err) => console.error('Error updating category:', err)
      });
    } else {
      this.categoryService.createCategory(this.formModel).subscribe({
        next: () => {
          this.loadCategories();
          this.resetForm();
        },
        error: (err) => console.error('Error creating category:', err)
      });
    }
  }

  delete(category: Category): void {
    if (!confirm(`Sei sicuro di voler eliminare la categoria "${category.name}"?`)) return;

    this.categoryService.deleteCategory(category.id).subscribe({
      next: () => this.loadCategories(),
      error: (err) => console.error('Error deleting category:', err)
    });
  }

  moveUp(category: Category): void {
    const cats = this.categories();
    const index = cats.findIndex(c => c.id === category.id);
    if (index > 0) {
      this.swapAndReorder(index, index - 1);
    }
  }

  moveDown(category: Category): void {
    const cats = this.categories();
    const index = cats.findIndex(c => c.id === category.id);
    if (index < cats.length - 1) {
      this.swapAndReorder(index, index + 1);
    }
  }

  private swapAndReorder(index1: number, index2: number): void {
    const cats = [...this.categories()];
    [cats[index1], cats[index2]] = [cats[index2], cats[index1]];
    
    const reorders = cats.map((c, i) => ({ id: c.id, sortOrder: i }));
    
    this.categoryService.reorderCategories(reorders).subscribe({
      next: () => this.loadCategories(),
      error: (err) => console.error('Error reordering categories:', err)
    });
  }
}
