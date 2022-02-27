import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { apiUrl } from '@app/core/constants/http.const';
import { AuthService } from '@app/core/services/auth.service';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, of, switchMap, tap } from 'rxjs';
import { login, loginFailure, loginSuccess } from './auth.actions';


@Injectable()
export class AuthEffects {

  login$ = createEffect(() => this.actions$.pipe(
    tap(console.log),
    ofType(login),
    map(action => {
      return loginSuccess({
        accessToken: AuthService.exampleToken,
        refreshToken: "examplerefreshtoken"}
      );

      /*return this.httpClient.post<{accessToken: string; refreshToken:string;}>(
        `${apiUrl}/login`,
        {
          username: action.username,
          password: action.password
        }
      ).pipe(map((tokens) => loginSuccess({
          accessToken: tokens.accessToken,
          refreshToken: tokens.refreshToken
      })));*/
    }),
    //switchMap(x => x),
    catchError((err) => of(loginFailure(err)))
  ));

  de = createEffect(()=>this.actions$.pipe(tap(console.log)))
  constructor(
    private actions$: Actions,
    private httpClient: HttpClient) {}

}
