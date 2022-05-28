import { AfterViewInit, Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { NgForm } from '@angular/forms';
import { TimeUnit } from '@app/core/enums/TimeUnit';
import { ParkArea } from '@app/core/models/park-area';
import { capitalize } from '@app/core/utils/string';

@Component({
  selector: 'app-park-area-form',
  templateUrl: './park-area-form.component.html',
  styleUrls: ['./park-area-form.component.scss']
})
export class ParkAreaFormComponent implements OnInit, AfterViewInit {

  @Input()
  area!: ParkArea;

  @Output()
  areaChange = new EventEmitter<ParkArea>();

  @Output()
  formSubmit = new EventEmitter<NgForm>();

  @Input()
  loading = false;

  @Input()
  submitLabel = "Submit";

  timeUnitOptions: TimeUnit[] = [
    "MINUTE",
    "HOUR",
    "DAY",
    "MONTH"
  ]
  constructor(
  ) { }

  ngOnInit(): void {

  }

  ngAfterViewInit(): void {

  }

  submit(ngForm: NgForm) {
    this.formSubmit.emit(ngForm);
  }

  addPricing() {
    if(!this.area.pricings)
      this.area.pricings = [];

    this.area.pricings!.push(<any>{areaId: this.area.id});
  }

  removePricing(index: number) {
    this.area.pricings?.splice(index,1);
  }

  isTimeInterval(val: any) {
    return 'beginningTime' in val && 'endTime' in val;
  }

  isPerTime(val: any) {
    return 'timeUnit' in val && 'timeAmount' in val;
  }

}
