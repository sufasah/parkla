import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable, take } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { RouteUrl } from '../utils/route.util';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(
    private authService:AuthService,
    private router: Router){

  }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {

    let allowed = this.authService.isLoggedIn();

    if(!!route.data && !!route.data.allowedRoles)
      allowed &&= this.authService.hasRoles(route.data.allowedRoles);

    return allowed
      ? true
      : this.router.createUrlTree([RouteUrl.login()]);
  }

}
