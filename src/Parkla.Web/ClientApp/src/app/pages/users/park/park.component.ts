import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ParkSpace, SpacePath } from '@app/core/models/parking-lot';
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

  timeRange:[Date?,Date?] = [];

  minDate = new Date();
  maxDate = new Date(Date.now()+1000*60*60*24*3);

  parkSpaces: ParkSpace[] = [
    {
      id:"178",
      status: "free",
      templatePath: [
        [165,21],
        [165,83],
        [197,83],
        [197,21]
      ],
      reservations: [{
        startTime: new Date(),
        endTime: new Date(Date.now()+1000*60*60)
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
        startTime: new Date(),
        endTime: new Date(Date.now()+1000*60*60)
      }]
    },
    {
      id:"181",
      status: "free",
      templatePath: [
        [261,21],
        [261,84],
        [293,83],
        [294,21]
      ],
      reservations: [{
        startTime: new Date(),
        endTime: new Date(Date.now()+1000*60*60)
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
        startTime: new Date(),
        endTime: new Date(Date.now()+1000*60*60)
      }]
    },
    {
      id:"183",
      status: "free",
      templatePath: [
        [326,21],
        [326,82],
        [358,83],
        [358,22]
      ],
      reservations: [{
        startTime: new Date(),
        endTime: new Date(Date.now()+1000*60*60)
      }]
    }
  ]

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
