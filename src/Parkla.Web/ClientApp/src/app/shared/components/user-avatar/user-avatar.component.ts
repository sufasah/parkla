import { Component, OnInit } from '@angular/core';
import { AuthService } from '@app/core/services/auth.service';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-user-avatar',
  templateUrl: './user-avatar.component.html',
  styleUrls: ['./user-avatar.component.scss']
})
export class UserAvatarComponent implements OnInit {

  label: string = "";

  items: MenuItem[] = [];

  constructor(private authService: AuthService) { }

  ngOnInit(): void {
    this.label = this.authService.accessToken?.preferred_username![0]!;

    this.items = [
      {
        label: 'Profile',
        icon: 'pi pi-fw pi-user',
        routerLink: [
          "/profile"
        ]
      },
      {
        label: 'Reservations',
        icon: 'pi pi-fw pi-car',
        routerLink: [
          "/reservations"
        ]
      },
      {
        label: 'Logout',
        icon: 'pi pi-fw pi-sign-out',
        command: this.logout,
        routerLink: [
          "/"
        ]
      }
    ];
  }

  logout() {
    this.authService.logout();
  }

}
