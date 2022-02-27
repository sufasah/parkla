import { createAction, props } from '@ngrx/store';

export const login = createAction(
  '[Auth] Login',
  props<{ username:string; password:string} >()
);

export const loginSuccess = createAction(
  '[Auth] Login Success',
  props<{ accessToken: string; refreshToken:string; }>()
);

export const loginFailure = createAction(
  '[Auth] Login Fail',
  props<{ error: any }>()
);
