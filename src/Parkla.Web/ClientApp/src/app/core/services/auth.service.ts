import { HttpClient } from '@angular/common/http';
import { Injectable, OnDestroy } from '@angular/core';
import { AccessToken } from '@app/core/models/access-token';
import { AppUser } from '@app/core/models/app-user';
import { refreshAccessToken } from '@app/store/auth/auth.actions';
import { selectAuthState } from '@app/store/auth/auth.selectors';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Store } from '@ngrx/store';
import { delay, filter, of, Subscription, tap } from 'rxjs';
import { apiUrl } from '../constants/http.const';
import { RefreshTokenResp } from '@app/core/models/refresh-token.resp';
import { NavigationEnd, Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService implements OnDestroy{
  static exampleToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL3d3dy5sb2NhbGhvc3QuY29tOjcwNzAiLCJzdWIiOiJiNTg2ZjNhNi0zMzcxLTQyZTQtYTExMy01ZmNlNWM3NmU2MmUiLCJhdWQiOiJodHRwczovL3d3dy5sb2NhbGhvc3QuY29tOjcwNzAiLCJleHAiOjE2NDU5MDY5NzEsIm5iZiI6MzAwMCwiaWF0IjoxNTE2MjM5MDIyLCJqdGkiOiJpZDEyMzEyMyIsIm5hbWUiOiJBaG1ldCBNZWhtZXQgVMO8cmsiLCJwcmVmZXJyZWRfdXNlcm5hbWUiOiJ0ZXN0dXNlciIsImVtYWlsIjoidGVzdEBlbWFpbC5jb20iLCJlbWFpbF92ZXJpZmllZCI6dHJ1ZSwiZ2VuZGVyIjoibWFsZSIsImJpcnRoZGF0ZSI6bnVsbCwicGhvbmVfbnVtYmVyIjoiMDU1NSA1NTUgNTUgNTUiLCJwaG9uZV9udW1iZXJfdmVyaWZpZWQiOmZhbHNlLCJhZGRyZXNzIjpudWxsLCJzaWQiOiJzaWQxMjMxMjMiLCJyb2xlcyI6WyJ0ZXN0MSIsInRlc3QyIl0sImdyb3VwcyI6WyJ0ZXN0Z3JvdXAxIiwidGVzdGdyb3VwMiJdfQ.nNzkVaxJo9JqM39o7x3LHiPEGv2Dh72d-_dDB77nlAE";

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
    private router: Router) {

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

  register(
    user:AppUser,
    password: string) {

      return of(true).pipe(delay(2000));
    return this.httpClient.post(`${apiUrl}/register`,{
      ...user,
      password
    });
  }

  refreshAccessToken() {
    return this.httpClient.post<RefreshTokenResp>(`http://localhost:5252/refreshToken`, {
      refreshToken: this.refreshToken
    });

    return this.httpClient.post<RefreshTokenResp>(`${apiUrl}/refreshToken`, {
      refreshToken: this.refreshToken
    }).pipe(tap(resp => {
      this.store.dispatch(refreshAccessToken({
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
    if(!(this.accessToken && this.accessToken.exp)) return false;

    return this.accessToken.exp < Date.now();
  }

  hasRoles(role:string | string[]){
    if(!(this.accessToken && this.accessToken.roles)) return false;

    return typeof role == "string"
      ? this.accessToken.roles.includes(role)
      : this.accessToken.roles.reduce((prev,cur) => prev || role.includes(cur),false);
  }

  hasAllRoles(roles: string[]){
    if(!(this.accessToken && this.accessToken.roles)) return false;
    let tokenRoles = this.accessToken.roles;

    return roles.reduce(
      (prev,cur) => prev && tokenRoles.includes(cur),
      true
    );
  }

}
