import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ReservationService } from '@app/core/services/reservation.service';
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
    this.router.navigate([".."]);
  }

}
