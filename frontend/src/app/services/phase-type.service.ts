import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { PhaseType } from '../models/phase-type';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PhaseTypeService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5254/api/RecipePhaseTypes';

  getPhaseTypes(): Observable<PhaseType[]> {
    return this.http.get<PhaseType[]>(this.apiUrl);
  }

  getPhaseType(id: number): Observable<PhaseType> {
    return this.http.get<PhaseType>(`${this.apiUrl}/${id}`);
  }

  createPhaseType(phaseType: PhaseType): Observable<PhaseType> {
    return this.http.post<PhaseType>(this.apiUrl, phaseType);
  }

  updatePhaseType(id: number, phaseType: PhaseType): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, phaseType);
  }

  deletePhaseType(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
