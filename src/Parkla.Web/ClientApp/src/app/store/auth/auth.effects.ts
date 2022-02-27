import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { apiLoginUrl } from '@app/core/constants/http.const';
import { accessTokenKey, refreshTokenKey } from '@app/core/constants/storage.const';
import { AuthService } from '@app/core/services/auth.service';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, delay, map, of, switchMap, tap } from 'rxjs';
import { login, loginFailure, loginSuccess } from './auth.actions';


@Injectable()
export class AuthEffects {

  login$ = createEffect(() => this.actions$.pipe(
    ofType(login),
    delay(1000), // -
    map(action => {
      localStorage.setItem(accessTokenKey,AuthService.exampleToken);
      localStorage.setItem(refreshTokenKey, "examplerefreshtoken");

      if(false){
        return loginSuccess({
          accessToken: AuthService.exampleToken,
          refreshToken: "examplerefreshtoken"}
        );
      }
      else {
        return loginFailure({error: "Login failed"});
      }

      /* +
      return this.httpClient.post<{accessToken: string; refreshToken:string;}>(
        apiLoginUrl,
        {
          username: action.username,
          password: action.password
        }
      ).pipe(map((tokens) => {
        localStorage.setItem(accessTokenKey,tokens.accessToken);
        localStorage.setItem(refreshTokenKey, tokens.refreshToken);

        return loginSuccess({
          accessToken: tokens.accessToken,
          refreshToken: tokens.refreshToken
        });
      }));*/
    }),
    // + switchMap(x => x),
    catchError((err) => of(loginFailure(err)))
  ));

  constructor(
    private actions$: Actions,
    private httpClient: HttpClient) {}

}
