import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Tag, TagCreate, TagUpdate } from '../models/tag';

@Injectable({
  providedIn: 'root'
})
export class TagService {
  private http = inject(HttpClient);
  private baseUrl = 'http://localhost:5254/api/tags';

  getTags(search?: string): Observable<Tag[]> {
    let params = new HttpParams();
    if (search) {
      params = params.set('search', search);
    }
    return this.http.get<Tag[]>(this.baseUrl, { params });
  }

  getTag(id: number): Observable<Tag> {
    return this.http.get<Tag>(`${this.baseUrl}/${id}`);
  }

  createTag(tag: TagCreate): Observable<Tag> {
    return this.http.post<Tag>(this.baseUrl, tag);
  }

  createTagsBatch(names: string[]): Observable<Tag[]> {
    return this.http.post<Tag[]>(`${this.baseUrl}/batch`, names);
  }

  updateTag(id: number, tag: TagUpdate): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, tag);
  }

  deleteTag(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  getPopularTags(count: number = 10): Observable<Tag[]> {
    return this.http.get<Tag[]>(`${this.baseUrl}/popular`, {
      params: new HttpParams().set('count', count.toString())
    });
  }
}
