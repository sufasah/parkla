import { Message } from 'primeng/api';
import { Component, NgZone, OnDestroy, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { AuthService } from '@app/core/services/auth.service';
import { MessageService } from 'primeng/api';
import { Router } from '@angular/router';
import { RouteUrl } from '@app/core/utils/route';
import { HttpErrorResponse } from '@angular/common/http';
import { AppUser } from '@app/core/models/app-user';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit, OnDestroy {

  verifyUsername = "";
  verifyPassword = "";

  verification = false;
  registering = false;

  constructor(
    private authService: AuthService,
    private messageService: MessageService,
    private router: Router,
    private ngZone: NgZone,
  ) {

  }

  ngOnInit(): void {}

  onLogin(event: {successful: boolean, error: string | null}) {
    if(event.successful) {
      this.messageService.add({
        life:1500,
        severity:'success',
        summary: 'Login',
        detail: 'Logged in successfully',
        icon:"pi-lock-open",
        data: {
          navigate: true,
          navigateTo: RouteUrl.parkMap()
        }
      });
    }
    else {
      this.messageService.add({
        life:5000,
        severity:"error",
        summary: "Login",
        detail: event.error!,
        icon: "pi-lock",
      })
      this.verification = false;
      this.registering = false;
    }
  }

  register({form, user, password}: {form: NgForm, user: AppUser, password:string}) {
    if(this.registering) return;
    if(form.invalid){
      var keys = Object.keys(form.controls);
      keys.forEach(e => {
        form.controls[e].markAsDirty()
      });
      return;
    }

    console.log(user);


    this.registering = true;
    let username = user.username;

    this.authService
      .register({
        ...user,
      }, password)
      .subscribe({
        next: () => {
          this.messageService.add({
            life:1500,
            severity:'register',
            summary: 'Register',
            detail: 'User registered successfully',
            icon:"pi-user-plus",
            data: {
              verify: true,
              username: username,
              password: password
            },
          })
          this.registering = false;
        },
        error: (err:HttpErrorResponse) => {
          this.messageService.add({
            life:5000,
            severity:"register-error",
            summary: "Register",
            detail: err.error.message,
            icon: "pi-lock",
          })
          this.registering = false;
        },
      });
  }

  cancelVerify() {
    this.router.navigateByUrl(RouteUrl.login());
  }

  messageClose(message: Message) {
    if(message.data?.verify){
      this.verifyUsername = message.data?.username;
      this.verifyPassword = message.data?.password;
      this.verification = true;
    }
    if(message.data?.navigate) {
      this.ngZone.run(() => {
        this.router.navigateByUrl(message.data?.navigateTo);
      });
    }
  }

  ngOnDestroy(): void {
  }
}
