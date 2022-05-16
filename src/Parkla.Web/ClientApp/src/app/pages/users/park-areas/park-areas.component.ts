import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { RSRoute } from '@app/core/constants/ref-sharing';
import { ParkArea } from '@app/core/models/park-area';
import { Park } from '@app/core/models/park';
import { mockAreas } from '@app/mock-data/areas';

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
    this. areas = mockAreas;
  }

  goMap() {
    this.router.navigate([".."]);
  }
}
