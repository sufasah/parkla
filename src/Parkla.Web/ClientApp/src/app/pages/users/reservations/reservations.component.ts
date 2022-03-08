import { Component, OnInit } from '@angular/core';
import { ParkSpanPerTime } from '@app/core/models/parking-lot';
import { Reservation } from '@app/core/models/reservation';
import { ReservationService } from '@app/core/services/reservation.service';
import { mockReservations } from '@app/mock-data/reservations';

@Component({
  selector: 'app-reservations',
  templateUrl: './reservations.component.html',
  styleUrls: ['./reservations.component.scss']
})
export class ReservationsComponent implements OnInit {

  reservations = mockReservations;

  constructor(reservationService: ReservationService) { }

  ngOnInit(): void {

  }

}
