import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { ChangablePark, Park } from '@app/core/models/park';
import { AuthService } from '@app/core/services/auth.service';
import { ParkService } from '@app/core/services/park.service';
import { RouteUrl } from '@app/core/utils/route';
import { MapDialogComponent } from '@app/shared/components/map-dialog/map-dialog.component';
import { MapMarkerComponent } from '@app/shared/components/map-marker/map-marker.component';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-park-map',
  templateUrl: './park-map.component.html',
  styleUrls: ['./park-map.component.scss']
})
export class ParkMapComponent implements OnInit, OnDestroy {

  parks = [];

  @ViewChild(MapDialogComponent)
  mapDialog?: MapDialogComponent;

  unsubscribe: Subscription[] = [];

  constructor(
    private router: Router,
    private authService: AuthService
  ) {

  }

  ngOnInit(): void {

  }

  navigateGoogleMaps(lat: number, lng: number) {
    window.location.href = `https://www.google.com/maps/place/${lat.toFixed(20)}+${lng.toFixed(20)}/@${lat.toFixed(20)},${lng.toFixed(20)},12z`;
  }

  markerClick(event:{event:any; changablePark: ChangablePark}) {
    this.mapDialog?.showDialog(event.changablePark);
    $("#appMap div.mapboxgl-ctrl-top-right button.mapboxgl-ctrl-shrink").trigger("click");
  }

  ngOnDestroy(): void {
    this.unsubscribe.forEach(x => x.unsubscribe());
  }
}
