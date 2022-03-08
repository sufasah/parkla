import { AfterViewInit, Component, OnInit } from '@angular/core';
import { ParkArea, ParkingLot } from '@app/core/models/parking-lot';
import { mockAreas } from '@app/mock-data/areas';
import { mockParkingLots } from '@app/mock-data/parking-lots';

declare var $:any;

  @Component({
  selector: 'app-park',
  templateUrl: './park.component.html',
  styleUrls: ['./park.component.scss']
})
export class ParkComponent implements OnInit, AfterViewInit {

  areaSuggestions: ParkArea[] = [];

  _selectedArea!: ParkArea;

  set selectedArea(value: ParkArea) {
    if(!value) return;
    this._selectedArea = value;
  }

  get selectedArea() {
    return this._selectedArea;
  }

  timeRange:[Date? ,Date?] = [
    new Date(),
    new Date(Date.now()+60000*15)
  ];

  minDate = new Date();
  maxDate = new Date(Date.now()+1000*60*60*24*3);

  parkingLot: ParkingLot = mockParkingLots[0];

  constructor() { }

  ngOnInit(): void {
    this.areaSuggestions = this.parkingLot.areas;
    this.selectedArea = this.parkingLot.areas[0];
  }

  ngAfterViewInit(): void {

  }

  searchArea(e:any) {
    let result: ParkArea[] = []

    this.parkingLot.areas.some(area => (
      area.name.includes(e.query) && result.push(area),
      result.length == 10
    ));

    this.areaSuggestions = result;
  }
}
