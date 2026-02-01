import { Injectable, signal, computed } from '@angular/core';

// -----------------------------------------------------------------------------
// Search Filters Interface
// -----------------------------------------------------------------------------
export interface SearchFilters {
  query: string;                    // Active now - text search
  categoryId?: number;              // @MOCK: Future - filter by category
  tagIds?: number[];                // @MOCK: Future - filter by tags
  maxCookingTime?: number;          // @MOCK: Future - max cooking time in minutes
  difficulty?: 'easy' | 'medium' | 'hard'; // @MOCK: Future - difficulty level
}

// -----------------------------------------------------------------------------
// Search Service
// -----------------------------------------------------------------------------
// Centralized service for search state management
// Used by NavbarComponent (writes) and RecipeListComponent (reads)
// -----------------------------------------------------------------------------
@Injectable({
  providedIn: 'root'
})
export class SearchService {
  // Current search filters using Angular Signals
  private readonly _filters = signal<SearchFilters>({
    query: ''
  });

  // Expose filters as readonly signal
  readonly filters = this._filters.asReadonly();

  // Computed signal for just the query (convenience)
  readonly searchQuery = computed(() => this._filters().query);

  // Computed signal to check if any filters are active
  readonly hasActiveFilters = computed(() => {
    const f = this._filters();
    return !!(
      f.query ||
      f.categoryId ||
      (f.tagIds && f.tagIds.length > 0) ||
      f.maxCookingTime ||
      f.difficulty
    );
  });

  // Filter panel visibility state
  private readonly _showFilters = signal(false);
  readonly showFilters = this._showFilters.asReadonly();

  /**
   * Update the search query
   * @param query - The search text
   */
  setSearchQuery(query: string): void {
    this._filters.update(f => ({ ...f, query }));
  }

  /**
   * Update category filter
   * @MOCK: Prepared for future implementation
   */
  setCategoryFilter(categoryId: number | undefined): void {
    this._filters.update(f => ({ ...f, categoryId }));
  }

  /**
   * Update tags filter
   * @MOCK: Prepared for future implementation
   */
  setTagsFilter(tagIds: number[]): void {
    this._filters.update(f => ({ ...f, tagIds }));
  }

  /**
   * Update cooking time filter
   * @MOCK: Prepared for future implementation
   */
  setCookingTimeFilter(maxMinutes: number | undefined): void {
    this._filters.update(f => ({ ...f, maxCookingTime: maxMinutes }));
  }

  /**
   * Update difficulty filter
   * @MOCK: Prepared for future implementation
   */
  setDifficultyFilter(difficulty: 'easy' | 'medium' | 'hard' | undefined): void {
    this._filters.update(f => ({ ...f, difficulty }));
  }

  /**
   * Update all filters at once
   */
  setFilters(filters: Partial<SearchFilters>): void {
    this._filters.update(f => ({ ...f, ...filters }));
  }

  /**
   * Clear all filters
   */
  clearFilters(): void {
    this._filters.set({ query: '' });
  }

  /**
   * Clear just the search query
   */
  clearSearchQuery(): void {
    this._filters.update(f => ({ ...f, query: '' }));
  }

  /**
   * Toggle filter panel visibility
   */
  toggleFilters(): void {
    this._showFilters.update(v => !v);
  }

  /**
   * Set filter panel visibility
   */
  setShowFilters(show: boolean): void {
    this._showFilters.set(show);
  }
}
