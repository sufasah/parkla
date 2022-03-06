import { AfterViewInit, Component, OnInit } from '@angular/core';
import { ParkArea, ParkingLot } from '@app/core/models/parking-lot';

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

  parkingLot: ParkingLot = {
    id: 1,
    lat: 41,
    lng: 29,
    location: "The Marmara Sea",
    name: "SeaPark",
    areas: [{
      name: "basement5",
      reservationsEnabled: true,
      notReservedOccupiable: true,
      spaces: [
        {
          id:"178",
          status: "empty",
          templatePath: [
            [165,21],
            [165,83],
            [197,83],
            [197,21]
          ],
          reservations: [{
            startTime: new Date(Date.now()+1000*60*60),
            endTime: new Date(Date.now()+1000*60*60*2)
          }]
        },
        {
          id:"177",
          status: "occupied",
          templatePath: [
            [133,21],
            [133,83],
            [165,83],
            [165,21]
          ],
          reservations: [{
            startTime: new Date(Date.now()+1000*60*60),
            endTime: new Date(Date.now()+1000*60*60*2)
          }]
        },
        {
          id:"181",
          status: "empty",
          templatePath: [
            [261,21],
            [261,84],
            [293,83],
            [294,21]
          ],
          reservations: [{
            startTime: new Date(Date.now()+1000*60*60),
            endTime: new Date(Date.now()+1000*60*60*2)
          }]
        },
        {
          id:"184",
          status: "occupied",
          templatePath: [
            [358,22],
            [358,84],
            [391,84],
            [391,21]
          ],
          reservations: [{
            startTime: new Date(Date.now()+1000*60*60),
            endTime: new Date(Date.now()+1000*60*60*2)
          }]
        },
        {
          id:"183",
          status: "empty",
          templatePath: [
            [326,21],
            [326,82],
            [358,83],
            [358,22]
          ],
          reservations: [{
            startTime: new Date(Date.now()+1000*60*60),
            endTime: new Date(Date.now()+1000*60*60*2)
          }]
        }
      ],
      templateImg: "https://www.realserve.com.au/wp-content/uploads/CarParkingPlans/CAR-PARKING-PLAN-SERVICE-BY.jpg"
    },{
      name: "heyyo",
      reservationsEnabled: false,
      notReservedOccupiable: false,
      spaces: [],
      templateImg: "https://www.atlanticenter.com/wp-content/uploads/2019/04/floor-plan-first-basement-floor-indoor-parking-spaces-to-rent-milan-1.jpg"
    }],
    status: {
      timestamp: Date.now(),
      emptySpace: 20,
      occupiedSpace: 5,
      reservedSpace: 4,
    }
  }

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
