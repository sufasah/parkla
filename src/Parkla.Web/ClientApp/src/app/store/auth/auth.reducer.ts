import { accessTokenKey, expiresKey, refreshTokenKey } from '@app/core/constants/storage';
import { createReducer, on } from '@ngrx/store';
import { logout, refreshTokens, refreshTokenExpired } from './auth.actions';
import { clearStorageTokens, setStorageTokens } from "@app/core/utils/storage";

export const authStateKey = "auth";

export interface AuthState {
  accessToken: string | null;
  refreshToken: string | null;
  expires: number | null;
}

export const initAuthState: AuthState = {
  accessToken: localStorage.getItem(accessTokenKey),
  refreshToken: localStorage.getItem(refreshTokenKey),
  expires: (() => {
    let expires = localStorage.getItem(expiresKey);
    return expires != null ? parseInt(expires) : null;
  })(),
};

export const authReducer = createReducer(
  initAuthState,
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
