import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { apiParkSpaces } from '../constants/http';
import { ParkSpace } from '../models/park-space';

@Injectable({
  providedIn: 'root'
})
export class ParkSpaceService {

  constructor(private httpClient: HttpClient) {

  }

  getParkSpaces() {
    return this.httpClient.get<ParkSpace[]>(apiParkSpaces + "/all");
  }

  getAreaParkSpaces(areaId: number, includeReservations: boolean = false) {
    return this.httpClient.get<ParkSpace[]>(apiParkSpaces + "/all", {
      params: {
        areaId,
        includeReservations
      }
    });
  }
}
