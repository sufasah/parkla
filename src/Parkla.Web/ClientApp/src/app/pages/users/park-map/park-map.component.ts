import { AfterViewInit, ApplicationRef, ChangeDetectorRef, Component, ElementRef, EmbeddedViewRef, OnInit, Renderer2, ViewChild, ViewContainerRef } from '@angular/core';
import { FullscreenControl, GeolocateControl, LngLat, Map, map, Marker, NavigationControl, PointLike, Popup } from "@tomtom-international/web-sdk-maps";
import { services } from "@tomtom-international/web-sdk-services";
import SearchBox, { } from "@tomtom-international/web-sdk-plugin-searchbox";
import { ttkey } from '@app/core/constants/private.const';
import { ConfirmationService } from 'primeng/api';
import { ParkingLot } from '@app/core/models/parking-lot';
import { Router } from '@angular/router';
import { mockParkingLots } from '@app/mock-data/parking-lots';
import { MapMarkerComponent } from '@app/shared/components/map-marker/map-marker.component';
@Component({
  selector: 'app-park-map',
  templateUrl: './park-map.component.html',
  styleUrls: ['./park-map.component.scss']
})
export class ParkMapComponent implements OnInit, AfterViewInit {

  @ViewChild("appSearchBox")
  appSearchBoxRef!: ElementRef<HTMLElement>;

  appMap!: Map;

  appSearchBox!: SearchBox;

  parkingLots = mockParkingLots;

  dialogVisible = false;

  constructor(
    private renderer: Renderer2,
    private viewRef: ViewContainerRef,
    private router: Router) {

  }

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    this.loadMap();
  }

  loadMap() {
    this.appMap = map({
      key: ttkey,
      container: "appMap",
      zoom: 12,
      language: "tr-TR",
      center: {
        lat:41.015137,
        lng:28.979530,
      },
    });

    this.appMap.addControl(new NavigationControl());
    this.appMap.addControl(new FullscreenControl());

    this.appSearchBox = new SearchBox(services, {
      searchOptions: {
        key: ttkey,
        language: "tr-TR",
        limit: 5
      },
      autocompleteOptions: {
        key: ttkey,
        language: "tr-TR",
},
      labels: {
        noResultsMessage: "Where are you looking for is not found."
      },
    });

    this.appSearchBox.on("tomtom.searchbox.resultselected",(e)=> {
      let result:any = e.data.result;
      this.appMap?.easeTo({
        center: {
          lat:result.position.lat,
          lng:result.position.lng,
        },
        animate: true,
        duration: 1500,
        zoom: 12
      })
    })

    this.appSearchBoxRef.nativeElement
      .appendChild(this.appSearchBox.getSearchBoxHTML());

    this.appMap.addControl(new GeolocateControl({
      trackUserLocation: true,
      showUserLocation: true,
      positionOptions: {
        enableHighAccuracy: true,
      },
      showAccuracyCircle: true
    }));

    this.parkingLots.forEach(el =>{
      this.makeMarker(el.lat,el.lng);
    });
  }

  makeMarkerElement(lat: number, lng: number) {
    let componentRef = this.viewRef.createComponent(MapMarkerComponent);

    componentRef.instance.onClick.subscribe((event:any) => this.markerOnClick());

    return (componentRef.hostView as EmbeddedViewRef<any>)
      .rootNodes[0] as HTMLElement;
  }

  makeMarker(lat: number, lng: number) {
    return new Marker(this.makeMarkerElement(lat,lng))
      .setLngLat({lat: lat, lng: lng})
      .addTo(this.appMap!);
  }

  markerOnClick() {
    this.dialogVisible = true;
  }

  navigateGoogleMaps(lat: number, lng: number) {
    window.location.href = `https://www.google.com/maps/place/${lat.toFixed(20)}+${lng.toFixed(20)}/@${lat.toFixed(20)},${lng.toFixed(20)},12z`;
  }

  async routeParkSelection(fromLat: number, fromLng: number, toLat: number, toLng: number) {
    return await services.calculateRoute({
      key: ttkey,
      computeBestOrder: true,
      routeRepresentation: "polyline",
      routeType:"fastest",
      traffic: true,
      travelMode:"car",
      locations:[
        [fromLat,fromLng],
        [toLat,toLng]
      ]
    });
  }

  zoomMarkerHandler() {

  }

  navigateToPark() {

    this.router.navigate(["park/parkingLotId"]);
  }

}
