import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '@app/core/services/auth.service';
import { UserService } from '@app/core/services/user.service';
import { RouteUrl } from '@app/core/utils/route';
import { MenuItem, MessageService } from 'primeng/api';

@Component({
  selector: 'app-load-money',
  templateUrl: './load-money.component.html',
  styleUrls: ['./load-money.component.scss']
})
export class LoadMoneyComponent implements OnInit {

  cardNumber: string = "";
  expireMonth: number | null = null;
  expireYear: number | null = null;
  nameOnCard: string = "";
  cvc: number | null = null;
  amount: number = 0;

  loading = false;

  bcModel: MenuItem[] = [
    {icon: 'pi pi-map', routerLink: "/"+RouteUrl.parkMap()},
    {label: `Profile`, routerLink: "/"+RouteUrl.profile()},
    {label: `Load Money`, styleClass: "last-item"},
  ];

  constructor(
    private router: Router,
    private userService: UserService,
    private authService: AuthService,
    private messageService: MessageService
  ) { }

  ngOnInit(): void {
  }

  goProfile() {
    this.router.navigateByUrl(RouteUrl.profile());
  }

  pay(form: NgForm) {
    if(this.loading) return;
    if(form.invalid){
      var keys = Object.keys(form.controls);
      keys.forEach(e => {
        form.controls[e].markAsDirty()
      });
      return;
    }

    this.loading = true;
    const userId = Number(this.authService.accessToken?.sub);


    this.userService.loadMoney(userId, this.amount).subscribe({
      next: user => {
        this.messageService.add({
          summary: "Load Money",
          closable: true,
          severity: "success",
          life:1500,
          detail: "Money is loaded successfully"
        });
      },
      error: (err:HttpErrorResponse) => {
        this.messageService.add({
          summary: "Load Money",
          closable: true,
          severity: "error",
          life:5000,
          detail: err.error.message
        });
      }
    });
  }

  messageClose() {
    this.router.navigateByUrl(RouteUrl.profile());
  }
}
