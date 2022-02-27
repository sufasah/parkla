import { accessTokenKey, refreshTokenKey } from '@app/core/constants/storage.const';
import { createReducer, on } from '@ngrx/store';
import { login, loginFailure, loginSuccess } from './auth.actions';

export const authStateKey = "auth";

export interface AuthState {
  accessToken: string | null;
  refreshToken: string | null;
  tokenLoading: boolean;
  tokenLoadSuccess: boolean | null;
  tokenLoadFail: boolean | null;
  loginError: string | null;
}

export const initAuthState: AuthState = {
  accessToken: localStorage.getItem(accessTokenKey),
  refreshToken: localStorage.getItem(refreshTokenKey),
  tokenLoading: false,
  tokenLoadSuccess: null,
  tokenLoadFail: null,
  loginError: null
};

export const authReducer = createReducer(
  initAuthState,

  on(login, state => ({
    ...state,
    tokenLoading: true
  })),
  on(loginSuccess, (state, action) => ({
    ...state,
    tokenLoading: false,
    tokenLoadSuccess: true,
    tokenLoadFail: false,
    loginError: null,
    accessToken: action.accessToken,
    refreshToken: action.refreshToken
  })),
  on(loginFailure, (state, action) =>({
    ...state,
    tokenLoading: false,
    tokenLoadSuccess: false,
    tokenLoadFail: true,
    loginError: action.error
  })),

);
