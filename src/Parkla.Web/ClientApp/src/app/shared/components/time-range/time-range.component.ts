import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { Calendar } from 'primeng/calendar';

@Component({
  selector: 'app-time-range',
  templateUrl: './time-range.component.html',
  styleUrls: ['./time-range.component.scss']
})
export class TimeRangeComponent implements OnInit {

  @Input()
  enabled = true;

  @Input()
  minDate?: Date;

  @Input()
  maxDate?: Date;

  @Output()
  onChange = new EventEmitter<[Date, Date]>();

  @ViewChild(Calendar)
  calendar!: Calendar;

  timeRange:[Date?, Date?] = [
    new Date(),
    new Date(Date.now()+60000*15)
  ];

  constructor() { }

  ngOnInit(): void {
    setTimeout(() => {
      this.onChange.emit(<any>this.timeRange);
    }, 0);
  }

  timeRangeChange(timeRange:any) {
    this.timeRange = timeRange;
    if(!this.timeRange[0] || !this.timeRange[1]) return;
    this.onChange.emit(<any>this.timeRange);
  }

  showCalendar() {
    this.calendar.toggle();
  }
}
