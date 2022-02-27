import { Component, OnDestroy, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '@app/core/services/auth.service';
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
  tokenLoading:boolean = false;
  tokenLoadSuccess:boolean | null= null;
  tokenLoadFail:boolean | null= null;
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
            navigateTo: "/test"
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

    this.authService.login(this.email,this.password);
    return;

    if(form.invalid){
      var keys = Object.keys(form.controls);
      keys.forEach(e => {
        form.controls[e].markAsDirty()
      });
      return;
    }

    this.authService.login(this.email,this.password);
  }

  messageClose(message: Message) {
    if(message.data?.navigate){
      this.router.navigate([message.data.navigateTo]);
    }
  }

  ngOnDestroy(): void {
    this.authStateSubscription?.unsubscribe();
  }
}
