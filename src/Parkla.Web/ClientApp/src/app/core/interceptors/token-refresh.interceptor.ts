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
import { apiAuthScheme } from '../constants/http.const';

@Injectable()
export class TokenRefreshInterceptor implements HttpInterceptor {

  constructor(private injector:Injector) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    let authService = this.injector.get<AuthService>(AuthService);

    return next.handle(request).pipe(catchError((err: HttpErrorResponse) => {
      if (err.status == 401 && err.headers.get("Token-Expired") == "true") {
        return authService.refreshAccessToken().pipe(switchMap(resp => {
          let newRequest = request.clone({
            setHeaders: {
              Authorization: apiAuthScheme+resp.accessToken
            }
          });

          return next.handle(newRequest);
        }));
      }
      return throwError(() => err);
    }));
  }
}
