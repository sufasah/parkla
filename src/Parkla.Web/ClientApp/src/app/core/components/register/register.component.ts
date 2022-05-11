import { Message } from 'primeng/api';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, NgForm } from '@angular/forms';
import { AuthService } from '@app/core/services/auth.service';
import { MessageService } from 'primeng/api';
import { Router } from '@angular/router';
import { RouteUrl } from '@app/core/utils/route';
import { Gender } from '@app/core/enums/Gender';
import { capitalize } from '@app/core/utils/string';
import { City } from '@app/core/models/city';
import { District } from '@app/core/models/district';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { apiCities, apiDistricts, apiLogin } from '@app/core/constants/http';

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

  city?: City;
  citySuggestions: City[] = [];
  cityEmptyMessage = "";

  district?: District;
  districtSuggestions: District[] = [];
  districtEmptyMessage = "";

  submitted = false;
  registering = false;

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
    private httpClient: HttpClient
  ) {

  }

  ngOnInit(): void {

  }

  register(form:NgForm) {
    if(form.invalid){
      var keys = Object.keys(form.controls);
      keys.forEach(e => {
        form.controls[e].markAsDirty()
      });
      return;
    }

    this.registering = true;
    this.authService
      .register({
        username: this.username,
        password: this.password,
        email: this.email,
        name: this.name,
        surname: this.surname,
        phone: this.phone!,
        cityId: this.city ? this.city.id : null,
        districtId: this.district ? this.district.id : null,
        gender: this.gender.value ? this.gender.value : null,
        address: this.address ? this.address : null ,
        birthdate: this.birthdate ? this.birthdate : null,
      })
      .subscribe({
        next: () => {
          this.messageService.add({
            life:1500,
            severity:'register',
            summary: 'Register',
            detail: 'User registered successfully',
            icon:"pi-user-plus",
            data: {
              navigate: true,
              navigateTo: RouteUrl.parkMap()
            }
          })
          this.registering = false;
        },
        error: (err:HttpErrorResponse) => {
          console.log(err);
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

  searchCity(evt: any) {
    this.httpClient.get<City[]>(apiCities+"/search", {
      params: {
        s: evt.query
      }
    }).subscribe({
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
    this.httpClient.get<District[]>(apiDistricts+"/search", {
      params: {
        s: evt.query
      }
    }).subscribe({
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

  setModel(pvm:any){
    return pvm;
  }

  messageClose(message: Message) {
    if(message.data?.navigate){
      this.router.navigateByUrl(message.data.navigateTo);
    }
  }

  ngOnDestroy(): void {
  }
}
