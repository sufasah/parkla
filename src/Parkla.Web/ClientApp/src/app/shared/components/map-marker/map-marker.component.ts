import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ParkingLot } from '@app/core/models/parking-lot';

@Component({
  selector: 'app-map-marker',
  templateUrl: './map-marker.component.html',
  styleUrls: ['./map-marker.component.scss']
})
export class MapMarkerComponent implements OnInit {

  @Output()
  public onClick = new EventEmitter<any>();

  @Input()
  park!:ParkingLot;

  constructor() { }

  ngOnInit(): void {
  }

  markerClick(event:any) {
    this.onClick.emit(event);
  }

}
