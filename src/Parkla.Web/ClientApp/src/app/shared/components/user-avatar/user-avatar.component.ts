import { Component, OnInit } from '@angular/core';
import { AuthService } from '@app/core/services/auth.service';
import { logout } from '@app/store/auth/auth.actions';
import { Store } from '@ngrx/store';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-user-avatar',
  templateUrl: './user-avatar.component.html',
  styleUrls: ['./user-avatar.component.scss']
})
export class UserAvatarComponent implements OnInit {

  label: string = "";

  items: MenuItem[] = [];

  constructor(
    private authService: AuthService,
    private store: Store) { }

  ngOnInit(): void {
    this.label = this.authService.accessToken?.preferred_username![0]!;

    this.items = [
      {
        label: 'Profile',
        icon: 'pi pi-fw pi-user',
        routerLink: ["/profile"]
      },
      {
        label: 'Reservations',
        icon: 'pi pi-fw pi-car',
        routerLink: ["/reservations"]
      },
      this.authService.asManager ? {
        label: "User Mode",
        icon: "pi pi-fw pi-user",
        routerLink: ["/parkmap"]
      } : {
        label: "Manager Mode",
        icon: "pi pi-fw pi-user",
        routerLink: ["/manager/parkmap"]
      },
      {
        label: 'Logout',
        icon: 'pi pi-fw pi-sign-out',
        command: () => this.logout(),
        routerLink: ["/"]
      },
    ];
  }

  logout() {
    this.store.dispatch(logout());
  }

}
