import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { Park } from '@app/core/models/park';

@Component({
  selector: 'app-map-marker',
  templateUrl: './map-marker.component.html',
  styleUrls: ['./map-marker.component.scss']
})
export class MapMarkerComponent implements OnInit, OnChanges {

  @Output()
  public onClick = new EventEmitter<any>();

  @Input()
  park!:Park;

  spaceCount = 1;

  constructor() { }

  ngOnChanges(changes: SimpleChanges): void {
    let park: Park = changes.park.currentValue;

    this.spaceCount = park.emptySpace + park.occupiedSpace;
    this.spaceCount = this.spaceCount == 0 ? 1 : this.spaceCount;
  }

  ngOnInit(): void {
    this.spaceCount = this.park.emptySpace + this.park.occupiedSpace;
    this.spaceCount = this.spaceCount == 0 ? 1 : this.spaceCount;
  }

  markerClick(event:any) {
    this.onClick.emit(event);
  }

}
