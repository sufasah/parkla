import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { AuthService } from '@app/core/services/auth.service';
import { selectAuthState } from '@app/store/auth/auth.selectors';
import { Store } from '@ngrx/store';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  email = "";
  password = "";
  tokenLoading:boolean = false;
  tokenLoadSuccess:boolean | null= null;
  tokenLoadFail:boolean | null= null;
  loginError:string | null = null;

  constructor(
    private authService: AuthService,
    private store:Store) { }

  ngOnInit(): void {
    this.store.select(selectAuthState).subscribe(state => {
      this.tokenLoading = state.tokenLoading;
      this.tokenLoadSuccess = state.tokenLoadSuccess;
      this.tokenLoadFail = state.tokenLoadFail;
      this.loginError = state.loginError;
    });
  }

  login(form:NgForm) {
    console.log(form);

    if(form.invalid){
      var keys = Object.keys(form.controls);
      keys.forEach(e => {
        form.controls[e].markAsDirty()
      });
      return;
    }

    this.authService.login(this.email,this.password);
  }
}
