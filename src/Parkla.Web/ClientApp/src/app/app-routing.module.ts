import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from '@core/components/login/login.component';
import { RegisterComponent } from '@core/components/register/register.component';
import { UsersRoutingModule } from '@app/users/users-routing.module'
import { ManagersRoutingModule } from './managers/managers-routing.module';

const routes: Routes = [
  {
    path: "",
    component: LoginComponent,
    pathMatch: "full",
  },
  {
    path: "register",
    component: RegisterComponent,
    pathMatch: "full"
  },
  {
    path: "**",
    pathMatch: "full",
    redirectTo: "/"
  },
];

@NgModule({
  declarations: [],
  imports: [
    RouterModule.forRoot(routes,{
        anchorScrolling: "enabled"
    }),
    UsersRoutingModule,
    ManagersRoutingModule
  ],
  exports: [
    RouterModule
  ]
})
export class AppRoutingModule { }
