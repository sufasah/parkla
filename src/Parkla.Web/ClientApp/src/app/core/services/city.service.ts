import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { apiCities } from '../constants/http';
import { City } from '../models/city';

@Injectable({
  providedIn: 'root'
})
export class CityService {

  constructor(private httpClient: HttpClient) { }

  search(search: string) {
    return this.httpClient.get<City[]>(apiCities+"/search", {
      params: {
        s: search
      }
    });
  }
}
