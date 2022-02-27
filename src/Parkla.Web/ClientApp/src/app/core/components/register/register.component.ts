import { Message } from 'primeng/api';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, NgForm } from '@angular/forms';
import { AuthService } from '@app/core/services/auth.service';
import { AppUser } from '@app/models/user';
import { MessageService } from 'primeng/api';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit, OnDestroy {

  isManager = false;
  email = "";
  password = "";
  passwordAgain = "";
  name = "";
  surname = "";
  gender = new FormControl("");
  genderOptions = ['Male','Female'];
  countryCode = {flag:"",code:90};
  phone:string | null = null;
  address = "";
  city = {name:"Turkey",code:"TR"};
  district = "";
  zip = "";
  birthDate:Date | null = null;
  citySuggestions = [];
  districtSuggestions = [];
  countryCodeSuggestions = [];
  submitted = false;

  registering = false;
  registerSubscription?:Subscription;

  get birthDay(){
    return this.birthDate?.getDay();
  }

  get birthMonth(){
    return this.birthDate?.getMonth();
  }

  get birthYear(){
    return this.birthDate?.getFullYear();
  }

  get maxBirthDate(){
    var date = new Date();
    date.setFullYear(date.getFullYear()-18);
    return date;
  }

  get minBirthDate(){
    return new Date(0);
  }

  get getUser() {
    return new AppUser(
      this.isManager,
      this.email,
      this.name,
      this.surname,
      this.phone!,
      this.city.name,
      this.district,
      this.gender.value,
      this.address,
      this.zip,
      this.birthDate
    );
  }

  constructor(
    private authService: AuthService,
    private messageService: MessageService,
    private router: Router) {

  }

  ngOnInit(): void {

  }

  register(form:NgForm) {
    console.log(form);
    this.registering = true;

    this.registerSubscription = this.authService
      .register(this.getUser,this.password)
      .subscribe(success => {
        if(success){
          this.messageService.add({
            life:1500,
            severity:'register',
            summary: 'Register',
            detail: 'User registered successfully',
            icon:"pi-user-plus",
            data: {
              navigate: true,
              navigateTo: "/test"
            }
          })
        }
        else {
          this.messageService.add({
            life:1500,
            severity:"register-error",
            summary: "Register",
            detail: "User not Registered",
            icon: "pi-lock",
          })
        }
        this.registering = false;
      });

    return;

    if(form.invalid){
      var keys = Object.keys(form.controls);
      keys.forEach(e => {
        form.controls[e].markAsDirty()
      });
      return;
    }

    this.registerSubscription = this.authService
      .register(this.getUser,this.password)
      .subscribe(success => {
        if(success){
          this.messageService.add({

          })
        }
      });
  }

  searchCity() {
  }

  searchDistrict() {
    this.districtSuggestions = <any>["a","b","c"];
  }

  searchCountryCode() {
    this.countryCodeSuggestions = <any>[{flag:"",code:"90"}]
  }

  genderClick(event:any){
    if(event.option === this.gender.value){
      this.gender.setValue("");
    }
  }

  setModel(pvm:any){
    return pvm;
  }

  messageClose(message: Message) {
    if(message.data?.navigate){
      this.router.navigate([message.data.navigateTo]);
    }
  }

  ngOnDestroy(): void {
    this.registerSubscription?.unsubscribe();
  }
}
