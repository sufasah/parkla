import { Component, OnInit } from '@angular/core';
import { AuthService } from '@app/core/services/auth.service';
import { RouteUrl } from '@app/core/utils/route';
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

    if(this.authService.asManager)
      this.initAsManager();
    else
      this.init();
  }

  init() {
    this.items = [
      {
        label: 'Profile',
        icon: 'pi pi-fw pi-user',
        routerLink: ["/" + RouteUrl.profile()]
      },
      {
        label: 'Reservations',
        icon: 'pi pi-fw pi-car',
        routerLink: ["/" + RouteUrl.reservations()]
      },
      {
        label: "Manager Mode",
        icon: "pi pi-fw pi-user",
        routerLink: ["/" + RouteUrl.mParkMap()]
      },
      {
        label: 'Logout',
        icon: 'pi pi-fw pi-sign-out',
        command: () => this.logout(),
        routerLink: ["/" + RouteUrl.login()]
      },
    ];
  }

  initAsManager() {
    this.items = [
      {
        label: 'Profile',
        icon: 'pi pi-fw pi-user',
        routerLink: ["/" + RouteUrl.mProfile()]
      },
      {
        label: "Dashboard",
        icon: "pi pi-fw pi-chart-bar",
        routerLink: ["/" + RouteUrl.mDashboard()]
      },
      {
        label: "User Mode",
        icon: "pi pi-fw pi-user",
        routerLink: ["/" + RouteUrl.parkMap()]
      },
      {
        label: 'Logout',
        icon: 'pi pi-fw pi-sign-out',
        command: () => this.logout(),
        routerLink: ["/" + RouteUrl.login()]
      },
    ]
  }

  logout() {
    this.store.dispatch(logout());
  }

}
