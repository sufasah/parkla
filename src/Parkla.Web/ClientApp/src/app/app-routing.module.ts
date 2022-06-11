import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from '@app/core/components/login/login.component';
import { RegisterComponent } from '@app/core/components/register/register.component';
import { AuthGuard } from './core/guards/auth.guard';
import { MParkMapComponent } from './pages/managers/m-park-map/m-park-map.component';
import { ParkAreasComponent } from './pages/users/park-areas/park-areas.component';
import { LoadMoneyComponent } from './pages/users/load-money/load-money.component';
import { ParkMapComponent } from './pages/users/park-map/park-map.component';
import { ParkAreaComponent } from './pages/users/park-area/park-area.component';
import { ReservationsComponent } from './pages/users/reservations/reservations.component';
import { ProfileComponent } from './pages/users/profile/profile.component';
import { MProfileComponent } from './pages/managers/m-profile/m-profile.component';
import { MParkAreaComponent } from './pages/managers/m-park-area/m-park-area.component';
import { MParkAreasComponent } from './pages/managers/m-park-areas/m-park-areas.component';
import { MParkAreaQRComponent } from './pages/managers/m-park-area-qr/m-park-area-qr.component';
import { MEditParkComponent } from './pages/managers/m-edit-park/m-edit-park.component';
import { MNewParkAreaComponent } from './pages/managers/m-new-park-area/m-new-park-area.component';
import { MEditParkAreaComponent } from './pages/managers/m-edit-park-area/m-edit-park-area.component';
import { MDashboardComponent } from './pages/managers/m-dashboard/m-dashboard.component';
import { NoAuthGuard } from './core/guards/no-auth.guard';
import { MNewParkComponent } from './pages/managers/m-new-park/m-new-park.component';
import { MEditAreaTemplateComponent } from './pages/managers/m-edit-area-template/m-edit-area-template.component';
import { NotFound404Component } from './core/components/not-found404/not-found404.component';

const routes: Routes = [
  {
    path: "",
    pathMatch: "full",
    redirectTo: "/user/parkmap"
  },
  {
    path: "login",
    component: LoginComponent,
    pathMatch: "full",
    canActivate: [NoAuthGuard]
  },
  {
    path: "register",
    component: RegisterComponent,
    pathMatch: "full",
    canActivate: [NoAuthGuard]
  },
  {
    path: "manager",
    children: [
      {
        path: "parkmap",
        component: MParkMapComponent,
        pathMatch: "full",
      },
      {
        path: "profile",
        component: MProfileComponent,
        pathMatch: "full",
        canActivate: [AuthGuard]
      },
      {
        path: "dashboard",
        component: MDashboardComponent,
        pathMatch: "full",
        canActivate: [AuthGuard]
      },
      {
        path: "park",
        children: [
          {
            path: "add",
            component: MNewParkComponent,
            pathMatch: "full",
            canActivate: [AuthGuard]
          },
          {
            path: ":parkid",
            children: [
              {
                path: "area",
                children: [
                  {
                    path: "add",
                    component: MNewParkAreaComponent,
                    pathMatch: "full",
                    canActivate: [AuthGuard]
                  },
                  {
                    path: ":areaid/QR",
                    component: MParkAreaQRComponent,
                    pathMatch: "full",
                    canActivate: [AuthGuard]
                  },
                  {
                    path: ":areaid/edit",
                    component: MEditParkAreaComponent,
                    pathMatch: "full",
                    canActivate: [AuthGuard]
                  },
                  {
                    path: ":areaid/edit/template",
                    component: MEditAreaTemplateComponent,
                    pathMatch: "full",
                    canActivate: [AuthGuard]
                  },
                  {
                    path: ":areaid",
                    component: MParkAreaComponent,
                    pathMatch: "full",
                    canActivate: [AuthGuard]
                  },
                ]
              },
              {
                path: "areas",
                component: MParkAreasComponent,
                pathMatch: "full",
                canActivate: [AuthGuard]
              },
              {
                path: "edit",
                component: MEditParkComponent
              }
            ]
          },
        ]
      },

    ]
  },
  {
    path: "user",
    children: [
      {
        path: "parkmap",
        component: ParkMapComponent,
        pathMatch: "full",
        data:{
          allowedRoles: ["test1"],
        },
        canActivate: [AuthGuard]
      },
      {
        path: "reservations",
        component: ReservationsComponent,
        canActivate: [AuthGuard]
      },
      {
        path: "profile",
        component: ProfileComponent,
        pathMatch: "full",
        canActivate: [AuthGuard]
      },
      {
        path: "park/:parkid/area/:areaid",
        component: ParkAreaComponent,
        pathMatch: "full",
        canActivate: [AuthGuard]
      },
      {
        path: "park/:parkid/areas",
        component: ParkAreasComponent,
        pathMatch: "full",
        canActivate: [AuthGuard]
      },
      {
        path: "load-money",
        component: LoadMoneyComponent,
        pathMatch: "full",
        canActivate: [AuthGuard]
      }
    ]
  },
  {
    path: "**",
    pathMatch: "full",
    component: NotFound404Component
  }
];

@NgModule({
  declarations: [],
  imports: [
    RouterModule.forRoot(routes,{
        anchorScrolling: "enabled"
    }),
  ],
  exports: [
    RouterModule
  ]
})
export class AppRoutingModule { }
