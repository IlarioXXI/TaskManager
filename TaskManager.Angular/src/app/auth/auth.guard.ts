import { ActivatedRouteSnapshot, CanActivateChildFn, CanActivateFn, RouterStateSnapshot } from '@angular/router';
import { AuthService } from './auth.service';
import { inject } from '@angular/core';

export const canActivate: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  if (localStorage.getItem('token') == null) {
    return false;
  }else{
    return true;
  }
};

export const canActivateChild: CanActivateChildFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
  const authService = inject(AuthService);
  return authService.isAdmin;
}
