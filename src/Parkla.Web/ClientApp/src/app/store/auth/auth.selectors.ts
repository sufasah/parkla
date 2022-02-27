import { createFeatureSelector, createSelector } from '@ngrx/store';
import { AppState } from '..';
import { AuthState, authStateKey } from './auth.reducer';

export const selectAuthState = createFeatureSelector<AuthState>(authStateKey);
