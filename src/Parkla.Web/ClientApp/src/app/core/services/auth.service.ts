import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AccessToken } from '@app/models/access-token';
import { AppUser } from '@app/models/user';
import { login, loginFailure, loginSuccess } from '@app/store/auth/auth.actions';
import { selectAuthState } from '@app/store/auth/auth.selectors';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Store } from '@ngrx/store';
import { delay, of, Subscription, take } from 'rxjs';
import { apiUrl } from '../constants/http.const';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  static exampleToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJodHRwczovL3d3dy5sb2NhbGhvc3QuY29tOjcwNzAiLCJzdWIiOiJiNTg2ZjNhNi0zMzcxLTQyZTQtYTExMy01ZmNlNWM3NmU2MmUiLCJhdWQiOiJodHRwczovL3d3dy5sb2NhbGhvc3QuY29tOjcwNzAiLCJleHAiOjE2NDU5MDY5NzEsIm5iZiI6MzAwMCwiaWF0IjoxNTE2MjM5MDIyLCJqdGkiOiJpZDEyMzEyMyIsIm5hbWUiOiJBaG1ldCBNZWhtZXQgVMO8cmsiLCJwcmVmZXJyZWRfdXNlcm5hbWUiOiJ0ZXN0dXNlciIsImVtYWlsIjoidGVzdEBlbWFpbC5jb20iLCJlbWFpbF92ZXJpZmllZCI6dHJ1ZSwiZ2VuZGVyIjoibWFsZSIsImJpcnRoZGF0ZSI6bnVsbCwicGhvbmVfbnVtYmVyIjoiMDU1NSA1NTUgNTUgNTUiLCJwaG9uZV9udW1iZXJfdmVyaWZpZWQiOmZhbHNlLCJhZGRyZXNzIjpudWxsLCJzaWQiOiJzaWQxMjMxMjMiLCJyb2xlcyI6WyJ0ZXN0MSIsInRlc3QyIl0sImdyb3VwcyI6WyJ0ZXN0Z3JvdXAxIiwidGVzdGdyb3VwMiJdfQ.nNzkVaxJo9JqM39o7x3LHiPEGv2Dh72d-_dDB77nlAE";

  private _accessToken?: AccessToken | null = null;
  private _refreshToken?: string | null = null;

  public get accessToken(){
    return this._accessToken;
  }

  public get refreshToken(){
    return this._refreshToken;
  }

  private authStateSubscription?:Subscription;

  constructor(
    private store:Store,
    private httpClient: HttpClient,
    private jwtHelper: JwtHelperService) {

    this.authStateSubscription =  store.select(selectAuthState).subscribe(state => {
      this._accessToken = state.accessToken ? jwtHelper.decodeToken<AccessToken>(state.accessToken) : null;
      this._refreshToken = state.refreshToken;
    });
  }

  login(
    email:string,
    password:string){

    this.store.dispatch(login({email,password}));
  }

  register(
    user:AppUser,
    password:string){
    return of(false).pipe(delay(2000));
    return this.httpClient.post(`${apiUrl}/register`,{
      ...user,
      password
    });
  }

  isLoggedIn() {
    return !!this.accessToken;
  }

  isTokenExpired() {
    if(!(this.accessToken && this.accessToken.exp)) return false;

    return this.accessToken.exp < Date.now();
  }

  hasRole(role:string | string[]){
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

  hasGroup(group:string | string[]){
    if(!(this.accessToken && this.accessToken.groups)) return false;

    return typeof group == "string"
      ? this.accessToken.groups.includes(group)
      : this.accessToken.groups.reduce((prev,cur) => prev || group.includes(cur),false);
  }

  hasAllGroups(groups: string[]){
    if(!(this.accessToken && this.accessToken.groups)) return false;
    let tokenGroups = this.accessToken.groups;

    return groups.reduce(
      (prev,cur) => prev && tokenGroups.includes(cur),
      true
    );
  }

}
