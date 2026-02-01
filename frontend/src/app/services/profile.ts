import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { UserProfile, UpdateProfileDto } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  private apiUrl = 'http://localhost:5254/api/profile';

  // Reactive state for profile
  private _profile = signal<UserProfile | null>(null);
  readonly profile = this._profile.asReadonly();

  constructor(private http: HttpClient) {}

  /**
   * Get current user's profile
   */
  getProfile(): Observable<UserProfile> {
    return this.http.get<UserProfile>(this.apiUrl).pipe(
      tap(profile => this._profile.set(profile))
    );
  }

  /**
   * Update current user's profile
   */
  updateProfile(dto: UpdateProfileDto): Observable<UserProfile> {
    return this.http.put<UserProfile>(this.apiUrl, dto).pipe(
      tap(profile => this._profile.set(profile))
    );
  }

  /**
   * Update user's avatar via URL
   */
  updateAvatarUrl(avatarUrl: string): Observable<UserProfile> {
    return this.http.put<UserProfile>(`${this.apiUrl}/avatar`, { avatarUrl }).pipe(
      tap(profile => this._profile.set(profile))
    );
  }

  /**
   * Upload avatar image file
   */
  uploadAvatar(file: File): Observable<UserProfile> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<UserProfile>(`${this.apiUrl}/avatar/upload`, formData).pipe(
      tap(profile => this._profile.set(profile))
    );
  }

  /**
   * Upload banner image file
   */
  uploadBanner(file: File): Observable<UserProfile> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<UserProfile>(`${this.apiUrl}/banner/upload`, formData).pipe(
      tap(profile => this._profile.set(profile))
    );
  }

  /**
   * Get user initials for avatar placeholder
   */
  getInitials(profile: UserProfile | null): string {
    if (!profile) return '?';
    
    if (profile.displayName) {
      const parts = profile.displayName.trim().split(/\s+/);
      if (parts.length >= 2) {
        return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
      }
      return profile.displayName.substring(0, 2).toUpperCase();
    }
    
    // Fallback to email
    const emailName = profile.email.split('@')[0];
    return emailName.substring(0, 2).toUpperCase();
  }

  /**
   * Clear cached profile (on logout)
   */
  clearProfile(): void {
    this._profile.set(null);
  }

  /**
   * Get full image URL (handles relative paths from backend)
   */
  getImageUrl(imageUrl: string | null | undefined): string | null {
    if (!imageUrl) return null;
    
    // If it's already an absolute URL, return as is
    if (imageUrl.startsWith('http://') || imageUrl.startsWith('https://')) {
      return imageUrl;
    }
    
    // If it's a relative path from backend, prepend the API base URL
    if (imageUrl.startsWith('/uploads/')) {
      return `http://localhost:5254${imageUrl}`;
    }
    
    return imageUrl;
  }

  /**
   * Get full avatar URL (handles relative paths from backend)
   * @deprecated Use getImageUrl instead
   */
  getAvatarUrl(avatarUrl: string | null | undefined): string | null {
    return this.getImageUrl(avatarUrl);
  }
}
