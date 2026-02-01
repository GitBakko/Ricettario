import { Component, OnInit, signal, inject, computed, effect } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgClass } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { RecipeService, RecipeFilter } from '../../services/recipe';
import { CategoryService } from '../../services/category';
import { Recipe } from '../../models/recipe';
import { Category } from '../../models/category';
import { TimeFormatPipe } from '../../pipes/time-format.pipe';
import { SearchService } from '../../core/services/search.service';
import { 
  MOCK_RECIPE_BADGES, 
  MOCK_USER_LIKES, 
  getRecipeBadge, 
  isRecipeLiked, 
  toggleRecipeLike 
} from '../../core/data/mock-data';

@Component({
  selector: 'app-recipe-list',
  standalone: true,
  imports: [RouterModule, FormsModule, NgClass, TimeFormatPipe, TranslateModule],
  templateUrl: './recipe-list.html',
  styleUrl: './recipe-list.scss'
})
export class RecipeListComponent implements OnInit {
  recipes = signal<Recipe[]>([]);
  categories = signal<Category[]>([]);
  
  // View mode: 'card' or 'list'
  viewMode = signal<'card' | 'list'>('card');
  
  // Local filter state (for categories - search comes from service)
  selectedCategoryId = signal<number | null>(null);

  private recipeService = inject(RecipeService);
  private categoryService = inject(CategoryService);
  private searchService = inject(SearchService);

  // Reactive search from navbar
  searchQuery = this.searchService.searchQuery;

  constructor() {
    // React to search query changes from navbar
    effect(() => {
      const query = this.searchQuery();
      this.loadRecipes();
    });
  }

  ngOnInit(): void {
    this.categoryService.getCategories().subscribe({
      next: (cats) => this.categories.set(cats),
      error: (err) => console.error('Error loading categories', err)
    });
    this.loadRecipes();
  }

  loadRecipes(): void {
    const filter: RecipeFilter = {};
    
    if (this.selectedCategoryId()) {
      filter.categoryId = this.selectedCategoryId();
    }
    if (this.searchQuery().trim()) {
      filter.search = this.searchQuery().trim();
    }

    this.recipeService.getRecipes(filter).subscribe({
        next: (data: Recipe[]) => this.recipes.set(data),
        error: (err) => console.error('Error loading recipes', err)
    });
  }

  onCategoryChange(categoryId: number | null): void {
    this.selectedCategoryId.set(categoryId);
    this.loadRecipes();
  }

  clearFilters(): void {
    this.selectedCategoryId.set(null);
    this.searchService.clearSearchQuery();
    this.loadRecipes();
  }

  toggleViewMode(): void {
    this.viewMode.update(mode => mode === 'card' ? 'list' : 'card');
  }

  setViewMode(mode: 'card' | 'list'): void {
    this.viewMode.set(mode);
  }

  // @MOCK: Get badge type for a recipe
  getRecipeBadge(recipeId: number): 'new' | 'popular' | null {
    return getRecipeBadge(recipeId);
  }

  // @MOCK: Check if recipe is liked
  isLiked(recipeId: number): boolean {
    return isRecipeLiked(recipeId);
  }

  // @MOCK: Toggle like state
  onLikeClick(event: Event, recipeId: number): void {
    event.stopPropagation();
    event.preventDefault();
    toggleRecipeLike(recipeId);
    // Force re-render by updating signal (mock behavior)
    this.recipes.update(r => [...r]);
  }

  getDifficultyInfo(level: string | null | undefined) {
      switch (level) {
          case 'Easy': return { label: 'Facile', icon: 'fa-seedling', class: 'text-success', bg: 'bg-success bg-opacity-10' };
          case 'Medium': return { label: 'Media', icon: 'fa-chart-simple', class: 'text-warning', bg: 'bg-warning bg-opacity-10' };
          case 'Hard': return { label: 'Difficile', icon: 'fa-bolt', class: 'text-danger', bg: 'bg-danger bg-opacity-10' };
          case 'Professional': return { label: 'Pro', icon: 'fa-graduation-cap', class: 'text-dark', bg: 'bg-dark bg-opacity-10' };
          default: return { label: level || 'Media', icon: 'fa-chart-simple', class: 'text-secondary', bg: 'bg-secondary bg-opacity-10' };
      }
  }
}
