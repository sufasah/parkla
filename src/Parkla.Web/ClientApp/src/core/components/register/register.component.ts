import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {

  isManager = false;
  email = "";
  password = "";
  passwordAgain = "";
  name = "";
  surname = "";
  gender = new FormControl("");
  genderOptions = ['Male','Female'];
  countryCode = {flag:"",code:90};
  phone = null;
  address = "";
  city = {name:"Turkey",code:"TR"};
  district = "";
  zip = "";
  birthDate:Date | null = null;
  citySuggestions = [];
  districtSuggestions = [];
  countryCodeSuggestions = [];

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

  constructor() {

  }

  ngOnInit(): void {

  }

  register() {
    console.log("register clicked");

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
}
