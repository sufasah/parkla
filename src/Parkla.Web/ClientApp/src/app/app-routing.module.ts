import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from '@app/core/components/login/login.component';
import { RegisterComponent } from '@app/core/components/register/register.component';
import { AuthGuard } from './core/guards/auth.guard';
import { MParkMapComponent } from './pages/managers/m-park-map/m-park-map.component';
import { ParkAreasComponent } from './pages/users/areas/park-areas.component';
import { LoadMoneyComponent } from './pages/users/load-money/load-money.component';
import { ParkMapComponent } from './pages/users/park-map/park-map.component';
import { ParkAreaComponent } from './pages/users/park-area/park-area.component';
import { ReservationsComponent } from './pages/users/reservations/reservations.component';
import { ProfileComponent } from './pages/users/profile/profile.component';

const routes: Routes = [
  {
    path: "",
    component: LoginComponent,
    pathMatch: "full",
    canActivate: [/*NoAuthGuard*/]
  },
  {
    path: "register",
    component: RegisterComponent,
    pathMatch: "full"
  },
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
    path: "manager",
    children: [
      {
        path: "parkmap",
        component: MParkMapComponent,
        pathMatch: "full",
      }
    ]
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
