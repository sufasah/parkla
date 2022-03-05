import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ParkingLot, ParkSpace, SpacePath } from '@app/core/models/parking-lot';
import { select } from 'd3-selection';
import { zoom, zoomIdentity, ZoomTransform } from "d3-zoom";

declare var $:any;

  @Component({
  selector: 'app-park',
  templateUrl: './park.component.html',
  styleUrls: ['./park.component.scss']
})
export class ParkComponent implements OnInit, AfterViewInit {


  areas = [

  ]

  areaSuggestions = []

  selectedArea = null;

  timeRange:[Date? ,Date?] = [
    new Date(),
    new Date(Date.now()+60000*15)
  ];

  minDate = new Date();
  maxDate = new Date(Date.now()+1000*60*60*24*3);

  parkingLot: ParkingLot = {
    id: "lot1",
    lat: 41,
    lng: 29,
    location: "The Marmara Sea",
    name: "SeaPark",
    reservationsEnabled: true,
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
    status: {
      timestamp: Date.now(),
      emptySpace: 20,
      occupiedSpace: 5,
      reservedSpace: 4,
    }
  }

  constructor() { }

  ngOnInit(): void {
    this.areaSuggestions = <any>[
      "a",
      "b",
      "c"
    ];
  }

  ngAfterViewInit(): void {


  }

  searchArea(e:any) {
    console.log(e.query );
    if(!e.query) this.areaSuggestions = <any>["a","b","c"];
  }


}

type Point = [number,number];
