import { Component, OnInit } from '@angular/core';
import { ParkSpanPerTime } from '@app/core/models/parking-lot';
import { Reservation } from '@app/core/models/reservation';
import { ReservationService } from '@app/core/services/reservation.service';

@Component({
  selector: 'app-reservations',
  templateUrl: './reservations.component.html',
  styleUrls: ['./reservations.component.scss']
})
export class ReservationsComponent implements OnInit {

  reservations: Reservation[] = [{
    parkId: 1,
    parkName: "seapark",
    areaName: "basement5",
    pricing: <ParkSpanPerTime>{
      type: "truck",
      timeUnit: "minutes",
      price: {
        moneyUnit: "TL",
        amount: 50
      },
      timeAmount: 30
    },
    timeFrom: new Date(),
    timeTo: new Date(Date.now()+1000*60*30)
  }];

  constructor(reservationService: ReservationService) { }

  ngOnInit(): void {

  }

}
