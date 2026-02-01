import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Proactive check: Authentication presence + Token validity
  if (authService.isAuthenticated() && !authService.isTokenExpired()) {
    return true;
  } else {
    // If expired or missing, force logout (cleanup) and redirect
    authService.logout();
    return router.createUrlTree(['/login']);
  }
};
