import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FullscreenControl, GeolocateControl, LngLat, Map, map, Marker, NavigationControl, PointLike, Popup } from "@tomtom-international/web-sdk-maps";
import { services } from "@tomtom-international/web-sdk-services";
import SearchBox, { } from "@tomtom-international/web-sdk-plugin-searchbox";
import { ttkey } from '@app/core/constants/private.const';
@Component({
  selector: 'app-park-map',
  templateUrl: './park-map.component.html',
  styleUrls: ['./park-map.component.scss']
})
export class ParkMapComponent implements OnInit, AfterViewInit {

  @ViewChild("appSearchBox")
  appSearchBoxRef!: ElementRef<HTMLElement>;

  appMap?: Map;
  appSearchBox?: SearchBox;
  key = ""
  parkingLots = [

  ]

  constructor() { }

  ngOnInit(): void {

  }

  ngAfterViewInit(): void {
    this.loadMap();
    this.appSearchBoxRef.nativeElement
      .appendChild(this.appSearchBox!.getSearchBoxHTML());
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

    })

    let apmp = this.appMap;

    apmp.addControl(new GeolocateControl({
      trackUserLocation: true,
      showUserLocation: true,
      positionOptions: {
        enableHighAccuracy: true,
      },
      showAccuracyCircle: true
    }))
    function makePopup(lat:number,lon:number){

      var markerHeight = 50, markerRadius = 10, linearOffset = 25;
      var popupOffsets: {[key:string]:PointLike} = {
        'top': [0, 0],
        'top-left': [0,0],
        'top-right': [0,0],
        'bottom': [0, -markerHeight],
        'bottom-left': [linearOffset, (markerHeight - markerRadius + linearOffset) * -1],
        'bottom-right': [-linearOffset, (markerHeight - markerRadius + linearOffset) * -1],
        'left': [markerRadius, (markerHeight - markerRadius) * -1],
        'right': [-markerRadius, (markerHeight - markerRadius) * -1]
      };
      return new Popup({offset: popupOffsets, className: 'my-class'})
      .setLngLat(new LngLat(lat,lon))
      .setHTML("<h1>Hello I'm a Popup!</h1>")
      .addTo(apmp);
    };

    var customelem: HTMLElement = document.createElement("div");
    customelem.style.width= "10px";
    customelem.style.height= "200px";
    customelem.style.backgroundColor= "green";
    customelem.style.cursor= "pointer";

    let markers = [
      new Marker(customelem)
      .setLngLat({lat: 41, lng: 29})
      .setPopup(makePopup(41,29))
      .addTo(this.appMap),
      new Marker()
      .setLngLat({lat: 41.001, lng: 28.999})
      .setPopup(makePopup(41.001,28.999))
      .addTo(this.appMap),
      new Marker()
      .setLngLat({lat: 41.014, lng: 28.990})
      .setPopup(makePopup(41.014,28.990))
      .addTo(this.appMap),
      new Marker()
      .setLngLat({lat: 41.020, lng: 28.985})
      .addTo(this.appMap),
      new Marker()
      .setLngLat({lat: 41.030, lng: 28.980})
      .addTo(this.appMap)
    ];

  }

  makeMarkerElement() {

  }

  makePopupElement() {

  }

  makeMarker() {

  }

  markerOnClick() {

  }

  makePopup() {

  }

  navigateGoogleMaps() {

  }

  routeParkSelection() {

  }

  zoomMarkerHandler() {

  }

}
