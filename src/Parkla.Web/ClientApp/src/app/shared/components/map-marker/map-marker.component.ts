import { Component, EventEmitter, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-map-marker',
  templateUrl: './map-marker.component.html',
  styleUrls: ['./map-marker.component.scss']
})
export class MapMarkerComponent implements OnInit {

  @Output()
  public onClick = new EventEmitter<any>();

  constructor() { }

  ngOnInit(): void {
  }

  markerClick(event:any) {
    this.onClick.emit(event);
  }

}
