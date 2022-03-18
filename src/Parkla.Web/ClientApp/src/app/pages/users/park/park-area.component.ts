import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RSRoute } from '@app/core/constants/ref-sharing.const';
import { DAY, HOUR, MINUTE, SECOND } from '@app/core/constants/time.const';
import { ParkArea, ParkingLot, ParkSpace, SpaceReservation } from '@app/core/models/parking-lot';
import { RefSharingService } from '@app/core/services/ref-sharing.service';
import { RouteUrl } from '@app/core/utils/route.util';
import { mockAreas } from '@app/mock-data/areas';
import { ParkTemplateComponent } from '@app/shared/components/park-template/park-template.component';
import { ConfirmationService, MenuItem, MessageService } from 'primeng/api';

@Component({
  selector: 'app-park',
  templateUrl: './park-area.component.html',
  styleUrls: ['./park-area.component.scss']
})
export class ParkAreaComponent implements OnInit, AfterViewInit {

  @ViewChild("parkTemplate")
  parkTemlate?: ParkTemplateComponent;

  dayNames = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

  selectedArea!: ParkArea;

  selectedSpace!: ParkSpace;

  dialogVisible = false;

  timeRange:[Date?, Date?] = [
    new Date(),
    new Date(Date.now()+60000*15)
  ];

  minDate = new Date();

  maxDate = new Date(Date.now()+1000*60*60*24*6);

  _weekDays: MenuItem[] = [{label:"x"}];
  get weekDays() {
    let nowBegin = new Date();
    let dayCode = nowBegin.getDay();
    nowBegin.setHours(0,0,0)

    if(this._weekDays[0].label != this.dayNames[dayCode]) {
      this._weekDays = [];
      for(let i=0; i<7; i++) {
        let dayIndex = (dayCode+i)%7;
        let dateBeginTimestamp = nowBegin.getTime() + i*DAY;
        let dateEndTimestamp = dateBeginTimestamp
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
    private refSharingService: RefSharingService,
    private router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService,
    private confirmationService: ConfirmationService) { }

  ngOnInit(): void {
    let area = this.refSharingService.getData<ParkArea>(RSRoute.areasSelectedArea);

    if(!!area)
      this.selectedArea = area;
    else {
      //get from server
      this.selectedArea = mockAreas[0];
    }
  }

  ngAfterViewInit(): void {

  }

  spaceClicked(space: ParkSpace) {
    this.selectedSpace = space;

    if(this.selectedArea.reservationsEnabled) {
      if(space.status == "empty") {
        this.showReserveModal();
        //show time interval selected
        //show pricig table
        //show reservation intervals and available intervals between them
        //if reserved show reserved user's username
        //if user wallet is not enough it must not be possible to confirm
      }
      else if((<any>space).isReserved) {
        this.showReserveModal();
      }
      else {
        if(!this.selectedArea.notReservedOccupiable) {
          this.showReserveModal();
        }
        else {
          this.messageService.add({
            life:1500,
            severity:'error',
            summary: 'Occupied Reservation',
            detail: 'It is not possible to reserve occupied space.',
          });
        }
      }
    }
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
    let parkid = this.route.snapshot.params["parkid"];
    this.refSharingService.removeData(RSRoute.areasSelectedArea);
    this.router.navigate([RouteUrl.parkAreas(parkid)]);
  }

  showReserveModal() {
    this.genereateReservationsOfDay(this.weekDays[0])
    this.dialogVisible = true;
  }

  timeRangeChange(timeRange:any) {
    this.timeRange = timeRange;

    if(!this.timeRange[0] || !this.timeRange[1]) return;

    this.selectedArea.spaces.forEach(space => {
      if(this.selectedArea.reservationsEnabled && space.reservations) {
        for(let i=0; i<space.reservations.length; i++) {
          let reservation = space.reservations[i];

          space.isReserved = this.isTimeRangesIntercept(
            reservation.startTime,
            reservation.endTime,
            this.timeRange[0]!,
            this.timeRange[1]!
          );

          if(space.isReserved) break;
        }
      }
    })

    this.parkTemlate?.drawCanvas();
  }

  dayTabSelected(event:any){
    let item: MenuItem = event.item;
    this.genereateReservationsOfDay(item);
  }

  genereateReservationsOfDay(item: MenuItem) {
    let spaceRes = this.selectedSpace.reservations;
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
