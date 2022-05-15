import { Injectable, Injector } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse,
} from '@angular/common/http';
import { catchError, EMPTY, filter, mergeMap, Observable, of, switchMap, tap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { apiAuthScheme } from '../constants/http';
import { Store } from '@ngrx/store';
import { refreshTokenExpired } from '@app/store/auth/auth.actions';
import { Router } from '@angular/router';
import { RouteUrl } from '../utils/route';
import { ParklaError } from '../models/parkla-error';
import { TokenResponse } from '../server-models/token';
import { selectAuthState } from '@app/store/auth/auth.selectors';

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
        let refreshObservable = authService.refreshTokens();
        if(!refreshObservable) {
          return store.select(selectAuthState).pipe(
            filter(x => authService.tokenRefreshing),
            mergeMap(state => {
              if(!!state.accessToken)
                return this.onRefreshSuccess(state.accessToken,request,next);
              else {
                this.onRefreshFail(store);
                return throwError(() => new HttpErrorResponse({
                  error: new ParklaError("Token is not refreshed")
                }));
              }
            })
          );
        }
        else return this.refreshLogic(refreshObservable, store, request, next);
      }
      return throwError(() => err);
    }));
  }

  private refreshLogic(refreshObservable: Observable<TokenResponse>, store: Store, request: HttpRequest<unknown>, next: HttpHandler) {
    return refreshObservable.pipe(
      catchError(
        (httpError:HttpErrorResponse) => {
          if(httpError.error instanceof ParklaError && httpError.error.isServerError) {
            this.onRefreshFail(store);
          }
          return throwError(() => httpError)
        }
      ),
      switchMap(
        resp => {
          return this.onRefreshSuccess(resp.accessToken, request,next);
        }
      ),
    );
  }

  private onRefreshSuccess(accessToken: string, request: HttpRequest<unknown>, next:HttpHandler) {
    const newRequest = request.clone({
      setHeaders: {
        "Authorization": apiAuthScheme + accessToken
      }
    });
    return next.handle(newRequest);
  }

  private onRefreshFail(store: Store) {
    store.dispatch(refreshTokenExpired());
    setTimeout(() => {
      window.location.href = window.location.origin + "/"+ RouteUrl.login();
    }, 3000);
  }
}
