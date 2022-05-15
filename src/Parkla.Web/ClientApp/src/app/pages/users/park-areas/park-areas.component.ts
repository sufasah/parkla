import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { RSRoute } from '@app/core/constants/ref-sharing';
import { ParkArea } from '@app/core/models/park-area';
import { Park } from '@app/core/models/park';
import { RefSharingService } from '@app/core/services/ref-sharing.service';
import { RouteUrl } from '@app/core/utils/route';
import { mockAreas } from '@app/mock-data/areas';

@Component({
  selector: 'app-areas',
  templateUrl: './park-areas.component.html',
  styleUrls: ['./park-areas.component.scss']
})
export class ParkAreasComponent implements OnInit {

  areas: ParkArea[] = [];

  constructor(
    private refSharingService: RefSharingService,
    private router: Router) { }

  ngOnInit(): void {
    let park = this.refSharingService.getData<Park>(RSRoute.mapSelectedPark);

    if(!!park)
      this.areas = park.areas;
    else {
      // get from server
      this. areas = mockAreas;
    }
  }

  goMap() {
    this.refSharingService.removeData(RSRoute.mapSelectedPark);
    this.router.navigate([".."]);
  }
}
