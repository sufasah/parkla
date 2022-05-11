import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { apiParks } from '../constants/http';
import { Park } from '../models/park';

@Injectable({
  providedIn: 'root'
})
export class ParkService {

  constructor(private httpClient: HttpClient) { }

  addPark(park: Park) {
    return this.httpClient.post<Park>(apiParks, {
      name: park.name,
      location: park.location,
      latitude: park.latitude,
      longitude: park.longitude,
      extras: park.extras
    });
  }

  updatePark(park: Park) {
    return this.httpClient.post<Park>(apiParks, {
      id: park.id,
      name: park.name,
      location: park.location,
      latitude: park.latitude,
      longitude: park.longitude,
      extras: park.extras
    });
  }
}
