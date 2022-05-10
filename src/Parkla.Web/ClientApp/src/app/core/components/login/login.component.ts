import { HttpErrorResponse } from '@angular/common/http';
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
  tokenLoading:boolean = false;
  tokenLoadSuccess:boolean | null = null;
  tokenLoadFail:boolean | null = null;
  loginError:string | null = null;
  verification = false;
  verifCode = "";
  verifying = false;
  verifyUsername = "";
  verifyPassword = "";

  private authStateSubscription?: Subscription;

  constructor(
    private authService: AuthService,
    private store:Store,
    private messageService: MessageService,
    private router: Router) { }

  ngOnInit(): void {
    this.authStateSubscription = this.store.select(selectAuthState).subscribe(state => {
      this.tokenLoading = state.tokenLoading;
      this.tokenLoadSuccess = state.tokenLoadSuccess;
      this.tokenLoadFail = state.tokenLoadFail;
      this.loginError = state.loginError;

      if(this.tokenLoadSuccess) {
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
      else if(this.tokenLoadFail) {
        this.messageService.add({
          life:5000,
          severity:"error",
          summary: "Login",
          detail: this.loginError!,
          icon: "pi-lock",
        })
      }
    });

  }

  login(form:NgForm) {
    if(form.invalid){
      var keys = Object.keys(form.controls);
      keys.forEach(e => {
        form.controls[e].markAsDirty()
      });
      return;
    }

    this.authService.asManager = this.asManager;

    this.tokenLoading = true;

    this.authService.login(
      this.username,
      this.password
    ).subscribe({
      next: successful => {
        if(!successful) {
          this.verifyUsername = this.username;
          this.verifyPassword = this.password;
          this.verification = true;
        }
      }
    });
  }

  verify(form:NgForm) {
    if(form.invalid){
      var keys = Object.keys(form.controls);
      keys.forEach(e => {
        form.controls[e].markAsDirty()
      });
      return;
    }

    this.verifying = true;

    this.authService.verify(this.username, this.verifCode)
      .subscribe({
        next: () => {
          this.messageService.add({
            life:1500,
            severity:'login',
            summary: 'Verification',
            detail: 'Email is verified',
            icon:"pi-lock-open"
          });
          this.verifying = false;
          this.authService.login(this.verifyUsername, this.verifyPassword).subscribe();
        },
        error: (err: HttpErrorResponse) => {
          this.messageService.add({
            life:5000,
            severity:"error",
            summary: "Verification",
            detail: err.error.message,
            icon: "pi-lock",
          })
          this.verifying = false;
        }
      });
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
