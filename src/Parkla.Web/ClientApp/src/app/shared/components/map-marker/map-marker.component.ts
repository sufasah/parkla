import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ChangablePark, Park } from '@app/core/models/park';

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
    let total = this.park.emptySpace +
      this.park.reservedSpace +
      this.park.occupiedSpace;

    return total == 0 ? 1 : total;
  }

  constructor() { }

  ngOnInit(): void {
  }

  markerClick(event:any) {
    this.onClick.emit(event);
  }

}
