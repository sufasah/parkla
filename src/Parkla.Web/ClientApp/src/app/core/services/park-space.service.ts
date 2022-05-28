import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs';
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
    })
    .pipe(
      map(spaces => {
        return spaces.map(x => {
          x.status = <any>x.status.toUpperCase();
          x.reservations.forEach(res => {
            res.startTime = new Date(res.startTime);
            res.endTime = new Date(res.endTime);
          });
          return x;
        }) ?? [];
      })
    );
  }
}
