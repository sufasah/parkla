import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of, tap } from 'rxjs';
import { apiUrl } from '../constants/http.const';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private httpClient:HttpClient) { }

  getUserDetail() {
    return of({
      id: 1,
      wallet: 25,
      birthdate: new Date(),
      countryCode: 90,
      phone: "555 555 55 55",
      city: "Istanbul",
      district: "Avcılar",
      address: "xxxx,yyyy,zzz",
      zip: undefined
    });
    return this.httpClient.get(`${apiUrl}/user/detail`);
  }

  updateUser(id:number,user:any) {
    return of({
      id: id,
      email:"newemail",
      name:"newname",
      surname:"newsurname",
      phone:"newphone",
      city:"newcity",
      district:"newdistrict",
      gender:"Female",
      address:"newaddress",
      zip:"newzip",
      birthdate: new Date(0)
    })
    //.pipe(tap(() => {throw new Error("err")}));
    return this.httpClient.put(`${apiUrl}/user/${id}`,user)
  }
}
