import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { DAY, HOUR, MINUTE, SECOND } from '@app/core/constants/time';
import { ParkSpace } from '@app/core/models/park-space';
import { getPricePerHour } from '@app/core/models/pricing';
import { Reservation } from '@app/core/models/reservation';
import { MenuItem, MessageService } from 'primeng/api';

@Component({
  selector: 'app-reservation-modal',
  templateUrl: './reservation-modal.component.html',
  styleUrls: ['./reservation-modal.component.scss']
})
export class ReservationModalComponent implements OnInit, OnChanges {

  @Input()
  selectedSpace?: ParkSpace;

  @Input()
  minDate = new Date();

  @Input()
  maxDate = new Date(Date.now()+1000*60*60*24*6);

  @Input()
  showReservationModal!: EventEmitter<boolean>;

  @Input()
  userMode = true;

  @Input()
  selectedTime: [Date?, Date?] = [undefined, undefined]

  @Output()
  reserveSpace = new EventEmitter<{space: ParkSpace, timeRange: [Date, Date]}>();

  dialogVisible = false;

  dayNames = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

  reserveLoading = false;

  reserveConfirmationVisible = false;
  payment?: number;

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

  reservationsOfDay: Reservation[] & {isReserved: boolean}[] = [];

  constructor(
    private messageService: MessageService
  ) { }

  ngOnInit(): void {
    this.showReservationModal.subscribe((show) => {
      this.reserveLoading = false;
      if(show) {
        this.showReserveModal();
      }
      else {
        this.dialogVisible = false;
      }
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if(changes.selectedSpace || changes.selectedTime) {
      const selectedSpace = changes.selectedSpace?.currentValue ?? this.selectedSpace;
      const selectedTime = changes.selectedTime?.currentValue ?? this.selectedTime;

      this.calculatePayment(selectedSpace, selectedTime)
    }
  }

  calculatePayment(space?: ParkSpace, timeRange?: [Date?, Date?]) {
    if(!space || !timeRange || !timeRange[0] || !timeRange[1] || !space.pricing) {
      this.payment = undefined;
    }
    else {
      const pricePerHour = getPricePerHour(space.pricing);
      const hourDiff = (timeRange[1].getTime()-timeRange[0].getTime()) / 36e5;
      this.payment = pricePerHour * hourDiff;
    }
  }

  reserveClick() {
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

    this.reserveConfirmationVisible = true;
  }

  reserveConfirm() {
    this.reserveConfirmationVisible = false;
    this.reserveSpace.emit({space: this.selectedSpace!, timeRange: [this.selectedTime[0]!, this.selectedTime[1]!]});
  }

  reserveCancel() {
    this.reserveConfirmationVisible = false;
  }

  showReserveModal() {
    this.generateSpaceReservationTable(this.weekDays[0]);

    const now = new Date();
    if(this.selectedSpace!.status.toUpperCase() == "OCCUPIED" && this.reservationsOfDay[0].startTime <= now && this.reservationsOfDay[0].endTime >= now)
      this.reservationsOfDay[0].isReserved = true;

    this.dialogVisible = true;
  }

  dayTabSelected(event:{item: MenuItem; event: PointerEvent | KeyboardEvent}){
    let item = event.item;
    this.generateSpaceReservationTable(item);

    const now = new Date();
    if(this.selectedSpace!.status.toUpperCase() == "OCCUPIED" && this.reservationsOfDay[0].startTime <= now && this.reservationsOfDay[0].endTime >= now)
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
    let beforeFirst: Reservation | null = null;
    let afterLast: Reservation | null = null;

    for(let i=0; i<spaceRes.length; i++)
      if(spaceRes[i].endTime < dateBegin)
        beforeFirst = spaceRes[i];

    for(let i=spaceRes.length-1; i>=0; i--)
      if(spaceRes[i].startTime > dateEnd)
        afterLast = spaceRes[i];

    if(resOfDay.length == 0) {
      this.reservationsOfDay = [<any>{
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
      resOfDay.splice(0,0,<any>{
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
        resOfDay.splice(i,0,<any>{
          startTime: resOfDay[i-1].endTime,
          endTime: resOfDay[i].startTime,
          isReserved: false
        });
        i++;
      }
      i++;
    }

    if(resOfDay[resOfDay.length-1].endTime < dateEnd) {
      resOfDay.splice(resOfDay.length,0,<any>{
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
