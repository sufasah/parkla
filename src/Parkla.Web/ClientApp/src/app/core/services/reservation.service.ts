import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs';
import { apiReservations } from '../constants/http';
import { Reservation } from '../models/reservation';

@Injectable({
  providedIn: 'root'
})
export class ReservationService {

  constructor(private httpClient: HttpClient) { }

  addReservation(reservation: Reservation) {
    return this.httpClient.post<Reservation>(apiReservations, reservation)
      .pipe(map(reservation => {
        reservation.startTime = new Date(reservation.startTime);
        reservation.endTime = new Date(reservation.endTime);
        return reservation;
      }));
  }

}
