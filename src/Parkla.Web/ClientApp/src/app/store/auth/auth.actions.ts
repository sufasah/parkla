import { createAction, props } from '@ngrx/store';

export const loginSuccess = createAction(
  '[Auth] Login Success',
  props<{ accessToken: string; refreshToken: string; expires: number; }>()
);

export const loginFailure = createAction(
  '[Auth] Login Fail',
  props<{ error: string }>()
);

export const loginVerify = createAction(
  '[Auth] Login Verify',
  props<{ message: string, username: string, password: string }>()
);

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
