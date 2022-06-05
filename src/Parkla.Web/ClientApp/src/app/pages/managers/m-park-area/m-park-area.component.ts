import { HttpErrorResponse } from '@angular/common/http';
import { Component, EventEmitter, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SpaceStatus } from '@app/core/enums/SpaceStatus';
import { ParkArea } from '@app/core/models/park-area';
import { ParkSpace } from '@app/core/models/park-space';
import { Reservation } from '@app/core/models/reservation';
import { AuthService } from '@app/core/services/auth.service';
import { ParkAreaService } from '@app/core/services/park-area.service';
import { ParkSpaceService } from '@app/core/services/park-space.service';
import { RouteUrl } from '@app/core/utils/route';
import { ParkTemplateComponent } from '@app/shared/components/area-template/area-template.component';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-m-park-area',
  templateUrl: './m-park-area.component.html',
  styleUrls: ['./m-park-area.component.scss']
})
export class MParkAreaComponent implements OnInit {

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
    private authService: AuthService,
    private areaService: ParkAreaService,
    private spaceService: ParkSpaceService,
    private messageService: MessageService
  ) {

  }

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

  spaceClicked(space: ParkSpace) {
    this.selectedSpace = space;

    if(!this.selectedSpace)
      this.showReservationModal.emit(false)
    else if(this.selectedArea.reservationsEnabled)
      this.showReservationModal.emit(true);

    this.showReservationModal.emit(false)
  }

  reservationChanges(reservation: Reservation, isDelete: boolean) {
    if(isDelete) {}
    else {}
  }

  getParkId() {
    return this.route.snapshot.paramMap.get("parkid")!;
  }


  goAreas() {
    let parkid = this.getParkId();
    this.router.navigateByUrl(this.authService.asManager
      ? RouteUrl.mParkAreas(parkid)
      : RouteUrl.parkAreas(parkid)
    );
  }

  goEditTemplate() {
    const parkid = this.getParkId();
    const areaid = this.getAreaId();

    this.router.navigateByUrl(RouteUrl.mEditParkAreaTemplate(parkid, areaid));
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
