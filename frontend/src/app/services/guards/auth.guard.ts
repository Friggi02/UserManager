import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../auth.service';
import { inject } from '@angular/core';

export const canActivate = (
  next: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
): boolean => {
  const authService = inject(AuthService);
  return authService.getAccessToken() != null;
};

@Injectable({
  providedIn: 'root',
})
export class AuthGuard {
  canActivate = canActivate;
}