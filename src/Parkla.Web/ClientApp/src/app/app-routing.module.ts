import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from '@app/core/components/login/login.component';
import { RegisterComponent } from '@app/core/components/register/register.component';
import { UsersRoutingModule } from '@app/pages/users/users-routing.module'
import { AuthGuard } from './core/guards/auth.guard';
import { NoAuthGuard } from './core/guards/no-auth.guard';
import { ManagersRoutingModule } from './pages/managers/managers-routing.module';
import { ParkMapComponent } from './pages/users/park-map/park-map.component';
import { ParkComponent } from './pages/users/park/park.component';

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
    path: "park/:id",
    component: ParkComponent,
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
