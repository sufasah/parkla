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
import { StepsModule } from 'primeng/steps';
import { FileUploadModule } from 'primeng/fileupload';
import { TableModule } from 'primeng/table';
import { TabViewModule } from 'primeng/tabview';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogModule } from 'primeng/dialog';
import { InputSwitchModule } from 'primeng/inputswitch';
import { SelectButtonModule } from 'primeng/selectbutton';
import { ToolbarModule } from 'primeng/toolbar';
import { KnobModule } from 'primeng/knob';
import { ChartModule } from 'primeng/chart';
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
import { SkeletonModule } from 'primeng/skeleton';
import { DataViewModule } from 'primeng/dataview';
import { ParkMapComponent } from '@app/pages/users/park-map/park-map.component';
import { ParkAreaComponent } from '@app/pages/users/park-area/park-area.component';
import { TokenRefreshInterceptor } from './core/interceptors/token-refresh.interceptor';
import { apiAuthScheme } from './core/constants/http';
import { ParkTemplateComponent } from './shared/components/area-template/area-template.component';
import { UserAvatarComponent } from './shared/components/user-avatar/user-avatar.component';
import { ProfileComponent } from './pages/users/profile/profile.component';
import { UserService } from './core/services/user.service';
import { ReservationsComponent } from './pages/users/reservations/reservations.component';
import { ReservationService } from './core/services/reservation.service';
import { MapMarkerComponent } from './shared/components/map-marker/map-marker.component';
import { ParkAreasComponent } from './pages/users/park-areas/park-areas.component';
import { AreaDataViewComponent } from './shared/components/area-dataview/area-dataview.component';
import { LoadMoneyComponent } from './pages/users/load-money/load-money.component';
import { MapComponent } from './shared/components/map/map.component';
import { MParkMapComponent } from './pages/managers/m-park-map/m-park-map.component';
import { MProfileComponent } from './pages/managers/m-profile/m-profile.component';
import { MParkAreasComponent } from './pages/managers/m-park-areas/m-park-areas.component';
import { MParkAreaComponent } from './pages/managers/m-park-area/m-park-area.component';
import { MParkAreaQRComponent } from './pages/managers/m-park-area-qr/m-park-area-qr.component';
import { MNewParkAreaComponent } from './pages/managers/m-new-park-area/m-new-park-area.component';
import { MNewParkComponent } from './pages/managers/m-new-park/m-new-park.component';
import { MEditParkAreaComponent } from './pages/managers/m-edit-park-area/m-edit-park-area.component';
import { MEditParkComponent } from './pages/managers/m-edit-park/m-edit-park.component';
import { MDashboardComponent } from './pages/managers/m-dashboard/m-dashboard.component';
import { QRCodeModule } from 'angularx-qrcode';
import { TimeRangeComponent } from './shared/components/time-range/time-range.component';
import { EditAreaTemplateComponent } from './shared/components/edit-area-template/edit-area-template.component';
import { ErrorHandlerInterceptor } from './core/interceptors/error-handler.interceptor';
import { CityService } from './core/services/city.service';
import { DistrictService } from './core/services/district.service';
import { ParkAreaService } from './core/services/park-area.service';
import { ParkSpaceService } from './core/services/park-space.service';
import { ParkService } from './core/services/park.service';
import { PricingService } from './core/services/pricing.service';
import { RealParkSpaceService } from './core/services/real-park-space.service';
import { VerifyComponent } from './shared/components/verify/verify.component';
import { SignalrService } from './core/services/signalr.service';
import { ParkFormComponent } from './shared/components/park-form/park-form.component';
import { ParkAreaFormComponent } from './shared/components/park-area-form/park-area-form.component';
import { AreaTemplateFormComponent } from './shared/components/area-template-form/area-template-form.component';
import { MEditAreaTemplateComponent } from './pages/managers/m-edit-area-template/m-edit-area-template.component';
import { UserFormComponent } from './shared/components/user-form/user-form.component';
import { ReservationModalComponent } from './shared/components/reservation-modal/reservation-modal.component';
import { DatePipe } from '@angular/common';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    ValuesMatchValidator,
    ReservationsComponent,
    TimeRangeComponent,
    SpinnerComponent,
    MapMarkerComponent,
    ParkMapComponent,
    ProfileComponent,
    MapComponent,
    MParkMapComponent,
    ParkAreaComponent,
    ParkTemplateComponent,
    LoadMoneyComponent,
    UserAvatarComponent,
    MProfileComponent,
    MParkAreasComponent,
    MParkAreaComponent,
    MParkAreaQRComponent,
    MNewParkAreaComponent,
    MNewParkComponent,
    MEditParkAreaComponent,
    MEditAreaTemplateComponent,
    EditAreaTemplateComponent,
    MEditParkComponent,
    MDashboardComponent,
    ParkAreasComponent,
    AreaDataViewComponent,
    VerifyComponent,
    ParkFormComponent,
    ParkAreaFormComponent,
    AreaTemplateFormComponent,
    UserFormComponent,
    ReservationModalComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BrowserAnimationsModule,
    ReactiveFormsModule,
    StepsModule,
    QRCodeModule,
    FormsModule,
    DropdownModule,
    TabMenuModule,
    KnobModule,
    BadgeModule,
    TableModule,
    SlideMenuModule,
    SkeletonModule,
    DataViewModule,
    ToolbarModule,
    TabViewModule,
    AppRoutingModule,
    AvatarModule,
    InputTextModule,
    DialogModule,
    InputNumberModule,
    KeyFilterModule,
    FileUploadModule,
    ConfirmDialogModule,
    ButtonModule,
    RippleModule,
    ChartModule,
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
      AuthEffects,
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
            skipWhenExpired: false,
            tokenGetter: (request) => {
              if(request && request.headers.has("authorization"))
                return null;
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
      useClass: ErrorHandlerInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TokenRefreshInterceptor,
      multi: true
    },
    ConfirmationService,
    UserService,
    ReservationService,
    CityService,
    SignalrService,
    DistrictService,
    ParkAreaService,
    ParkSpaceService,
    DatePipe,
    ParkService,
    PricingService,
    RealParkSpaceService
  ],
  bootstrap: [
    AppComponent
  ]
})
export class AppModule { }
