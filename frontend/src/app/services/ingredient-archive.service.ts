import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { 
  IngredientArchive, 
  IngredientArchiveCreate, 
  IngredientArchiveUpdate,
  IngredientConversion,
  IngredientConversionCreate,
  IngredientConversionUpdate,
  IngredientCategory 
} from '../models/ingredient-archive.model';

@Injectable({
  providedIn: 'root'
})
export class IngredientArchiveService {
  private http = inject(HttpClient);
  private baseUrl = 'http://localhost:5254/api/ingredients-archive';

  // =====================
  // Ingredient CRUD
  // =====================

  getIngredients(category?: IngredientCategory, search?: string): Observable<IngredientArchive[]> {
    let url = this.baseUrl;
    const params: string[] = [];
    
    if (category !== undefined) params.push(`category=${category}`);
    if (search) params.push(`search=${encodeURIComponent(search)}`);
    
    if (params.length) url += '?' + params.join('&');
    
    return this.http.get<IngredientArchive[]>(url);
  }

  getIngredient(id: number): Observable<IngredientArchive> {
    return this.http.get<IngredientArchive>(`${this.baseUrl}/${id}`);
  }

  createIngredient(ingredient: IngredientArchiveCreate): Observable<IngredientArchive> {
    return this.http.post<IngredientArchive>(this.baseUrl, ingredient);
  }

  updateIngredient(id: number, ingredient: IngredientArchiveUpdate): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, ingredient);
  }

  deleteIngredient(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  // =====================
  // Conversions
  // =====================

  getConversions(ingredientId: number): Observable<IngredientConversion[]> {
    return this.http.get<IngredientConversion[]>(`${this.baseUrl}/${ingredientId}/conversions`);
  }

  addConversion(ingredientId: number, conversion: IngredientConversionCreate): Observable<IngredientConversion> {
    return this.http.post<IngredientConversion>(`${this.baseUrl}/${ingredientId}/conversions`, conversion);
  }

  updateConversion(conversionId: number, conversion: IngredientConversionUpdate): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/conversions/${conversionId}`, conversion);
  }

  deleteConversion(conversionId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/conversions/${conversionId}`);
  }

  // =====================
  // Categories
  // =====================

  getCategories(): Observable<{ value: number; name: string }[]> {
    return this.http.get<{ value: number; name: string }[]>(`${this.baseUrl}/categories`);
  }
}
