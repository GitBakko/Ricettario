import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { MOCK_TRENDING_RECIPES, TrendingRecipe } from '../../core/data/mock-data';

@Component({
  selector: 'app-trending-now',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './trending-now.html',
  styleUrl: './trending-now.scss'
})
export class TrendingNowComponent {
  // @MOCK: Trending recipes from mock data
  trendingRecipes: TrendingRecipe[] = MOCK_TRENDING_RECIPES;

  // @MOCK: Navigate to recipe (no real functionality yet)
  onRecipeClick(recipe: TrendingRecipe): void {
    console.log('@MOCK: Recipe clicked', recipe.title);
    // TODO: Navigate to actual recipe when available
  }
}
