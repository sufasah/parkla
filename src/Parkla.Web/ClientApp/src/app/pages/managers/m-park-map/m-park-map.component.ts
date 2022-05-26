import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Park } from '@app/core/models/park';
import { AuthService } from '@app/core/services/auth.service';
import { ParkService } from '@app/core/services/park.service';
import { RouteUrl } from '@app/core/utils/route';
import { MapMarkerComponent } from '@app/shared/components/map-marker/map-marker.component';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-m-park-map',
  templateUrl: './m-park-map.component.html',
  styleUrls: ['./m-park-map.component.scss']
})
export class MParkMapComponent implements OnInit, OnDestroy {

  parks = [];

  dialogVisible = false;

  selectedPark: Park | null = null;

  unsubscribe: Subscription[] = [];

  get spaceCount() {
    let total = this.selectedPark!.emptySpace +
      this.selectedPark!.reservedSpace +
      this.selectedPark!.occupiedSpace;

    return total == 0 ? 1 : total;
  }

  constructor(
    private router: Router,
    private authService: AuthService,
    private confirmService: ConfirmationService,
    private messageService: MessageService,
    private parkService: ParkService) { }

  ngOnInit(): void {
  }

  newPark() {
    this.router.navigateByUrl(RouteUrl.mNewPark());
  }

  navigateToParkAreas(park:Park) {

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

    $("#appMap div.mapboxgl-ctrl-top-right button.mapboxgl-ctrl-shrink").trigger("click");
  }

  editPark(park: Park) {
    this.router.navigateByUrl(RouteUrl.mEditPark(park.id));
  }

  deletePark(park: Park) {
    this.confirmService.confirm({
      message: `Are you sure to delete the park with '${park.name}' name?`,
      accept: () => {
        this.parkService.deletePark(park).subscribe({
          next: () => {
            this.messageService.add({
              summary: "Park Deletion",
              closable: true,
              severity: "success",
              life:1500,
              detail: `The park with '${park.name}' name is deleted.`
            });
          },
          error: (err: HttpErrorResponse) => {
            this.messageService.add({
              summary: "Park Deletion",
              closable: true,
              severity: "error",
              life:5000,
              detail: err.error.message
            });
          }
        })

        this.dialogVisible = false;

      }
    });
  }

  ngOnDestroy(): void {
    this.unsubscribe.forEach(x => x.unsubscribe());
  }

}
