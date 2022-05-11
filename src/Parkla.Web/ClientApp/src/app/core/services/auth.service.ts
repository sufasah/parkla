import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, OnDestroy } from '@angular/core';
import { AccessToken } from '@app/core/models/access-token';
import { AppUser } from '@app/core/models/app-user';
import { loginFailure, loginSuccess, refreshTokens } from '@app/store/auth/auth.actions';
import { selectAuthState } from '@app/store/auth/auth.selectors';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Store } from '@ngrx/store';
import { catchError, filter, map, Subscription, tap, throwError } from 'rxjs';
import { apiAuthScheme, apiLogin, apiRefreshToken, apiRegister, apiUrl, apiVerification } from '../constants/http';
import { TokenResponse } from '@app/core/server-models/token';
import { NavigationEnd, Router } from '@angular/router';
import { Gender } from '../enums/Gender';

@Injectable({
  providedIn: 'root'
})
export class AuthService implements OnDestroy{
  private _accessToken: AccessToken | null = null;
  private _refreshToken: string | null = null;

  private authStateSubscription?:Subscription;
  private routeSubscription?:Subscription;

  asManager: boolean = false;

  get accessToken(){
    return this._accessToken;
  }

  get refreshToken(){
    return this._refreshToken;
  }

  constructor(
    private store:Store,
    private httpClient: HttpClient,
    private jwtHelper: JwtHelperService,
    private router: Router
    ) {
      this.authStateSubscription = store.select(selectAuthState).subscribe(state => {
        this._accessToken = state.accessToken ? this.jwtHelper.decodeToken<AccessToken>(state.accessToken) : null;
        this._refreshToken = state.refreshToken;
      });

      this.routeSubscription = router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event) => {
        let navEnd = <NavigationEnd> event;
        this.asManager = navEnd.urlAfterRedirects.toLowerCase().includes("manager");
      });
    }


  ngOnDestroy(): void {
    this.authStateSubscription?.unsubscribe();
    this.routeSubscription?.unsubscribe();
  }

  login(username: string, password: string) {
    return this.httpClient.post<TokenResponse | string>(
      apiLogin,
      {
        username: username,
        password: password
      }
    ).pipe(
      catchError((err:HttpErrorResponse) => {
        this.store.dispatch(loginFailure({error: err.error.message}))
        return throwError(() => err);
      }),
      map(result => {
        let isStr = typeof result === "string";
        if(!isStr) {
          let tokens = <TokenResponse> result;
          this.store.dispatch(loginSuccess({
            accessToken: tokens.accessToken,
            refreshToken: tokens.refreshToken,
            expires: tokens.expires
          }));
          return true;
        }
        return false;
      })
    );
  }

  verify(username:string, verificationCode: string) {
    return this.httpClient.post(apiVerification, {
      username: username,
      verificationCode: verificationCode
    })
  }

  register(user: AppUser, password: string) {

    return this.httpClient.post(apiRegister,{
      username: user.username,
      password: password,
      email: user.email,
      name: user.name,
      surname: user.surname,
      phone: user.phone!,
      cityId: user.city ? user.city.id : null,
      districtId: user.district ? user.district.id : null,
      gender: user.gender ? Gender[user.gender] : null,
      address: user.address ? user.address : null ,
      birthdate: user.birthdate ? user.birthdate : null,
    });
  }

  refreshTokens() {
    return this.httpClient.get<TokenResponse>(apiRefreshToken, {
      headers: {
        "Authorization": apiAuthScheme + this.refreshToken
      }
    }).pipe(tap(resp => {
      this.store.dispatch(refreshTokens({
        accessToken: resp.accessToken,
        refreshToken: resp.refreshToken,
        expires: resp.expires
      }))
    }));
  }

  isLoggedIn() {
    return !!this.accessToken;
  }

  isTokenExpired() {
    if(!(this.accessToken && this.accessToken.exp))
      throw "Access token or exp value is not found";
    return this.accessToken.exp < Date.now();
  }

}
