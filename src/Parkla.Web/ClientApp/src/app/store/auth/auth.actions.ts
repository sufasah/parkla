import { createAction, props } from '@ngrx/store';

export const login = createAction(
  '[Auth] Login',
  props<{ email:string; password:string} >()
);

export const loginSuccess = createAction(
  '[Auth] Login Success',
  props<{ accessToken: string; refreshToken: string; expires: number; }>()
);

export const loginFailure = createAction(
  '[Auth] Login Fail',
  props<{ error: string }>()
);

export const refreshAccessToken = createAction(
  '[Auth] Refresh Access Token',
  props<{ accessToken: string; refreshToken: string; expires: number; }>()
);
