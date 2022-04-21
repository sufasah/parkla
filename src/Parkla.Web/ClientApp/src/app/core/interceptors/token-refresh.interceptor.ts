import { Injectable, Injector } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpResponse,
  HttpErrorResponse
} from '@angular/common/http';
import { catchError, EMPTY, Observable, of, switchMap, tap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { apiAuthScheme } from '../constants/http';
import { Store } from '@ngrx/store';
import { refreshTokenExpired } from '@app/store/auth/auth.actions';

@Injectable()
export class TokenRefreshInterceptor implements HttpInterceptor {

  constructor(private injector:Injector) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    let authService = this.injector.get<AuthService>(AuthService);
    let store = this.injector.get<Store>(Store);

    return next.handle(request).pipe(catchError((err: HttpErrorResponse) => {
      if (err.status == 401 && err.headers.get("Token-Expired") == "true") {
        return authService.refreshAccessToken().pipe(
          switchMap(resp => {
            let newRequest = request.clone({
              setHeaders: {
                Authorization: apiAuthScheme+resp.accessToken
              }
            });

            return next.handle(newRequest);
          }),

          catchError((refreshTokenErr:HttpErrorResponse) => {
            if(refreshTokenErr.status == 403){
              store.dispatch(refreshTokenExpired());
              let newResponse = new HttpErrorResponse({
                ...err,
                url: err.url!,
                error: refreshTokenErr.message,
              });

              return throwError(() => newResponse);
            }

            let newResponse  = new HttpErrorResponse({
              ...err,
              url: err.url ?? "",
              error: "AccessToken Refresh Error"
            })

            return throwError(() => newResponse);
          })
        );
      }
      return throwError(() => err);
    }));
  }
}
