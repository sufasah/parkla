import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { RSRoute } from '@app/core/constants/ref-sharing';
import { Park } from '@app/core/models/park';
import { AuthService } from '@app/core/services/auth.service';
import { RefSharingService } from '@app/core/services/ref-sharing.service';
import { RouteUrl } from '@app/core/utils/route';
import { mockParks } from '@app/mock-data/parking-lots';
import { MapMarkerComponent } from '@app/shared/components/map-marker/map-marker.component';

import SearchBox, { } from "@tomtom-international/web-sdk-plugin-searchbox";

@Component({
  selector: 'app-park-map',
  templateUrl: './park-map.component.html',
  styleUrls: ['./park-map.component.scss']
})
export class ParkMapComponent implements OnInit, AfterViewInit {

  parks = mockParks;

  dialogVisible = false;

  selectedPark: Park | null = null;

  get spaceCount() {
    return this.selectedPark!.status.emptySpace +
      this.selectedPark!.status.reservedSpace +
      this.selectedPark!.status.occupiedSpace
  }

  constructor(
    private router: Router,
    private refSharingService: RefSharingService,
    private authService: AuthService) {

  }

  ngOnInit(): void {

  }

  ngAfterViewInit(): void {

  }

  navigateGoogleMaps(lat: number, lng: number) {
    window.location.href = `https://www.google.com/maps/place/${lat.toFixed(20)}+${lng.toFixed(20)}/@${lat.toFixed(20)},${lng.toFixed(20)},12z`;
  }

  navigateToParkAreas(park:Park) {
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

    $("#appMap div.mapboxgl-ctrl-top-right button.mapboxgl-ctrl-shrink").trigger("click");
  }

}
