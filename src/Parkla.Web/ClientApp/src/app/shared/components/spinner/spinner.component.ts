import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-spinner',
  templateUrl: './spinner.component.html',
  styleUrls: ['./spinner.component.scss']
})
export class SpinnerComponent implements OnInit {

  @Input()
  blockSpinner:boolean = false;

  @Input()
  target:any;

  @Input()
  blocked: boolean = false;

  @Input()
  cancellable: boolean = true;

  @Output()
  blockedChange = new EventEmitter<boolean>();

  constructor() { }

  ngOnInit(): void {
  }

  cancelClick(){
    this.blocked = false;
    this.blockedChange.emit(false);
  }
}
