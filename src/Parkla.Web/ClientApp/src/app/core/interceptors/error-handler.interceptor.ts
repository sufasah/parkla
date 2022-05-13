import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { ParklaError } from '../models/parkla-error';

@Injectable()
export class ErrorHandlerInterceptor implements HttpInterceptor {

  constructor() {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((err:HttpErrorResponse) => {
        let init = {
          error: err.error,
          headers: err.headers,
          status: err.status,
          statusText: err.statusText,
          url: err.url!,
        };

        if(!(err.error instanceof ParklaError)) {
          init.error = new ParklaError(init.error);
        }
        return throwError(() => new HttpErrorResponse(init));
      })
    );
  }
}
