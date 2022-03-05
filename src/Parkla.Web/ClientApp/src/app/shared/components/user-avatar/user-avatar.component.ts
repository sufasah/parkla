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
        label: 'File',
        items: [
          {
            label: 'New',
            icon: 'pi pi-fw pi-plus',
            items: [
              {label: 'Project'},
              {label: 'Other'},
            ]
          },
          {label: 'Open'},
          {label: 'Quit'}
        ]
      },
      {
        label: 'Edit',
        icon: 'pi pi-fw pi-pencil',
        items: [
          {label: 'Delete', icon: 'pi pi-fw pi-trash'},
          {label: 'Refresh', icon: 'pi pi-fw pi-refresh'}
        ]
      }
    ];
  }

}
