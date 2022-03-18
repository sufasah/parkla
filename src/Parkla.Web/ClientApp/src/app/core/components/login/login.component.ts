import { Component, OnDestroy, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '@app/core/services/auth.service';
import { UserService } from '@app/core/services/user.service';
import { RouteUrl } from '@app/core/utils/route.util';
import { login } from '@app/store/auth/auth.actions';
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

  email = "";
  password = "";
  asManager = false;
  tokenLoading:boolean = false;
  tokenLoadSuccess:boolean | null = null;
  tokenLoadFail:boolean | null = null;
  loginError:string | null = null;

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
          life:1500,
          severity:"error",
          summary: "Login",
          detail: this.loginError!,
          icon: "pi-lock",
        })
      }
    });

  }

  login(form:NgForm) {
    console.log(form);

    this.authService.asManager = this.asManager;

    this.store.dispatch(login({
      email: this.email,
      password: this.password
    }));


    return;

    if(form.invalid){
      var keys = Object.keys(form.controls);
      keys.forEach(e => {
        form.controls[e].markAsDirty()
      });
      return;
    }

    this.authService.asManager = this.asManager;

    this.store.dispatch(login({
      email:this.email,
      password: this.password
    }));
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
