import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ReservationService } from '@app/core/services/reservation.service';

@Component({
  selector: 'app-reservations',
  templateUrl: './reservations.component.html',
  styleUrls: ['./reservations.component.scss']
})
export class ReservationsComponent implements OnInit {

  reservations = [];

  constructor(
    private router:Router,
    private reservationService: ReservationService) { }

  ngOnInit(): void {

  }

  goMap() {
    this.router.navigate([".."]);
  }

}
