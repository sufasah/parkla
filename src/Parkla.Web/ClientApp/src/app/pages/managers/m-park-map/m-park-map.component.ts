import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { RSRoute } from '@app/core/constants/ref-sharing.const';
import { ParkingLot } from '@app/core/models/parking-lot';
import { AuthService } from '@app/core/services/auth.service';
import { RefSharingService } from '@app/core/services/ref-sharing.service';
import { RouteUrl } from '@app/core/utils/route.util';
import { mockParkingLots } from '@app/mock-data/parking-lots';
import { MapMarkerComponent } from '@app/shared/components/map-marker/map-marker.component';
import { ConfirmationService, MessageService } from 'primeng/api';

@Component({
  selector: 'app-m-park-map',
  templateUrl: './m-park-map.component.html',
  styleUrls: ['./m-park-map.component.scss']
})
export class MParkMapComponent implements OnInit {

  parks = mockParkingLots;

  dialogVisible = false;

  selectedPark: ParkingLot | null = null;

  get spaceCount() {
    return this.selectedPark!.status.emptySpace +
      this.selectedPark!.status.reservedSpace +
      this.selectedPark!.status.occupiedSpace
  }

  constructor(
    private router: Router,
    private refSharingService: RefSharingService,
    private authService: AuthService,
    private confirmService: ConfirmationService,
    private messageService: MessageService) { }

  ngOnInit(): void {
  }

  newParkingLot() {
    this.router.navigateByUrl(RouteUrl.mNewPark());
  }

  navigateToParkAreas(park:ParkingLot) {
    this.refSharingService.setData(RSRoute.mapSelectedPark,park);

    this.router.navigateByUrl(this.authService.asManager
      ? RouteUrl.mParkAreas(park.id)
      : RouteUrl.parkAreas(park.id)
    );
  }

  markerClick(event:{event:any; element:MapMarkerComponent}) {
    console.log(event);

    let element = event.element;
    event = event.event;

    this.selectedPark = element.park;
    this.dialogVisible = true;
  }

  editPark(park: ParkingLot) {
    this.router.navigateByUrl(RouteUrl.mEditPark(park.id));
  }

  deletePark(park: ParkingLot) {
    this.confirmService.confirm({
      message: 'Are you sure to delete xxx named park with xxx id?',
      accept: () => {
        // TODO: DELETE PARK

        this.dialogVisible = false;

        this.messageService.add({
          summary: "Park Deletion",
          closable: true,
          severity: "success",
          life:1500,
          detail: "The xxx id park with xx name is deleted."
        })
      }
    });
  }

}
