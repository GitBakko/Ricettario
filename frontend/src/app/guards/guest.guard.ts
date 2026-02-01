import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth';

export const guestGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // If user is authenticated, block access to guest pages (login/register) and redirect to home
  if (authService.isAuthenticated() && !authService.isTokenExpired()) {
    return router.createUrlTree(['/']);
  }
  
  return true;
};
