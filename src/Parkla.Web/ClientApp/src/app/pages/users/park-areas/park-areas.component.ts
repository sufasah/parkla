import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ParkArea } from '@app/core/models/park-area';

@Component({
  selector: 'app-areas',
  templateUrl: './park-areas.component.html',
  styleUrls: ['./park-areas.component.scss']
})
export class ParkAreasComponent implements OnInit {

  areas: ParkArea[] = [];

  constructor(
    private router: Router) { }

  ngOnInit(): void {
    //this. areas = mockAreas;
  }

  goMap() {
    this.router.navigate([".."]);
  }
}
