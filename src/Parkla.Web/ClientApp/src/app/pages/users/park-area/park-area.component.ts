import { HttpErrorResponse } from '@angular/common/http';
import { Component, EventEmitter, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DAY, HOUR, MINUTE, SECOND } from '@app/core/constants/time';
import { SpaceStatus } from '@app/core/enums/SpaceStatus';
import { ParkArea } from '@app/core/models/park-area';
import { ParkSpace } from '@app/core/models/park-space';
import { SpaceReservation } from '@app/core/models/space-reservation';
import { AuthService } from '@app/core/services/auth.service';
import { ParkAreaService } from '@app/core/services/park-area.service';
import { ParkSpaceService } from '@app/core/services/park-space.service';
import { RouteUrl } from '@app/core/utils/route';
import { ParkTemplateComponent } from '@app/shared/components/area-template/area-template.component';
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

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService,
    private areaService: ParkAreaService,
    private spaceService: ParkSpaceService,
    private confirmationService: ConfirmationService,
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
        this.selectedArea = {...this.selectedArea, spaces: spaces.map(x => {
          x.status = <any>x.status.toUpperCase();
          return x;
        }) ?? []};
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

  timeRangeChange(timeRange: [Date?, Date?]) {
    this.selectedArea.spaces.forEach(space => {
      if(this.selectedArea.reservationsEnabled && space.reservations) {
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
    })

    this.parkTemlate?.drawCanvas();
  }

  spaceClicked(space: ParkSpace) {
    this.selectedSpace = space;

    if(!this.selectedSpace)
      this.showReservationModal.emit(false)

    if(this.selectedArea.reservationsEnabled)
      this.showReservationModal.emit(true);
  }

  reserveSpace() {
    this.confirmationService.confirm({
      message: 'Are you sure to reserve xxx number park space for xxx TL money from 19:00 12-12-1920 to 14:14 12-12-1921 ?',
      accept: () => {
        this.messageService.add({
          summary: "Reservation",
          closable: true,
          severity: "success",
          life:1500,
          detail: "The xxx number park reserved"
        })
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
