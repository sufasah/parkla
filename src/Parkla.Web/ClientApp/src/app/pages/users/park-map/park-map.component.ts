import { AfterViewInit, ChangeDetectorRef, Component, ElementRef, OnInit, Renderer2, ViewChild, ViewContainerRef } from '@angular/core';
import { FullscreenControl, GeolocateControl, LngLat, Map, map, Marker, NavigationControl, PointLike, Popup } from "@tomtom-international/web-sdk-maps";
import { services } from "@tomtom-international/web-sdk-services";
import SearchBox, { } from "@tomtom-international/web-sdk-plugin-searchbox";
import { ttkey } from '@app/core/constants/private.const';
import { ConfirmationService } from 'primeng/api';
import { ParkingLot } from '@app/core/models/parking-lot';
import { Router } from '@angular/router';
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
  parkingLots = <ParkingLot[]>[
    {
      id: "123Ade3",
      name: "xotopar",
      lat: 41.01293504282219,
      lng: 28.95420994690147,
      status: {
        timestamp: Date.now(),
        emptySpace: 20,
        occupiedSpace: 5,
        reservedSpace: 4,
      }
    },
    {
      id: "123Ade3",
      name: "xotopar",
      lat: 41.01293504282219,
      lng: 28.95420994690147,
      status: {
        timestamp: Date.now(),
        emptySpace: 20,
        occupiedSpace: 5,
        reservedSpace: 4,
      }
    },
    {
      id: "123Ade3",
      name: "xotopar",
      lat: 41.01746840384831,
      lng: 28.933095597536038,
      status: {
        timestamp: Date.now(),
        emptySpace: 20,
        occupiedSpace: 5,
        reservedSpace: 4,
      }
    }
    ,
    {
      id: "123Ade3",
      name: "xotopar",
      lat: 41.024850638335835,
      lng: 29.01257481506545,
      status: {
        timestamp: Date.now(),
        emptySpace: 20,
        occupiedSpace: 5,
        reservedSpace: 4,
      }
    }
    ,
    {
      id: "123Ade3",
      name: "xotopar",
      lat: 40.99648279881782,
      lng:  29.05291523864898,
      status: {
        timestamp: Date.now(),
        emptySpace: 20,
        occupiedSpace: 5,
        reservedSpace: 4,
      }
    }
    ,
    {
      id: "123Ade3",
      name: "xotopar",
      lat: 40.97536090806648,
      lng:  28.978757523805115,
      status: {
        timestamp: Date.now(),
        emptySpace: 20,
        occupiedSpace: 5,
        reservedSpace: 4,
      }
    }
    ,
    {
      id: "123Ade3",
      name: "xotopar",
      lat: 41.00490375179632,
      lng:  28.93652882507385,
      status: {
        timestamp: Date.now(),
        emptySpace: 20,
        occupiedSpace: 5,
        reservedSpace: 4,
      }
    }
    ,
    {
      id: "123Ade3",
      name: "xotopar",
      lat: 41.04738076198703,
      lng: 29.02115788391214,
      status: {
        timestamp: Date.now(),
        emptySpace: 20,
        occupiedSpace: 5,
        reservedSpace: 4,
      }
    }
    ,
    {
      id: "123Ade3",
      name: "xotopar",
      lat: 41.02446212030708,
      lng: 29.072312974243374,
      status: {
        timestamp: Date.now(),
        emptySpace: 20,
        occupiedSpace: 5,
        reservedSpace: 4,
      }
    }
  ]

  dialogVisible = false;
  constructor(
    private renderer: Renderer2,
    private changeDetector: ChangeDetectorRef,
    private router: Router) {

  }

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    this.loadMap();

    this.appMap?.on("click", (e) => console.log(e.lngLat));
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
      .appendChild(this.appSearchBox!.getSearchBoxHTML());
    let apmp = this.appMap;

    apmp.addControl(new GeolocateControl({
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

  makeMarkerElement(lat: number, lng: number): HTMLElement {
    let result:HTMLDivElement = this.renderer.createElement("div");
    this.renderer.addClass(result,"marker");
    this.renderer.listen(result,"click",()=>this.markerOnClick());

    let info: HTMLDivElement = this.renderer.createElement("div");
    this.renderer.addClass(info,"marker-info");

    this.renderer.appendChild(result,info);

    return result;
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
