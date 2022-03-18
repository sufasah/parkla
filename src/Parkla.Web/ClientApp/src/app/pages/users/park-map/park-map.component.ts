import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { mockParkingLots } from '@app/mock-data/parking-lots';

import SearchBox, { } from "@tomtom-international/web-sdk-plugin-searchbox";

@Component({
  selector: 'app-park-map',
  templateUrl: './park-map.component.html',
  styleUrls: ['./park-map.component.scss']
})
export class ParkMapComponent implements OnInit, AfterViewInit {

  parks = mockParkingLots;

  constructor() {

  }

  ngOnInit(): void {

  }

  ngAfterViewInit(): void {

  }

}
