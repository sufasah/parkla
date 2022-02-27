import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { AuthService } from '@app/core/services/auth.service';
import { selectAuthState } from '@app/store/auth/auth.selectors';
import {  } from '@ngrx/store';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  email = "";
  password = "";

  constructor(
    private authService: AuthService) { }

  ngOnInit(): void {
    console.log(this.authService);
    this.authService.login("deneme","123");

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
  }
}
