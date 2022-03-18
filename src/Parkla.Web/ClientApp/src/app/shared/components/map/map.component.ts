import { AfterViewInit, Component, EmbeddedViewRef, Input, OnInit, ViewContainerRef } from '@angular/core';
import { ttkey } from '@app/core/constants/private.const';
import { ParkingLot } from '@app/core/models/parking-lot';
import { mockParkingLots } from '@app/mock-data/parking-lots';
import { MapMarkerComponent } from '@app/shared/components/map-marker/map-marker.component';
import { Feature, FeatureCollection } from 'geojson';
import { clusterCircleLayer, clusterCircleLayerId, clusterSourceId, clusterSymbolLayer} from '@app/core/constants/map.const';
import { RSRoute } from '@app/core/constants/ref-sharing.const';
import { RouteUrl } from '@app/core/utils/route.util';
import { FullscreenControl, GeolocateControl, Map, map, Marker, NavigationControl,  } from "@tomtom-international/web-sdk-maps";
import { services } from "@tomtom-international/web-sdk-services";
import { RefSharingService } from '@app/core/services/ref-sharing.service';
import { Router } from '@angular/router';
import SearchBox from '@tomtom-international/web-sdk-plugin-searchbox';

@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.scss']
})
export class MapComponent implements OnInit, AfterViewInit{

  @Input()
  searchBoxParent?: HTMLElement;

  searchBox?: SearchBox;

  appMap!: Map;

  private _parks: ParkingLot[] = [];

  @Input()
  set parks(value: ParkingLot[]) {
    this._parks = value;

    this._parks.forEach(el => {
      this.markersOnTheMap[el.id] = this.makeMarker(el);
    });
  }

  get parks() {
    return this._parks;
  }

  dialogVisible = false;

  markersOnTheMap: {[key:number]:Marker} = {};

  selectedPark: ParkingLot | null = null;

  searchMarker?: Marker;

  get spaceCount() {
    return this.selectedPark!.status.emptySpace +
      this.selectedPark!.status.reservedSpace +
      this.selectedPark!.status.occupiedSpace
  }

  constructor(
    private refSharingService: RefSharingService,
    private router: Router,
    private viewRef: ViewContainerRef) { }

  ngOnInit(): void {

  }

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.loadMap();
    }, 0);
  }

  loadMap() {
    this.appMap = map({
      key: ttkey,
      container: "appMap",
      zoom: 12,
      language: "tr-TR",
      center: {
        lat: 41.015137,
        lng: 28.979530,
      },
    });

    this.appMap.addControl(new NavigationControl());
    this.appMap.addControl(new FullscreenControl());
    this.appMap.addControl(new GeolocateControl({
      trackUserLocation: true,
      showUserLocation: true,
      positionOptions: {
        enableHighAccuracy: true,
      },
      showAccuracyCircle: true
    }));


    this.addSearchBoxToMap();

    this.addMarkerClusterToMap();
  }

  addMarkerClusterToMap() {
    this.appMap.on("load", (e:any) => {
      this.appMap.addSource(clusterSourceId,{
        type:'geojson',
        data: <FeatureCollection>{
          type: "FeatureCollection",
          features: this.parks.map(park => (<Feature>{
            id: park.id,
            type: "Feature",
            geometry: {
              type: "Point",
              coordinates: [park.lng,park.lat],
            },
            properties: {
              ...park
            }
          }))
        },
        cluster: true,
        clusterMaxZoom: 14,
        clusterRadius: 200,
      });

      this.appMap.addLayer(clusterCircleLayer);

      this.appMap.addLayer(clusterSymbolLayer);
    });

    this.appMap.on("data", (e:any) => {
      if(e.sourceId !== clusterSourceId || !(<any>this.appMap.getSource(clusterSourceId)).loaded()) return;
      this.refreshMarkers();
    });

    this.appMap.on('click', clusterCircleLayerId, (e) => {
      var features:any = this.appMap.queryRenderedFeatures(e.point, { layers: [clusterCircleLayerId] });
      var clusterId = features[0].properties.cluster_id;

      (<any>this.appMap.getSource(clusterSourceId)).getClusterExpansionZoom(clusterId, (err:any, zoom:any) => {
        if (err) return;
        this.appMap.easeTo({
            center: features[0].geometry.coordinates,
            zoom: zoom + 0.5
        });
      });
    });

    this.appMap.on('mouseenter', clusterCircleLayerId, () => {
      this.appMap.getCanvas().style.cursor = 'pointer';
    });

    this.appMap.on('mouseleave', clusterCircleLayerId, () => {
      this.appMap.getCanvas().style.cursor = '';
    });

    this.appMap.on('move', (e:any) => this.refreshMarkers());
    this.appMap.on('moveend',(e:any) => this.refreshMarkers());
    this.appMap.on('click',(e:any) => this.searchMarker?.remove())
  }

  addSearchBoxToMap() {
    this.searchBox = new SearchBox(services, {
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
        noResultsMessage: "Where are you looking for is not found.",
        placeholder: "Search an address"
      }
    });

    this.searchBox.on("tomtom.searchbox.resultselected",(e)=> {
      let result:any = e.data.result;
      if(!result.position) return;

      this.appMap?.easeTo({
        center: {
          lat:result.position.lat,
          lng:result.position.lng,
        },
        animate: true,
        duration: 1500,
        zoom: 16
      });

      this.searchMarker?.remove();

      this.searchMarker = new Marker({})
        .setLngLat([result.position.lng,result.position.lat])
        .addTo(this.appMap);
    })

    this.searchBoxParent?.appendChild(this.searchBox.getSearchBoxHTML());
  }

  makeMarkerElement(park: ParkingLot) {
    let componentRef = this.viewRef.createComponent(MapMarkerComponent);

    componentRef.instance.onClick
      .subscribe((event:any) => this.markerOnClick(event, componentRef.instance));

    componentRef.instance.park = park;

    return (componentRef.hostView as EmbeddedViewRef<any>)
      .rootNodes[0] as HTMLElement;
  }

  makeMarker(park: ParkingLot) {
    return new Marker(this.makeMarkerElement(park))
      .setLngLat({lat: park.lat, lng: park.lng})
      .remove()
  }

  markerOnClick(event:any, element: MapMarkerComponent) {
    this.selectedPark = element.park;
    this.dialogVisible = true;
  }

  navigateGoogleMaps(lat: number, lng: number) {
    window.location.href = `https://www.google.com/maps/place/${lat.toFixed(20)}+${lng.toFixed(20)}/@${lat.toFixed(20)},${lng.toFixed(20)},12z`;
  }

  navigateToParkAreas(park:ParkingLot) {
    this.refSharingService.setData(RSRoute.mapSelectedPark,park);
    this.router.navigate([RouteUrl.parkAreas(park.id)]);
  }

  refreshMarkers() {
    Object.keys(this.markersOnTheMap).forEach((id:any) => {
      this.markersOnTheMap[id].remove();
    });

    this.appMap.querySourceFeatures(clusterSourceId).forEach((feature:any) => {
      if (feature.properties && !feature.properties.cluster) {
        let id = parseInt(feature.properties.id, 10);
        let marker = this.markersOnTheMap[id];
        marker.addTo(this.appMap);
      }
    });
  }

}
