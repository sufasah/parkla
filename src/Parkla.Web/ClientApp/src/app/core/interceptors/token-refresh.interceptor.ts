import { Injectable, Injector } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse,
} from '@angular/common/http';
import { catchError, EMPTY, Observable, of, switchMap, tap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { apiAuthScheme } from '../constants/http';
import { Store } from '@ngrx/store';
import { refreshTokenExpired } from '@app/store/auth/auth.actions';
import { Router } from '@angular/router';
import { RouteUrl } from '../utils/route';
import { ParklaError } from '../models/parkla-error';

@Injectable()
export class TokenRefreshInterceptor implements HttpInterceptor {

  constructor(
    private injector:Injector,
    private router: Router
  ) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    let authService = this.injector.get<AuthService>(AuthService);
    let store = this.injector.get<Store>(Store);

    return next.handle(request).pipe(catchError((err: HttpErrorResponse) => {
        if (err.status == 401 && err.headers.get("Token-Expired") == "true") {
          return authService.refreshTokens().pipe(
            catchError((refreshTokenErr:HttpErrorResponse) => {
              if(refreshTokenErr.error instanceof ParklaError && refreshTokenErr.error.isServerError) {
                store.dispatch(refreshTokenExpired());
                setTimeout(() => {
                  window.location.href = window.location.origin + "/"+ RouteUrl.login();
                }, 3000);
              }
              return throwError(() => refreshTokenErr);
            }),
            switchMap(resp => {
              const newRequest = request.clone({
                setHeaders: {
                  "Authorization": apiAuthScheme + resp.accessToken
                }
              });
              return next.handle(newRequest);
          }),
        );
      }
      return throwError(() => err);
    }));
  }
}
