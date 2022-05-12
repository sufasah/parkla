import { AfterViewInit, Component, EmbeddedViewRef, EventEmitter, Input, OnDestroy, OnInit, Output, ViewContainerRef } from '@angular/core';
import { ttkey } from '@app/core/constants/private';
import { Park } from '@app/core/models/park';
import { MapMarkerComponent } from '@app/shared/components/map-marker/map-marker.component';
import { Feature, FeatureCollection } from 'geojson';
import { clusterCircleLayer, clusterCircleLayerId, clusterSourceId, clusterSymbolLayer} from '@app/core/constants/map';
import { FullscreenControl, GeoJSONSource, GeolocateControl, Map, map, Marker, NavigationControl,  } from "@tomtom-international/web-sdk-maps";
import { services } from "@tomtom-international/web-sdk-services";
import SearchBox from '@tomtom-international/web-sdk-plugin-searchbox';
import { ParkService } from '@app/core/services/park.service';

@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.scss']
})
export class MapComponent implements OnInit, AfterViewInit, OnDestroy{

  @Input()
  searchBoxParent?: HTMLElement;

  searchBox?: SearchBox;

  appMap!: Map;

  private _parks: Park[] = [];

  @Input()
  set parks(value: Park[]) {
    this._parks = value;

    this._parks.forEach(el => {
      this.markersOnTheMap[el.id] = this.makeMarker(el);
    });
  }

  get parks() {
    return this._parks;
  }

  @Input()
  showUserParks = false;

  @Output()
  markerOnClick = new EventEmitter<{event:any; element:MapMarkerComponent}>();

  markersOnTheMap: {[key:number]:Marker} = {};

  searchMarker?: Marker;

  featureCollection = <FeatureCollection>{
    type: "FeatureCollection",
    features: []
  };

  setFatureIntervalId?: NodeJS.Timer;

  constructor(
    private viewRef: ViewContainerRef,
    private parkService: ParkService
  ) { }

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

  setFeatureCollection() {
    let parks = (this.showUserParks ? this.parkService.userParks : this.parkService.allParks);
    let values = Object.values(parks);

    Object.values(this.markersOnTheMap).forEach(value => value.remove());
    this.markersOnTheMap = {};

    values.forEach(el => {
      this.markersOnTheMap[el.id] = this.makeMarker(el);
    });

    this.featureCollection.features = values.map(park => (<Feature>{
        id: park.id,
        type: "Feature",
        geometry: {
          type: "Point",
          coordinates: [park.longitude,park.latitude],
        },
        properties: {
          id: park.id
        }
    }));

    let source = <GeoJSONSource>this.appMap.getSource(clusterSourceId);
    source.setData(this.featureCollection);
  }

  addMarkerClusterToMap() {
    this.appMap.on("load", (e:any) => {
      this.appMap.addSource(clusterSourceId,{
        type:'geojson',
        data: this.featureCollection,
        cluster: true,
        clusterMaxZoom: 14,
        clusterRadius: 200,
      });

      this.appMap.getSource(clusterSourceId)

      this.appMap.addLayer(clusterCircleLayer);

      this.appMap.addLayer(clusterSymbolLayer);
    });

    this.appMap.on("sourcedata", (e:any) => {
      const source = (<any>this.appMap.getSource(clusterSourceId));
      if(e.sourceId == clusterSourceId && !source.loaded()) {
        this.setFatureIntervalId = setInterval(() => {
          this.setFeatureCollection();
        },3000);
      }
      if(e.sourceId !== clusterSourceId || !source.loaded()) return;
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

    this.appMap.on('move', (e:any) => {
      this.refreshMarkers()
    });
    this.appMap.on('moveend',(e:any) => {
      this.refreshMarkers()
    });
    this.appMap.on('click',(e:any) => {
      this.searchMarker?.remove()
    });
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
          lat: result.position.lat,
          lng: result.position.lng,
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

  makeMarkerElement(park: Park) {
    let componentRef = this.viewRef.createComponent(MapMarkerComponent);
    componentRef.instance.onClick
      .subscribe((event:any) => {
        this.markerOnClick.emit({
          event: event,
          element: componentRef.instance
        });
      });

    componentRef.instance.park = park;

    return (componentRef.hostView as EmbeddedViewRef<any>)
      .rootNodes[0] as HTMLElement;
  }

  makeMarker(park: Park) {
    let marker = new Marker(this.makeMarkerElement(park))
      .setLngLat({lat: park.latitude, lng: park.longitude})
      .remove()
    return marker;
  }

  refreshMarkers() {
    Object.values(this.markersOnTheMap).forEach((value: Marker) => {
      value.remove();
    });

    this.appMap.querySourceFeatures(clusterSourceId).forEach((feature: Feature) => {
      if (feature.properties && !feature.properties.cluster) {
        let id = parseInt(feature.properties.id, 10);
        let marker = this.markersOnTheMap[id];
        marker.addTo(this.appMap);
      }
    });
  }

  ngOnDestroy(): void {
    clearInterval(this.setFatureIntervalId!);
  }
}
