import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { apiLoginUrl } from '@app/core/constants/http.const';
import { accessTokenKey, expiresKey, refreshTokenKey } from '@app/core/constants/storage.const';
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
      if(true){
        return loginSuccess({
          accessToken: AuthService.exampleToken,
          refreshToken: "examplerefreshtoken",
          expires: Date.now()+ 3600*1000 // 3600 seconds
        });
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

        return loginSuccess({
          accessToken: tokens.accessToken,
          refreshToken: tokens.refreshToken,
          expires: tokens.expires
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
