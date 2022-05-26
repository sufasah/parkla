import { HttpErrorResponse } from '@angular/common/http';
import { Component, NgZone, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { AppUser } from '@app/core/models/app-user';
import { AuthService } from '@app/core/services/auth.service';
import { UserService } from '@app/core/services/user.service';
import { RouteUrl } from '@app/core/utils/route';
import { Message, MessageService } from 'primeng/api';
import { catchError, throwError } from 'rxjs';

@Component({
  selector: 'app-m-profile',
  templateUrl: './m-profile.component.html',
  styleUrls: ['./m-profile.component.scss']
})
export class MProfileComponent implements OnInit {
  appUser!: AppUser;

  updating = false;

  password = "";
  passwordAgain = "";

  constructor(
    private authService: AuthService,
    private userService: UserService,
    private messageService: MessageService,
    private ngZone: NgZone,
    private router: Router) {

  }

  ngOnInit(): void {
    const token = this.authService.accessToken;
    const userId = Number(token?.sub);

    this.userService.getUser(userId).subscribe((user:AppUser) => {
      this.appUser = user;
    })
  }

  save({form, user}: {form: NgForm, user: AppUser}) {
    if(this.updating) return;
    if(form.invalid){
      var keys = Object.keys(form.controls);
      keys.forEach(e => {
        form.controls[e].markAsDirty()
      });
      return;
    }

    this.updating = true;

    this.userService.updateUser(user).subscribe({
      next: newUser => {
        this.appUser = newUser;

        this.messageService.add({
          life:1500,
          severity:'success',
          summary: 'Update User',
          detail: 'New user informations are saved',
          icon:"pi-save",
        })
        this.updating = false;
      },
      error: (err: HttpErrorResponse) => {
        this.messageService.add({
          life:5000,
          severity:"error",
          summary: "Save",
          detail: err.error,
          icon: "pi-lock",
        })
        this.updating = false;
        return throwError(() => err);
      }
    });
  }

  messageClose(message: Message) {
    if(message.data?.navigate){
      this.ngZone.run(() => {
        this.router.navigateByUrl(message.data.navigateTo);
      });
    }
  }

  goMap() {
    this.router.navigateByUrl(RouteUrl.mParkMap());
  }
}
