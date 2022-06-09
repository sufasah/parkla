import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { apiDistricts } from '../constants/http';
import { District } from '../models/district';

@Injectable({
  providedIn: 'root'
})
export class DistrictService {

  constructor(private httpClient: HttpClient) { }

  search(cityId: number, search: string) {
    return this.httpClient.get<District[]>(apiDistricts+"/search", {
      params: {
        cityId,
        s: search
      }
    });
  }
}
