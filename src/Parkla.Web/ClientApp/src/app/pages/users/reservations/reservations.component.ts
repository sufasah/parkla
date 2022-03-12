import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ParkSpanPerTime } from '@app/core/models/parking-lot';
import { Reservation } from '@app/core/models/reservation';
import { ReservationService } from '@app/core/services/reservation.service';
import { RouteUrl } from '@app/core/utils/route.util';
import { mockReservations } from '@app/mock-data/reservations';

@Component({
  selector: 'app-reservations',
  templateUrl: './reservations.component.html',
  styleUrls: ['./reservations.component.scss']
})
export class ReservationsComponent implements OnInit {

  reservations = mockReservations;

  constructor(
    private router:Router,
    private reservationService: ReservationService) { }

  ngOnInit(): void {

  }

  goMap() {
    this.router.navigate([RouteUrl.parkMap()]);
  }

}
