import { AfterViewInit, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RSRoute } from '@app/core/constants/ref-sharing.const';
import { ParkArea, ParkingLot } from '@app/core/models/parking-lot';
import { RefSharingService } from '@app/core/services/ref-sharing.service';
import { RouteUrl } from '@app/core/utils/route.util';
import { mockAreas } from '@app/mock-data/areas';
import { mockParkingLots } from '@app/mock-data/parking-lots';

declare var $:any;

  @Component({
  selector: 'app-park',
  templateUrl: './park-area.component.html',
  styleUrls: ['./park-area.component.scss']
})
export class ParkAreaComponent implements OnInit, AfterViewInit {

  areaSuggestions: ParkArea[] = [];

  _selectedArea!: ParkArea;

  dialogVisible = false;

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

  constructor(
    private refSharingService: RefSharingService,
    private router: Router,
    private route: ActivatedRoute) { }

  ngOnInit(): void {
    let area = this.refSharingService.getData<ParkArea>(RSRoute.areasSelectedArea);

    if(!!area)
      this.selectedArea = area;
    else {
      //get from server
      this.selectedArea = mockAreas[0];
    }
  }

  ngAfterViewInit(): void {

  }

  reserveSpace() {

  }

  goAreas() {
    let parkid = this.route.snapshot.params["parkid"];
    this.refSharingService.removeData(RSRoute.areasSelectedArea);
    this.router.navigate([RouteUrl.parkAreas(parkid)]);
  }
}
