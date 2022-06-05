import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { Reservation } from '@app/core/models/reservation';
import { AuthService } from '@app/core/services/auth.service';
import { ReservationService } from '@app/core/services/reservation.service';
import { MessageService } from 'primeng/api';
import { Table } from 'primeng/table';

@Component({
  selector: 'app-reservations',
  templateUrl: './reservations.component.html',
  styleUrls: ['./reservations.component.scss']
})
export class ReservationsComponent implements OnInit {

  @ViewChild(Table)
  table!: Table;

  reservations: Reservation[] = [];

  constructor(
    private router:Router,
    private reservationService: ReservationService,
    private authService: AuthService,
    private messageService: MessageService
  ) { }

  ngOnInit(): void {
    this.reservationService.getUserReservations(Number(this.authService.accessToken?.sub)).subscribe(reservations => {
      this.reservations = reservations;
    });
  }

  goMap() {
    this.router.navigate([".."]);
  }

  deleteReservation(reservation: Reservation) {
    this.reservationService.deleteReservation(reservation.id).subscribe({
      next: () => {
        this.messageService.add({
          life:1500,
          severity:'success',
          summary: 'Reservation Deletion',
          detail: 'Reservation is deleted',
          icon:"pi-save",
        });
        var resIndex = this.reservations.indexOf(reservation);
        this.reservations = [
          ...this.reservations.slice(0, resIndex),
          ...this.reservations.slice(resIndex+1, this.reservations.length)
        ];
      },
      error: (err: HttpErrorResponse) => {
        this.messageService.add({
          life:5000,
          severity:'error',
          summary: 'Reservation Deletion',
          detail: err.error.message,
          icon:"pi-save",
        });
      }
    });
  }

}
