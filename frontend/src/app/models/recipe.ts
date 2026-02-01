import { Category } from './category';
import { Tag } from './tag';

export interface Ingredient {
  id?: number;
  name: string;
  quantity: number;
  unit: string;
  bakersPercentage?: number;
}

export interface PhaseIngredient {
    id?: number;
    ingredientName: string;
    quantity: number;
    unit: string;
}

export interface RecipePhase {
    id?: number;
    title: string;
    recipePhaseTypeId: number;
    recipePhaseType?: { 
        id: number; 
        name: string; 
        description?: string;
        icon?: string;
        color?: string;
        isActiveWork: boolean; 
    };
    description?: string;
    durationMinutes: number;
    temperature?: number;
    ovenMode?: string;
    phaseIngredients: PhaseIngredient[];
}

export interface RecipeVideo {
    id?: number;
    url: string;
}

export interface Recipe {
  id?: number;
  title: string;
  description?: string;
  instructions?: string;
  prepTimeMinutes: number;
  cookTimeMinutes: number;
  difficulty?: string;
  imageUrl?: string;
  totalFlourWeight: number;
  servingPieces: number;
  pieceWeight?: number;
  ingredients: Ingredient[];
  
  phases?: RecipePhase[];
  videos?: RecipeVideo[];

  // Category & Tags
  categoryId?: number | null;
  category?: Category | null;
  tags?: Tag[];
  tagIds?: number[];

  userId?: string;
  createdAt?: string;
}

