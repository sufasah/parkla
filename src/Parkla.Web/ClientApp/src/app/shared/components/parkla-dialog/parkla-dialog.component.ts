import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-parkla-dialog',
  templateUrl: './parkla-dialog.component.html',
  styleUrls: ['./parkla-dialog.component.scss']
})
export class ParklaDialogComponent implements OnInit {

  @Input()
  visible = false;

  @Input()
  title = "PARKLA"

  @Input()
  titleIcon = "pi pi-car"

  @Input()
  leftButtonStyleClass = ""

  @Input()
  rightButtonStyleClass = ""

  @Input()
  leftButtonLabel = "Cancel"

  @Input()
  rightButtonLabel = "Confirm"

  @Input()
  leftButtonIcon = "pi pi-times"

  @Input()
  rightButtonIcon = "pi pi-check"

  @Output()
  leftButtonClick = new EventEmitter<any>();

  @Output()
  rightButtonClick = new EventEmitter<any>();

  constructor() { }

  ngOnInit(): void {
  }

  leftButtonClickFn(event: any) {
    this.leftButtonClick.emit(event);
  }

  rightButtonClickFn(event: any) {
    this.rightButtonClick.emit(event);
  }
}
