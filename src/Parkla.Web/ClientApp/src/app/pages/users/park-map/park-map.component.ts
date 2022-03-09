import { AfterViewInit, ApplicationRef, ChangeDetectorRef, Component, ComponentRef, ElementRef, EmbeddedViewRef, OnInit, Renderer2, ViewChild, ViewContainerRef } from '@angular/core';
import { CircleLayer, FullscreenControl, GeoJSONSource, GeolocateControl, LngLat, Map, map, Marker, NavigationControl, PointLike, Popup, SymbolLayer,  } from "@tomtom-international/web-sdk-maps";
import { services } from "@tomtom-international/web-sdk-services";
import SearchBox, { } from "@tomtom-international/web-sdk-plugin-searchbox";
import { ttkey } from '@app/core/constants/private.const';
import { Router } from '@angular/router';
import { mockParkingLots } from '@app/mock-data/parking-lots';
import { MapMarkerComponent } from '@app/shared/components/map-marker/map-marker.component';
import { Feature, FeatureCollection } from 'geojson';
import { ParkingLot } from '@app/core/models/parking-lot';
import { clusterCircleLayer, clusterCircleLayerId, clusterSourceId, clusterSymbolLayer} from '@app/core/constants/map.const';
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

  markersOnTheMap: {[key:number]:Marker} = {};

  selectedPark: ParkingLot | null = null;

  get spaceCount() {
    return this.selectedPark!.status.emptySpace +
      this.selectedPark!.status.reservedSpace +
      this.selectedPark!.status.occupiedSpace
  }

  constructor(
    private viewRef: ViewContainerRef,
    private router: Router) {

  }

  ngOnInit(): void {
    this.parkingLots.forEach(el => {
      this.markersOnTheMap[el.id] = this.makeMarker(el);
    });
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
          features: this.parkingLots.map(park => (<Feature>{
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
  }

  addSearchBoxToMap() {
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
  }

  markerOnClick(event:any, element: MapMarkerComponent) {
    this.selectedPark = element.park;
    this.dialogVisible = true;
  }

  navigateGoogleMaps(lat: number, lng: number) {
    window.location.href = `https://www.google.com/maps/place/${lat.toFixed(20)}+${lng.toFixed(20)}/@${lat.toFixed(20)},${lng.toFixed(20)},12z`;
  }

  navigateToPark() {
    this.router.navigate(["park/parkingLotId"]);
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
