import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, tap } from 'rxjs';
import { apiReservations } from '../constants/http';
import { Reservation } from '../models/reservation';

@Injectable({
  providedIn: 'root'
})
export class ReservationService {

  constructor(private httpClient: HttpClient) { }

  getUserReservations(userId: number) {
    return this.httpClient.get<Reservation[]>(`${apiReservations}/user/${userId}`)
      .pipe(tap(reservations => {
        reservations.forEach(res => {
          res.endTime = new Date(res.endTime);
          res.startTime = new Date(res.startTime);
        });
      }));
  }

  addReservation(reservation: Reservation) {
    return this.httpClient.post<Reservation>(apiReservations, reservation)
      .pipe(tap(reservation => {
        reservation.startTime = new Date(reservation.startTime);
        reservation.endTime = new Date(reservation.endTime);
      }));
  }

  deleteReservation(id: number) {
    return this.httpClient.delete(apiReservations, {
      body: {
        id
      }
    });
  }

}
