import { Component, OnDestroy, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '@app/core/services/auth.service';
import { RouteUrl } from '@app/core/utils/route';
import { selectAuthState } from '@app/store/auth/auth.selectors';
import { Store } from '@ngrx/store';
import { Message, MessageService } from 'primeng/api';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit, OnDestroy {

  username = "";
  password = "";
  asManager = false;
  logging = false;

  verifyUsername = "";
  verifyPassword = "";
  verification = false;

  private authStateSubscription?: Subscription;

  constructor(
    private authService: AuthService,
    private store:Store,
    private messageService: MessageService,
    private router: Router) { }

  ngOnInit(): void {
  }

  onLogin(event: {successful: boolean, error: string | null}) {
    if(event.successful) {
      this.messageService.add({
        life:1500,
        severity:'login',
        summary: 'Login',
        detail: 'Logged in successfully',
        icon:"pi-lock-open",
        data: {
          navigate: true,
          navigateTo: this.asManager
            ? RouteUrl.mParkMap()
            : RouteUrl.parkMap()
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
      this.logging = false;
    }
  }

  login(form:NgForm) {
    if(this.logging) return;
    if(form.invalid){
      var keys = Object.keys(form.controls);
      keys.forEach(e => {
        form.controls[e].markAsDirty()
      });
      return;
    }

    this.authService.asManager = this.asManager;
    this.logging = true;

    let username = this.username;
    let password = this.password;

    this.authService.login(
      username,
      password
    ).subscribe({
      next: successful => {
        if(!successful) {
          this.verifyUsername = username;
          this.verifyPassword = password;
          this.verification = true;
        }
        else {
          this.onLogin({successful: true, error: null});
        }
      }
    });
  }

  cancelVerify() {
    this.username = "";
    this.password = "";
    this.asManager = false;
    this.logging = false;
    this.verifyUsername = "";
    this.verifyPassword = "";
    this.verification = false;
  }

  messageClose(message: Message) {
    if(message.data?.navigate){
      this.router.navigateByUrl(message.data.navigateTo);
    }
  }

  ngOnDestroy(): void {
    this.authStateSubscription?.unsubscribe();
  }
}
