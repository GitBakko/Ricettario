export interface Category {
  id: number;
  name: string;
  description?: string;
  icon?: string;
  color?: string;
  sortOrder: number;
  isSystemDefault: boolean;
}

export interface CategoryCreate {
  name: string;
  description?: string;
  icon?: string;
  color?: string;
  sortOrder?: number;
}

export interface CategoryUpdate {
  name: string;
  description?: string;
  icon?: string;
  color?: string;
  sortOrder?: number;
}

export interface CategoryReorder {
  id: number;
  sortOrder: number;
}
