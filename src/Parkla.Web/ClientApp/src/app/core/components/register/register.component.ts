import { Message } from 'primeng/api';
import { Component, NgZone, OnDestroy, OnInit } from '@angular/core';
import { FormControl, NgForm } from '@angular/forms';
import { AuthService } from '@app/core/services/auth.service';
import { MessageService } from 'primeng/api';
import { Router } from '@angular/router';
import { RouteUrl } from '@app/core/utils/route';
import { Gender } from '@app/core/enums/Gender';
import { capitalize } from '@app/core/utils/string';
import { City } from '@app/core/models/city';
import { District } from '@app/core/models/district';
import { HttpErrorResponse } from '@angular/common/http';
import { CityService } from '@app/core/services/city.service';
import { DistrictService } from '@app/core/services/district.service';
import { AppUser } from '@app/core/models/app-user';
import { Subscription } from 'rxjs';
import { Store } from '@ngrx/store';
import { selectAuthState } from '@app/store/auth/auth.selectors';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit, OnDestroy {

  username = "";
  email = "";
  password = "";
  passwordAgain = "";
  name = "";
  surname = "";
  gender = new FormControl("");
  genderOptions = [
    capitalize(Gender[Gender.MALE]),
    capitalize(Gender[Gender.FEMALE])
  ];
  phone: string | null = null;
  address = "";
  birthdate: string | null = null;

  city: City | null = null;
  citySuggestions: City[] = [];
  cityEmptyMessage = "";

  district: District | null = null;
  districtSuggestions: District[] = [];
  districtEmptyMessage = "";

  submitted = false;
  registering = false;

  verifyUsername = "";
  verifyPassword = "";
  verification = false;

  get maxBirthDate(){
    let date = new Date();
    return date.setFullYear(date.getFullYear()-18);
  }

  get minBirthDate(){
    return `01.01.${new Date().getFullYear()-99}`;
  }

  constructor(
    private authService: AuthService,
    private messageService: MessageService,
    private router: Router,
    private cityService: CityService,
    private ngZone: NgZone,
    private districtService: DistrictService
  ) {

  }

  ngOnInit(): void {
  }

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

  register(form:NgForm) {
    if(this.registering) return;
    if(form.invalid){
      var keys = Object.keys(form.controls);
      keys.forEach(e => {
        form.controls[e].markAsDirty()
      });
      return;
    }

    this.registering = true;
    let username = this.username;
    let password = this.password;

    console.log(username,password);


    this.authService
      .register(<AppUser>{
        username: username,
        email: this.email,
        name: this.name,
        surname: this.surname,
        phone: this.phone!,
        city: this.city,
        district: this.district,
        gender: this.gender.value ? Gender[this.gender.value.toUpperCase()] : null,
        address: this.address ? this.address : null ,
        birthdate: this.birthdate ? this.birthdate : null,
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

  searchCity(evt: any) {
    this.cityService.search(evt.query).subscribe({
      next: cities => {
        this.citySuggestions = cities;
        this.cityEmptyMessage = "No city found"
      },
      error: (err: HttpErrorResponse) => {
        this.citySuggestions = [];
        this.cityEmptyMessage = err.error.message;
      }
    });
  }

  searchDistrict(evt: any) {
    this.districtService.search(evt.query).subscribe({
      next: districts => {
        this.districtSuggestions = districts;
        this.districtEmptyMessage = "No city found"
      },
      error: (err: HttpErrorResponse) => {
        this.districtSuggestions = [];
        this.districtEmptyMessage = err.error.message;
      }
    });
  }

  genderClick(event:any){
    if(event.option === this.gender.value){
      this.gender.setValue("");
    }
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
