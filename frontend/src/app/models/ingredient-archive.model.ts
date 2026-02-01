export interface IngredientArchive {
  id: number;
  name: string;
  description?: string;
  defaultUnit: string;
  category: IngredientCategory;
  categoryName: string;
  icon?: string;
  color?: string;
  customProperties: IngredientCustomProperty[];
  isSystemDefault: boolean;
  sortOrder: number;
  conversionCount: number;
}

export interface IngredientArchiveCreate {
  name: string;
  description?: string;
  defaultUnit?: string;
  category: IngredientCategory;
  icon?: string;
  color?: string;
  customProperties?: IngredientCustomProperty[];
  sortOrder?: number;
}

export interface IngredientArchiveUpdate {
  name: string;
  description?: string;
  defaultUnit?: string;
  category: IngredientCategory;
  icon?: string;
  color?: string;
  customProperties?: IngredientCustomProperty[];
  sortOrder?: number;
}

export interface IngredientCustomProperty {
  label: string;
  value: string;
}

export interface IngredientConversion {
  id: number;
  fromIngredientId: number;
  toIngredientId: number;
  toIngredientName: string;
  toIngredientIcon?: string;
  toIngredientColor?: string;
  conversionRatio: number;
  notes?: string;
  isReverse: boolean;
}

export interface IngredientConversionCreate {
  toIngredientId: number;
  conversionRatio: number;
  notes?: string;
}

export interface IngredientConversionUpdate {
  conversionRatio: number;
  notes?: string;
}

// Category as string to match backend JSON serialization
export type IngredientCategory = 
  | 'Farina' 
  | 'Lievito' 
  | 'Liquido' 
  | 'Grasso' 
  | 'Dolcificante' 
  | 'Sale' 
  | 'Uova' 
  | 'Latticini' 
  | 'Altro';

export const IngredientCategories: IngredientCategory[] = [
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

export const IngredientCategoryLabels: Record<IngredientCategory, string> = {
  'Farina': 'Farina',
  'Lievito': 'Lievito',
  'Liquido': 'Liquido',
  'Grasso': 'Grasso',
  'Dolcificante': 'Dolcificante',
  'Sale': 'Sale',
  'Uova': 'Uova',
  'Latticini': 'Latticini',
  'Altro': 'Altro'
};

export const IngredientCategoryIcons: Record<IngredientCategory, string> = {
  'Farina': 'fa-solid fa-wheat-awn',
  'Lievito': 'fa-solid fa-cubes',
  'Liquido': 'fa-solid fa-droplet',
  'Grasso': 'fa-solid fa-oil-can',
  'Dolcificante': 'fa-solid fa-candy-cane',
  'Sale': 'fa-solid fa-mortar-pestle',
  'Uova': 'fa-solid fa-egg',
  'Latticini': 'fa-solid fa-cheese',
  'Altro': 'fa-solid fa-ellipsis'
};
