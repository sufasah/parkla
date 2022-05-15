import { createAction, props } from '@ngrx/store';

export const refreshTokens = createAction(
  '[Auth] Refresh Tokens',
  props<{ accessToken: string, refreshToken: string, expires: number }>()
);

export const refreshTokenExpired = createAction(
  '[Auth] Refresh Token Expired'
);

export const logout = createAction(
  "[Auth] Logout"
);
