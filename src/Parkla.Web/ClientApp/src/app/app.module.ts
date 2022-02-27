import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppComponent } from './app.component';

import { SharedModule } from '@app/shared/shared.module';
import { CoreModule } from '@app/core/core.module';
import { AppRoutingModule } from './app-routing.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LoginComponent } from '@app/core/components/login/login.component';
import { RegisterComponent } from '@app/core/components/register/register.component';

import { InputTextModule } from 'primeng/inputtext'
import { InputNumberModule } from 'primeng/inputnumber'
import { ButtonModule } from 'primeng/button'
import { RippleModule } from 'primeng/ripple';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { PasswordModule } from 'primeng/password';
import { InputMaskModule } from 'primeng/inputmask';
import { SelectButtonModule } from 'primeng/selectbutton';
import { CalendarModule } from 'primeng/calendar';
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
import { HttpClientModule } from '@angular/common/http';
import { AuthEffects } from './store/auth/auth.effects';
import { EffectsModule } from '@ngrx/effects';
@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    ValuesMatchValidator
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BrowserAnimationsModule,
    ReactiveFormsModule,
    FormsModule,
    AppRoutingModule,
    CoreModule,
    SharedModule,
    InputTextModule,
    InputNumberModule,
    ButtonModule,
    RippleModule,
    AutoCompleteModule,
    PasswordModule,
    DividerModule,
    InputMaskModule,
    SelectButtonModule,
    CalendarModule,
    HttpClientModule,
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
            authScheme: "Bearer ",
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
    AuthService
  ],
  bootstrap: [
    AppComponent
  ]
})
export class AppModule { }
