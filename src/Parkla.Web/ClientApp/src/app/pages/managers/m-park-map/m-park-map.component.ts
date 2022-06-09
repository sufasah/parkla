import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { ChangablePark, Park } from '@app/core/models/park';
import { AuthService } from '@app/core/services/auth.service';
import { ParkService } from '@app/core/services/park.service';
import { RouteUrl } from '@app/core/utils/route';
import { MapDialogComponent } from '@app/shared/components/map-dialog/map-dialog.component';
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

  @ViewChild(MapDialogComponent)
  mapDialog?: MapDialogComponent;

  unsubscribe: Subscription[] = [];

  constructor(
    private router: Router,
  ) { }

  ngOnInit(): void {
  }

  newPark() {
    this.router.navigateByUrl(RouteUrl.mNewPark());
  }

  markerClick(event:{event:any; changablePark: ChangablePark}) {
    this.mapDialog?.showDialog(event.changablePark);
    $("#appMap div.mapboxgl-ctrl-top-right button.mapboxgl-ctrl-shrink").trigger("click");
  }

  ngOnDestroy(): void {
    this.unsubscribe.forEach(x => x.unsubscribe());
  }

}
