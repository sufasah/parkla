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
import { ConfirmationService } from 'primeng/api';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { PasswordModule } from 'primeng/password';
import { BadgeModule } from 'primeng/badge';
import { InputMaskModule } from 'primeng/inputmask';
import { AvatarModule } from 'primeng/avatar';
import { TableModule } from 'primeng/table';
import { TabViewModule } from 'primeng/tabview';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogModule } from 'primeng/dialog';
import { InputSwitchModule } from 'primeng/inputswitch';
import { SelectButtonModule } from 'primeng/selectbutton';
import { ToolbarModule } from 'primeng/toolbar';
import { KnobModule } from 'primeng/knob';
import { MessageModule } from 'primeng/message';
import { CalendarModule } from 'primeng/calendar';
import { ToastModule } from 'primeng/toast';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { SlideMenuModule } from 'primeng/slidemenu';
import { TabMenuModule } from 'primeng/tabmenu';
import { DropdownModule } from 'primeng/dropdown';
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
import { MessageService } from 'primeng/api';
import { ImageModule } from 'primeng/image';
import { KeyFilterModule } from 'primeng/keyfilter';
import { DataViewModule } from 'primeng/dataview';
import { ParkMapComponent } from '@app/pages/users/park-map/park-map.component';
import { ParkAreaComponent } from '@app/pages/users/park/park-area.component';
import { TokenRefreshInterceptor } from './core/interceptors/token-refresh.interceptor';
import { apiAuthScheme } from './core/constants/http.const';
import { ParkTemplateComponent } from './shared/components/park-template/park-template.component';
import { UserAvatarComponent } from './shared/components/user-avatar/user-avatar.component';
import { ProfileComponent } from './shared/profile/profile.component';
import { UserService } from './core/services/user.service';
import { ReservationsComponent } from './pages/users/reservations/reservations.component';
import { ReservationService } from './core/services/reservation.service';
import { MapMarkerComponent } from './shared/components/map-marker/map-marker.component';
import { ParkAreasComponent } from './pages/users/areas/park-areas.component';
import { AreaDataViewComponent } from './shared/components/area-dataview/area-dataview.component';
import { RefSharingService } from './core/services/ref-sharing.service';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    ValuesMatchValidator,
    ReservationsComponent,
    SpinnerComponent,
    MapMarkerComponent,
    ParkMapComponent,
    ProfileComponent,
    ParkAreaComponent,
    ParkTemplateComponent,
    UserAvatarComponent,
    ParkAreasComponent,
    AreaDataViewComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BrowserAnimationsModule,
    ReactiveFormsModule,
    FormsModule,
    DropdownModule,
    TabMenuModule,
    KnobModule,
    BadgeModule,
    TableModule,
    SlideMenuModule,
    DataViewModule,
    ToolbarModule,
    TabViewModule,
    AppRoutingModule,
    AvatarModule,
    InputTextModule,
    DialogModule,
    InputNumberModule,
    KeyFilterModule,
    ConfirmDialogModule,
    ButtonModule,
    RippleModule,
    ImageModule,
    AutoCompleteModule,
    PasswordModule,
    ToastModule,
    MessageModule,
    DividerModule,
    BlockUIModule,
    InputMaskModule,
    InputSwitchModule,
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
    },
    ConfirmationService,
    UserService,
    ReservationService,
    RefSharingService
  ],
  bootstrap: [
    AppComponent
  ]
})
export class AppModule { }
