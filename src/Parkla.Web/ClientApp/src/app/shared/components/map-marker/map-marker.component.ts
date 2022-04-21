import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Park } from '@app/core/models/park';

@Component({
  selector: 'app-map-marker',
  templateUrl: './map-marker.component.html',
  styleUrls: ['./map-marker.component.scss']
})
export class MapMarkerComponent implements OnInit {

  @Output()
  public onClick = new EventEmitter<any>();

  @Input()
  park!:Park;

  get spaceCount() {
    return this.park.status.emptySpace +
      this.park.status.reservedSpace +
      this.park.status.occupiedSpace
  }

  constructor() { }

  ngOnInit(): void {
  }

  markerClick(event:any) {
    this.onClick.emit(event);
  }

}
