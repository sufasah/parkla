import { Component, NgZone, OnInit } from '@angular/core';
import { FormControl, NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { AppUser } from '@app/core/models/app-user';
import { City } from '@app/core/models/city';
import { District } from '@app/core/models/district';
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
  city = <City>{id: 1, name: "ist"};
  district = <District>{id: 2, city: {id:5, name:"ank"}, name: "avcilar"};
  zip = "";
  birthdate: string | null = null;
  citySuggestions = [];
  districtSuggestions = [];
  countryCodeSuggestions = [];

  submitted = false;
  updating = false;

  get maxBirthDate(){
    var date = new Date();
    date.setFullYear(date.getFullYear()-18);
    return date;
  }

  get minBirthDate(){
    return new Date(0);
  }

  get getUser() {
    return <AppUser>{
      username: this.username,
      email: this.email,
      name: this.name,
      surname: this.surname,
      phone: this.phone!,
      city: this.city,
      district: this.district,
      gender: this.gender.value,
      address: this.address,
      birthdate: this.birthdate,
    };
  }

  constructor(
    private authService: AuthService,
    private userService: UserService,
    private messageService: MessageService,
    private ngZone: NgZone,
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
        life:5000,
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
            life:5000,
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
      this.ngZone.run(() => {
        this.router.navigateByUrl(message.data.navigateTo);
      });
    }
  }

  goMap() {
    this.router.navigateByUrl(RouteUrl.mParkMap());
  }
}
