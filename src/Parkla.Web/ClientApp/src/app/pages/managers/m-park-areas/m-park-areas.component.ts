import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RSRoute } from '@app/core/constants/ref-sharing.const';
import { ParkArea } from '@app/core/models/park-area';
import { ParkingLot } from '@app/core/models/parking-lot';
import { RefSharingService } from '@app/core/services/ref-sharing.service';
import { RouteUrl } from '@app/core/utils/route.util';
import { mockAreas } from '@app/mock-data/areas';
import { ConfirmationService, MessageService } from 'primeng/api';

@Component({
  selector: 'app-m-park-areas',
  templateUrl: './m-park-areas.component.html',
  styleUrls: ['./m-park-areas.component.scss']
})
export class MParkAreasComponent implements OnInit {

  areas: ParkArea[] = [];

  constructor(
    private refSharingService: RefSharingService,
    private router: Router,
    private route: ActivatedRoute,
    private confirmService: ConfirmationService,
    private messageService: MessageService) { }

  ngOnInit(): void {
    let park = this.refSharingService.getData<ParkingLot>(RSRoute.mapSelectedPark);

    if(!!park)
      this.areas = park.areas;
    else {
      // get from server
      this. areas = mockAreas;
    }
  }

  goMap() {
    this.router.navigate([RouteUrl.mParkMap()]);
  }

  newArea() {
    let parkid = this.route.snapshot.params.parkid;
    this.router.navigateByUrl(RouteUrl.mNewParkArea(parkid))
  }

  editArea(area: ParkArea) {
    let parkid = this.route.snapshot.params.parkid;
    this.router.navigateByUrl(RouteUrl.mEditParkArea(parkid,area.id))
  }

  deleteArea(area: ParkArea) {
    this.confirmService.confirm({
      message: 'Are you sure to delete xxx named park area with xxx id?',
      accept: () => {
        // TODO: DELETE PARK AREA

        this.messageService.add({
          summary: "Park Area Deletion",
          closable: true,
          severity: "error",
          life:1500,
          detail: "The park area with xxx id and xxx name is deleted."
        })
      }
    });
  }
}
