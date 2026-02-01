
import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap, catchError } from 'rxjs/operators';
import { of, throwError } from 'rxjs';

export interface LoginResponse {
  accessToken: string;
  expiresIn: number;
  refreshToken: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:5254'; 
  
  // Zoneless State
  private _accessToken = signal<string | null>(localStorage.getItem('accessToken'));
  private _userEmail = signal<string | null>(localStorage.getItem('userEmail'));

  isAuthenticated = computed(() => !!this._accessToken());
  currentUserName = computed(() => {
    const email = this._userEmail();
    if (!email) return 'Utente';
    // Extract name from email (e.g., "john.doe@example.com" -> "John Doe")
    const namePart = email.split('@')[0];
    return namePart.split(/[._]/).map(word => 
      word.charAt(0).toUpperCase() + word.slice(1).toLowerCase()
    ).join(' ');
  });

  private tokenTimer: any;

  constructor(private http: HttpClient, private router: Router) {
      if (this._accessToken()) {
          // Check validity on startup
          if (this.isTokenExpired()) {
              this.logout();
          } else {
              this.startTokenTimer();
          }
      }
  }

  register(email: string, password: string) {
    return this.http.post(`${this.apiUrl}/register`, { email, password });
  }

  login(email: string, password: string) {
    // using .NET Identity mapIdentityApi standard endpoint
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, { email, password }).pipe(
      tap(response => {
        this.setToken(response.accessToken, response.expiresIn);
        this._userEmail.set(email);
        localStorage.setItem('userEmail', email);
      })
    );
  }

  logout() {
    this.stopTokenTimer();
    this._accessToken.set(null);
    this._userEmail.set(null);
    localStorage.removeItem('accessToken');
    localStorage.removeItem('tokenExpiration');
    localStorage.removeItem('userEmail');
    this.router.navigate(['/login']);
  }

  getToken() {
    return this._accessToken();
  }

  private setToken(token: string, expiresIn?: number) {
    this._accessToken.set(token);
    localStorage.setItem('accessToken', token);
    
    if (expiresIn) {
        // Calculate expiration date in milliseconds
        const expirationTime = new Date().getTime() + (expiresIn * 1000);
        localStorage.setItem('tokenExpiration', expirationTime.toString());
    }
    
    this.startTokenTimer();
  }

  public isTokenExpired(): boolean {
      if (!this._accessToken()) return true;

      const expirationStr = localStorage.getItem('tokenExpiration');
      if (!expirationStr) {
          // Fallback: If no expiration is stored, assume valid until server says 401
          // The previous JWT decoding logic was failing because .NET Identity tokens are often opaque
          return false; 
      }

      const expirationTime = parseInt(expirationStr, 10);
      return new Date().getTime() > expirationTime;
  }

  private startTokenTimer() {
      this.stopTokenTimer();
      // Check every 30 seconds
      this.tokenTimer = setInterval(() => {
          if (this.isTokenExpired()) {
              this.logout();
          }
      }, 30000); 
  }

  private stopTokenTimer() {
      if (this.tokenTimer) {
          clearInterval(this.tokenTimer);
          this.tokenTimer = null;
      }
  }
}
