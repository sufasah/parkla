import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, ViewChild } from '@angular/core';
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
  SpaceStatusEnum = SpaceStatus;

  @ViewChild("parkTemplate")
  parkTemlate?: ParkTemplateComponent;

  dayNames = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

  selectedArea: ParkArea = <any>{
    id: this.getAreaId(),
    spaces: []
  };;

  selectedSpace?: ParkSpace;

  dialogVisible = false;

  minDate = new Date();

  maxDate = new Date(Date.now()+1000*60*60*24*6);

  _weekDays: MenuItem[] = [{label:"x"}];
  get weekDays() {
    const nowBegin = new Date();
    const dayCode = nowBegin.getDay();
    nowBegin.setHours(0,0,0)

    if(this._weekDays[0].label != this.dayNames[dayCode]) {
      this._weekDays = [];
      for(let i=0; i<7; i++) {
        const dayIndex = (dayCode+i)%7;
        const dateBeginTimestamp = nowBegin.getTime() + i*DAY;
        const dateEndTimestamp = dateBeginTimestamp
          + 23*HOUR
          + 59*MINUTE
          + 59*SECOND;

        this._weekDays.push({
          label: this.dayNames[dayIndex],
          command: (e) => this.dayTabSelected(e),
          state: {
            dateBegin: new Date(dateBeginTimestamp),
            dateEnd: new Date(dateEndTimestamp)
          }
        });
      }
    }

    return this._weekDays;
  }

  reservationsOfDay: SpaceReservation[] & {isReserved: boolean}[] = [];

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
        this.selectedArea = {...this.selectedArea, spaces: spaces ?? []};
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

    if(this.selectedArea.reservationsEnabled)
        this.showReserveModal();
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

  showReserveModal() {
    this.generateSpaceReservationTable(this.weekDays[0]);

    const now = new Date();
    if(this.selectedSpace!.status == SpaceStatus.OCCUPIED && this.reservationsOfDay[0].startTime <= now && this.reservationsOfDay[0].endTime >= now)
      this.reservationsOfDay[0].isReserved = true;

    this.dialogVisible = true;
  }

  dayTabSelected(event:{item: MenuItem; event: PointerEvent | KeyboardEvent}){
    let item = event.item;
    this.generateSpaceReservationTable(item);

    const now = new Date();
    if(this.selectedSpace!.status == SpaceStatus.OCCUPIED && this.reservationsOfDay[0].startTime <= now && this.reservationsOfDay[0].endTime >= now)
      this.reservationsOfDay[0].isReserved = true;
  }

  generateSpaceReservationTable(item: MenuItem) {
    let spaceRes = this.selectedSpace!.reservations;
    let resOfDay =  spaceRes.filter(res => this.isTimeRangesIntercept(
      res.startTime,
      res.endTime,
      item.state?.dateBegin,
      item.state?.dateEnd
    )).map(res => ({
      ...res,
      isReserved: true
    }));

    let dateBegin = item.state?.dateBegin;
    let dateEnd = item.state?.dateEnd;
    let beforeFirst: SpaceReservation | null = null;
    let afterLast: SpaceReservation | null = null;

    for(let i=0; i<spaceRes.length; i++)
      if(spaceRes[i].endTime < dateBegin)
        beforeFirst = spaceRes[i];

    for(let i=spaceRes.length-1; i>=0; i--)
      if(spaceRes[i].startTime > dateEnd)
        afterLast = spaceRes[i];

    if(resOfDay.length == 0) {
      this.reservationsOfDay = [{
        username: "-",
        startTime: beforeFirst ? beforeFirst.endTime : this.minDate,
        endTime: afterLast ? afterLast.startTime : this.maxDate,
        isReserved: false
      }];
      return;
    }

    if(resOfDay[0].startTime <= dateBegin && resOfDay[0].endTime >= dateEnd) {
      return;
    }

    let i=1;

    if(resOfDay[0].startTime > dateBegin) {
      resOfDay.splice(0,0,{
        username: "-",
        startTime: beforeFirst
          ? beforeFirst.endTime
          : this.minDate,
        endTime: resOfDay[0].startTime,
        isReserved: false
      });
      i++;
    }

    while(i < resOfDay.length){
      if(resOfDay[i-1].endTime != resOfDay[i].startTime) {
        resOfDay.splice(i,0,{
          username: "-",
          startTime: resOfDay[i-1].endTime,
          endTime: resOfDay[i].startTime,
          isReserved: false
        });
        i++;
      }
      i++;
    }

    if(resOfDay[resOfDay.length-1].endTime < dateEnd) {
      resOfDay.splice(resOfDay.length,0,{
        username: "-",
        startTime: resOfDay[resOfDay.length-1].endTime,
        endTime: afterLast
          ? afterLast.startTime
          : this.maxDate,
        isReserved: false
      })
    }

    this.reservationsOfDay = resOfDay;
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
