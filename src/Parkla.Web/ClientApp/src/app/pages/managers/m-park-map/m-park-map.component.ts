import { Component, OnInit } from '@angular/core';
import { mockParkingLots } from '@app/mock-data/parking-lots';

@Component({
  selector: 'app-m-park-map',
  templateUrl: './m-park-map.component.html',
  styleUrls: ['./m-park-map.component.scss']
})
export class MParkMapComponent implements OnInit {

  parks = mockParkingLots;

  constructor() { }

  ngOnInit(): void {
  }

}
