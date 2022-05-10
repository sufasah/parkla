import { accessTokenKey, expiresKey, refreshTokenKey } from '@app/core/constants/storage';
import { createReducer, on } from '@ngrx/store';
import { loginFailure, loginSuccess, logout, refreshTokens, refreshTokenExpired, loginVerify } from './auth.actions';
import { clearStorageTokens, setStorageTokens } from "@app/core/utils/storage";

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
  on(refreshTokens, (state, action) => {
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
