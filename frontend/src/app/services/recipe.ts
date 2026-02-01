import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Recipe } from '../models/recipe';

export interface RecipeFilter {
  categoryId?: number | null;
  tags?: string;
  search?: string;
}

@Injectable({
  providedIn: 'root'
})
export class RecipeService {
  private apiUrl = 'http://localhost:5254/api/recipes'; // Adjust port if needed

  constructor(private http: HttpClient) { }

  getRecipes(filter?: RecipeFilter): Observable<Recipe[]> {
    let params = new HttpParams();
    if (filter?.categoryId) {
      params = params.set('categoryId', filter.categoryId.toString());
    }
    if (filter?.tags) {
      params = params.set('tags', filter.tags);
    }
    if (filter?.search) {
      params = params.set('search', filter.search);
    }
    return this.http.get<Recipe[]>(this.apiUrl, { params });
  }

  getRecipe(id: number): Observable<Recipe> {
    return this.http.get<Recipe>(`${this.apiUrl}/${id}`);
  }

  createRecipe(recipe: Recipe): Observable<Recipe> {
    return this.http.post<Recipe>(this.apiUrl, recipe);
  }

  updateRecipe(id: number, recipe: Recipe): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, recipe);
  }

  uploadImage(file: File): Observable<{url: string}> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{url: string}>(`${this.apiUrl}/upload-image`, formData);
  }

  downloadPdf(id: number, pieces?: number, pieceWeight?: number): void {
      let url = `${this.apiUrl}/${id}/pdf`;
      const params = [];
      if (pieces) params.push(`pieces=${pieces}`);
      if (pieceWeight) params.push(`pieceWeight=${pieceWeight}`);
      
      if (params.length > 0) {
          url += `?${params.join('&')}`;
      }
      
      window.open(url, '_blank');
  }

  resizeRecipe(id: number, newFlour?: number, pieces?: number, pieceWeight?: number): Observable<any> {
    let params = new HttpParams();
    if (newFlour) params = params.set('newFlour', newFlour);
    if (pieces) params = params.set('pieces', pieces);
    if (pieceWeight) params = params.set('pieceWeight', pieceWeight);

    return this.http.post<any>(`${this.apiUrl}/${id}/resize`, {}, { params });
  }

  deleteRecipe(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
