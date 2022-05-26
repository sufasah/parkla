import { HttpErrorResponse } from '@angular/common/http';
import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Gender } from '@app/core/enums/Gender';
import { AppUser } from '@app/core/models/app-user';
import { City } from '@app/core/models/city';
import { District } from '@app/core/models/district';
import { CityService } from '@app/core/services/city.service';
import { DistrictService } from '@app/core/services/district.service';
import { SelectButton } from 'primeng/selectbutton';

@Component({
  selector: 'app-user-form',
  templateUrl: './user-form.component.html',
  styleUrls: ['./user-form.component.scss']
})
export class UserFormComponent implements OnInit {

  @ViewChild(SelectButton)
  selectButtonElement!: SelectButton;

  @Output()
  formSubmit = new EventEmitter<{form: NgForm, user:AppUser, password: string}>();

  @Input()
  isEdit: boolean = false;

  @Input()
  loading: boolean = false;

  @Input()
  submitLabel = "Submit";

  @Input()
  set appUser(value: AppUser) {
    if(value)
      this.user = value;
  }

  id: number | null = null;
  xmin: number = 0;
  wallet: number = 0;
  username = "";
  email = "";
  password = "";
  passwordAgain = "";
  name = "";
  surname = "";

  gender: Gender = <any>null;

  genderOptions: any[] = [
    "MALE",
    "FEMALE"
  ];
  phone: string | null = null;
  address = "";
  birthdate: Date | null = null;

  city: City | null = null;
  citySuggestions: City[] = [];
  cityEmptyMessage = "";

  district: District | null = null;
  districtSuggestions: District[] = [];
  districtEmptyMessage = "";

  get maxBirthDate(){
    let date = new Date();
    return date.setFullYear(date.getFullYear()-18);
  }

  get minBirthDate(){
    return `01.01.${new Date().getFullYear()-99}`;
  }

  get user() {
    return <AppUser>{
      id: this.id,
      wallet: this.wallet,
      username: this.username,
      email: this.email,
      name: this.name,
      surname: this.surname,
      phone: this.phone!,
      cityId: this.city?.id ?? null,
      districtId: this.district?.id ?? null,
      gender: this.gender,
      address: this.address,
      birthdate: this.birthdate?.toISOString(),
      xmin: this.xmin
    };
  }

  set user(value: AppUser) {
    this.id = value.id;
    this.wallet = value.wallet;
    this.username = value.username;
    this.email = value.email;
    this.name = value.name;
    this.surname = value.surname;
    this.phone! = value.phone;
    this.city = value.city;
    this.district = value.district;
    this.gender = value.gender!;
    this.address = value.address ?? "";
    this.birthdate = value.birthdate ? new Date(value.birthdate) : null;
    this.xmin = value.xmin;
  }

  constructor(
    private cityService: CityService,
    private districtService: DistrictService
  ) {

  }

  ngOnInit(): void {
  }

  submit(form: NgForm) {
    this.formSubmit.emit({form: form, user: this.user, password: this.password});
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

  ngOnDestroy(): void {

  }

}
