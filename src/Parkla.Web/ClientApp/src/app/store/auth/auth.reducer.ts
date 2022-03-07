import { accessTokenKey, expiresKey, refreshTokenKey } from '@app/core/constants/storage.const';
import { createReducer, on } from '@ngrx/store';
import { login, loginFailure, loginSuccess, logout, refreshAccessToken, refreshTokenExpired } from './auth.actions';
import { clearStorageTokens, setStorageTokens } from "@core/utils/storage.util";

export const authStateKey = "auth";

export interface AuthState {
  accessToken: string | null;
  refreshToken: string | null;
  expires: number | null;
  tokenLoading: boolean;
  tokenLoadSuccess: boolean | null;
  tokenLoadFail: boolean | null;
  loginError: string | null;
}

export const initAuthState: AuthState = {
  accessToken: localStorage.getItem(accessTokenKey),
  refreshToken: localStorage.getItem(refreshTokenKey),
  expires: (() => {
    let expires = localStorage.getItem(expiresKey);
    return expires != null ? parseInt(expires) : null;
  })(),
  tokenLoading: false,
  tokenLoadSuccess: null,
  tokenLoadFail: null,
  loginError: null
};

export const authReducer = createReducer(
  initAuthState,

  on(login, state => ({
    ...state,
    tokenLoading: true,
    tokenLoadSuccess: null,
    tokenLoadFail: null
  })),
  on(loginSuccess, (state, action) => {
    setStorageTokens(
      action.accessToken,
      action.refreshToken,
      action.expires.toString());

    return {
      ...state,
      tokenLoading: false,
      tokenLoadSuccess: true,
      tokenLoadFail: false,
      loginError: null,
      accessToken: action.accessToken,
      refreshToken: action.refreshToken,
      expiresKey: action.expires
    }
  }),
  on(loginFailure, (state, action) => ({
    ...state,
    tokenLoading: false,
    tokenLoadSuccess: false,
    tokenLoadFail: true,
    loginError: action.error
  })),
  on(refreshAccessToken, (state, action) => {
    setStorageTokens(
      action.accessToken,
      action.refreshToken,
      action.expires.toString());

    return {
      ...state,
      accessToken: action.accessToken,
      refreshToken: action.refreshToken,
      expires: action.expires
    }
  }),
  on(refreshTokenExpired, (state) => {
    clearStorageTokens();

    return {
      ...state,
      accessToken: null,
      refreshToken: null,
      expires: null
    }
  }),
  on(logout, (state) => {
    clearStorageTokens();

    return {
      ...state,
      accessToken: null,
      refreshToken: null,
      expires: null
    }
  })
);
