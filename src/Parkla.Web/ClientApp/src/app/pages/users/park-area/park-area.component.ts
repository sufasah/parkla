import { DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, EventEmitter, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ParkArea } from '@app/core/models/park-area';
import { ParkSpace } from '@app/core/models/park-space';
import { getPricePerHour } from '@app/core/models/pricing';
import { Reservation } from '@app/core/models/reservation';
import { AuthService } from '@app/core/services/auth.service';
import { ParkAreaService } from '@app/core/services/park-area.service';
import { ParkSpaceService } from '@app/core/services/park-space.service';
import { ReservationService } from '@app/core/services/reservation.service';
import { RouteUrl } from '@app/core/utils/route';
import { ParkTemplateComponent } from '@app/shared/components/area-template/area-template.component';
import * as moment from 'moment';
import { ConfirmationService, MenuItem, MessageService } from 'primeng/api';

@Component({
  selector: 'app-park',
  templateUrl: './park-area.component.html',
  styleUrls: ['./park-area.component.scss']
})
export class ParkAreaComponent implements OnInit {
  @ViewChild("parkTemplate")
  parkTemlate?: ParkTemplateComponent;

  selectedArea: ParkArea = <any>{
    id: this.getAreaId(),
    spaces: []
  };;

  selectedSpace?: ParkSpace;

  showReservationModal = new EventEmitter<boolean>(true);

  minDate = new Date();

  maxDate = new Date(Date.now()+1000*60*60*24*6);

  selectedTime: [Date?, Date?] = [undefined, undefined];

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService,
    private areaService: ParkAreaService,
    private spaceService: ParkSpaceService,
    private reservationService: ReservationService,
    private confirmationService: ConfirmationService,
    private datePipe: DatePipe,
    private authService: AuthService) { }


  ngOnInit(): void {
    this.getData();
  }

  getData() {
    this.areaService.getArea(this.getAreaId()).subscribe({
      next: area => {
        Object.assign(this.selectedArea, area);
        this.getAreaSpaces();
      },
      error: (err: HttpErrorResponse) => {
        this.messageService.add({
          life:5000,
          severity:"error",
          summary: "Fetch Park Area",
          detail: err.error.message,
          icon: "pi-lock"
        });
      }
    });
  }

  getAreaSpaces() {
    this.spaceService.getAreaParkSpaces(this.getAreaId(), true).subscribe({
      next: spaces => {
        this.selectedArea = {...this.selectedArea, spaces: spaces};
        this.timeRangeChange(this.selectedTime);
      },
      error: (err: HttpErrorResponse) => {
        this.messageService.add({
          life:5000,
          severity:"error",
          summary: "Fetch Park Spaces",
          detail: err.error.message,
          icon: "pi-lock"
        });
      }
    });
  }

  getAreaId() {
    return Number(this.route.snapshot.paramMap.get("areaid"));
  }

  findSpaceReserved(space: ParkSpace, timeRange: [Date?, Date?]) {
    for(let i=0; i<space.reservations.length; i++) {
      let reservation = space.reservations[i];
      space.isReserved = this.isTimeRangesIntercept(
        reservation.startTime,
        reservation.endTime,
        timeRange[0]!,
        timeRange[1]!
      );

      if(space.isReserved) break;
    }
  }

  timeRangeChange(timeRange: [Date?, Date?]) {
    this.selectedArea.spaces.forEach(space => {
      if(this.selectedArea.reservationsEnabled && space.reservations) {
        this.findSpaceReserved(space, timeRange);
      }
    });

    this.selectedTime = timeRange
    this.parkTemlate?.drawCanvas();
  }

  spaceClicked(space: ParkSpace) {
    this.selectedSpace = space;

    if(!this.selectedSpace)
      this.showReservationModal.emit(false);
    else if(this.selectedArea.reservationsEnabled)
      this.showReservationModal.emit(true);
  }

  reservationChanges(reservation: Reservation, isDelete: boolean) {
    var isIntercept = this.isTimeRangesIntercept(this.selectedTime[0]!, this.selectedTime[1]!, reservation.startTime, reservation.endTime);

    if(isDelete) {
      if(isIntercept) {
        reservation.space.isReserved = false;
        this.findSpaceReserved(reservation.space, this.selectedTime);
        this.parkTemlate?.drawCanvas();
      }
    }
    else {
      if(isIntercept) {
        reservation.space.isReserved = true;
        this.parkTemlate?.drawCanvas();
      }
    }
  }

  reserveSpace(space: ParkSpace) {
    if(!this.selectedTime || !this.selectedTime[0] || !this.selectedTime[1]) {
      this.messageService.add({
        summary: "Reservation",
        closable: true,
        severity: "error",
        life:5000,
        detail: "Invalid Time Range. Select a time interval."
      });
      return;
    }

    const pricePerHour = getPricePerHour(space.pricing);
    const timeRange: [Date, Date] = [this.selectedTime[0], this.selectedTime[1]];
    const hourDiff = (timeRange[1].getTime()-timeRange[0].getTime()) / 36e5;
    const payment = pricePerHour * hourDiff;

    this.confirmationService.confirm({
      message: `Are you sure to reserve the space with '${space.name}' name for ${payment} TL from ${this.datePipe.transform(timeRange[0], "MM-dd HH:mm")} to ${this.datePipe.transform(timeRange[1], "MM-dd HH:mm")} ?`,
      accept: () => {
        this.addReservation(space, timeRange);
      }
    });
  }

  addReservation(space: ParkSpace, timeRange: [Date, Date]) {
    this.reservationService.addReservation({
      id: <any>undefined,
      startTime: timeRange[0],
      endTime: timeRange[1]!,
      spaceId: space.id,
      space: <any>undefined,
      userId: Number(this.authService.accessToken!.sub!),
      user: <any>undefined
    }).subscribe({
      next: reservation => {
        this.showReservationModal.emit(false);
        this.messageService.add({
          life:5000,
          severity:"success",
          summary: "Reservation",
          detail: "Space is reserved successfully",
          icon: "pi-lock"
        });
      },
      error: (err: HttpErrorResponse) => {
        this.messageService.add({
          life:5000,
          severity:"error",
          summary: "Reservation",
          detail: err.error.message,
          icon: "pi-lock"
        });
      }
    });
  }

  goAreas() {
    let parkid = this.route.snapshot.paramMap.get("parkid")!;

    this.router.navigateByUrl(this.authService.asManager
      ? RouteUrl.mParkAreas(parkid)
      : RouteUrl.parkAreas(parkid)
    );
  }

  isTimeRangesIntercept(
    start1: Date,
    end1: Date,
    start2: Date,
    end2: Date
  ) {
    return (start1 >= start2 && start1 <= end2) ||
      (end1 >= start2 && end1 <= end2) ||
      (start1 <= start2 && end1 >= end2);
  }
}
