import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from '@app/core/components/login/login.component';
import { RegisterComponent } from '@app/core/components/register/register.component';
import { UsersRoutingModule } from '@app/pages/users/users-routing.module'
import { ManagersRoutingModule } from './pages/managers/managers-routing.module';
import { TestComponent } from './pages/users/test/test.component';

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
    path: "test",
    component: TestComponent,
    pathMatch: "full",
    data:{
      allowedRoles: ["test1"],
    }
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
  ],
  exports: [
    RouterModule
  ]
})
export class AppRoutingModule { }
