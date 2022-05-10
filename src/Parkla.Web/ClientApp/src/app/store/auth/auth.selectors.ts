import { createFeatureSelector } from '@ngrx/store';
import { AuthState, authStateKey } from './auth.reducer';

export const selectAuthState = createFeatureSelector<AuthState>(authStateKey);
