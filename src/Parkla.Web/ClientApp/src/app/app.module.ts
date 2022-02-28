import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppComponent } from './app.component';

import { AppRoutingModule } from './app-routing.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LoginComponent } from '@app/core/components/login/login.component';
import { RegisterComponent } from '@app/core/components/register/register.component';

import { InputTextModule } from 'primeng/inputtext'
import { BlockUIModule } from 'primeng/blockui'
import { InputNumberModule } from 'primeng/inputnumber'
import { ButtonModule } from 'primeng/button'
import { RippleModule } from 'primeng/ripple';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { PasswordModule } from 'primeng/password';
import { InputMaskModule } from 'primeng/inputmask';
import { SelectButtonModule } from 'primeng/selectbutton';
import { MessageModule } from 'primeng/message';
import { CalendarModule } from 'primeng/calendar';
import { ToastModule } from 'primeng/toast';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { DividerModule } from 'primeng/divider';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ValuesMatchValidator } from '@app/core/validators/values-match-validator.directive';
import { JwtConfig, JwtModule, JWT_OPTIONS } from '@auth0/angular-jwt';
import { Store, StoreModule } from '@ngrx/store';
import { initAppState, metaAppReducers, reducers } from '@app/store';
import { AuthGuard } from '@app/core/guards/auth.guard';
import { AuthService } from '@app/core/services/auth.service';
import { selectAuthState } from './store/auth/auth.selectors';
import { firstValueFrom } from 'rxjs';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthEffects } from './store/auth/auth.effects';
import { EffectsModule } from '@ngrx/effects';
import { SpinnerComponent } from './shared/components/spinner/spinner.component';
import { TestComponent } from './pages/users/test/test.component';
import { MessageService } from 'primeng/api';
import { ParkMapComponent } from '@app/pages/users/park-map/park-map.component';
import { TokenRefreshInterceptor } from './core/interceptors/token-refresh.interceptor';
import { apiAuthScheme } from './core/constants/http.const';
@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    ValuesMatchValidator,
    SpinnerComponent,
    TestComponent,
    ParkMapComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BrowserAnimationsModule,
    ReactiveFormsModule,
    FormsModule,
    AppRoutingModule,
    InputTextModule,
    InputNumberModule,
    ButtonModule,
    RippleModule,
    AutoCompleteModule,
    PasswordModule,
    ToastModule,
    MessageModule,
    DividerModule,
    BlockUIModule,
    InputMaskModule,
    SelectButtonModule,
    CalendarModule,
    HttpClientModule,
    ProgressSpinnerModule,
    EffectsModule.forRoot([
      AuthEffects
    ]),
    StoreModule.forRoot(reducers,{
      initialState: initAppState,
      metaReducers: metaAppReducers
    }),
    JwtModule.forRoot({
      jwtOptionsProvider: {
        provide: JWT_OPTIONS,
        useFactory: (store:Store) => {
          return <JwtConfig>{
            allowedDomains:["localhost:7070","localhost:5252"],
            authScheme: apiAuthScheme,
            tokenGetter: (request) => {
              return firstValueFrom(store.select(selectAuthState))
                .then(state => state.accessToken)
            }
          }
        },
        deps: [Store]
      }
    })
  ],
  providers: [
    AuthGuard,
    AuthService,
    MessageService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TokenRefreshInterceptor,
      multi: true
    }
  ],
  bootstrap: [
    AppComponent
  ]
})
export class AppModule { }
