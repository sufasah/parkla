import { ActionReducerMap, MetaReducer } from '@ngrx/store';
import { environment } from '../../environments/environment';
import { authReducer, AuthState, authStateKey, initAuthState } from './auth/auth.reducer';

export const stateFeatureKey = 'state';

export interface AppState {
  [authStateKey]: AuthState;
}

export const initAppState: AppState = {
  [authStateKey]: initAuthState
}

export const reducers: ActionReducerMap<AppState> = {
  [authStateKey]: authReducer
};

export const metaAppReducers: MetaReducer<AppState>[] = !environment.production ? [] : [];
