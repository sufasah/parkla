import { Component, OnInit } from '@angular/core';
import { FormControl, NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { AppUser } from '@app/core/models/app-user';
import { AuthService } from '@app/core/services/auth.service';
import { UserService } from '@app/core/services/user.service';
import { Message, MessageService } from 'primeng/api';
import { catchError, throwError } from 'rxjs';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {

  id!: number;
  wallet!: number;
  username = "";
  email = "";
  password = "";
  passwordAgain = "";
  name = "";
  surname = "";
  gender = new FormControl("");
  genderOptions = ['Male','Female'];
  countryCode = 90;
  phone:string | null = null;
  address = "";
  city = "Turkey";
  district = "";
  zip = "";
  birthdate:Date | null = null;
  citySuggestions = [];
  districtSuggestions = [];
  countryCodeSuggestions = [];
  submitted = false;

  updating = false;

  get birthDay(){
    return this.birthdate?.getDay();
  }

  get birthMonth(){
    return this.birthdate?.getMonth();
  }

  get birthYear(){
    return this.birthdate?.getFullYear();
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
      this.username,
      this.email,
      this.name,
      this.surname,
      this.phone!,
      this.city,
      this.district,
      this.gender.value,
      this.address,
      this.zip,
      this.birthdate
    );
  }

  constructor(
    private authService: AuthService,
    private userService: UserService,
    private messageService: MessageService,
    private router: Router) {

  }

  ngOnInit(): void {
    let token = this.authService.accessToken;
    this.username = token?.preferred_username!;
    this.email = token?.email!;
    let lastSpace = token?.name!.lastIndexOf(" ")!;
    this.name = token?.name?.substring(0,lastSpace)!;
    this.surname = token?.name?.substring(lastSpace+1)!;
    this.gender = new FormControl(token?.gender);

    console.log(token);


    this.userService.getUserDetail().subscribe((userDetail:any) => {
      this.id = userDetail.id;
      this.wallet = userDetail.wallet;
      this.birthdate = userDetail.birthdate;
      this.countryCode = userDetail.countryCode;
      this.phone = userDetail.phone;
      this.city = userDetail.city;
      this.district = userDetail.district;
      this.address = userDetail.address;
      this.zip = userDetail.zip;
    })
  }

  save(form:NgForm) {
    console.log(form);
    this.updating = true;

    this.userService.updateUser(this.id,{
      email: this.email,
      password: this.password,
      name: this.name,
      surname: this.surname,
      gender: this.gender,
      birthdate: this.birthdate,
      countryCode: this.countryCode,
      city: this.city,
      district: this.district,
      address: this.address,
      zip: this.zip
    })
    .pipe(catchError(err => {
      this.messageService.add({
        life:1500,
        severity:"error",
        summary: "Save",
        detail: "Values can't be saved successfully",
        icon: "pi-lock",
      })
      this.updating = false;
      return throwError(() => err);
    }))
    .subscribe(success => {
      if(success){
        this.messageService.add({
          life:1500,
          severity:'save',
          summary: 'Save',
          detail: 'New values are saved successfully',
          icon:"pi-save",
        })
      }
      else {
      }
      this.updating = false;
    });

    return;

    if(form.invalid){
      var keys = Object.keys(form.controls);
      keys.forEach(e => {
        form.controls[e].markAsDirty()
      });
      return;
    }

    this.updating = true;

    this.userService.updateUser(this.id,{
      email: this.email,
      password: this.password,
      name: this.name,
      surname: this.surname,
      gender: this.gender,
      birthdate: this.birthdate,
      countryCode: this.countryCode,
      city: this.city,
      district: this.district,
      address: this.address,
      zip: this.zip
    })
      .subscribe(success => {
        if(success){
          this.messageService.add({
            life:1500,
            severity:'save',
            summary: 'Save',
            detail: 'New values are saved successfully',
            icon:"pi-save",
          })
        }
        else {
          this.messageService.add({
            life:1500,
            severity:"error",
            summary: "Save",
            detail: "Values can't be saved successfully",
            icon: "pi-lock",
          })
        }
        this.updating = false;
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
  }

}