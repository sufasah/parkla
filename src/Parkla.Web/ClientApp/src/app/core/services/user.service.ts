import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of, tap } from 'rxjs';
import { apiUrl, apiUsers } from '../constants/http';
import { AppUser } from '../models/app-user';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private httpClient:HttpClient) { }

  getUser(id: number) {
    return this.httpClient.get<AppUser>(apiUsers + `/${id}`);
  }

  updateUser(user: AppUser) {
    return this.httpClient.put<AppUser>(apiUsers, {
      ...user
    });
  }

  loadMoney(userId: number, amount: number) {
    return this.httpClient.put<AppUser>(apiUsers+"/load-money", {
      id: userId,
      wallet: amount
    });
  }
}
