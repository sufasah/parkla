import { ActionReducerMap, MetaReducer } from '@ngrx/store';
import { environment } from '../../environments/environment';
import { authReducer, AuthState, authStateKey, initAuthState } from './auth/auth.reducer';
import { initMapState, mapReducer, MapState, mapStateKey } from './map/map.reducer';

export const stateFeatureKey = 'state';

export interface AppState {
  [authStateKey]: AuthState;
  [mapStateKey]: MapState;
}

export const initAppState: AppState = {
  [authStateKey]: initAuthState,
  [mapStateKey]: initMapState
}

export const reducers: ActionReducerMap<AppState> = {
  [authStateKey]: authReducer,
  [mapStateKey]: mapReducer
};

export const metaAppReducers: MetaReducer<AppState>[] = !environment.production ? [] : [];
