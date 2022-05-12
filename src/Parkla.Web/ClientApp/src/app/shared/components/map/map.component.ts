import { AfterViewInit, Component, ComponentRef, EmbeddedViewRef, EventEmitter, Input, OnDestroy, OnInit, Output, ViewContainerRef } from '@angular/core';
import { ttkey } from '@app/core/constants/private';
import { ChangablePark, Park } from '@app/core/models/park';
import { MapMarkerComponent } from '@app/shared/components/map-marker/map-marker.component';
import { Feature, FeatureCollection } from 'geojson';
import { clusterCircleLayer, clusterCircleLayerId, clusterSourceId, clusterSymbolLayer} from '@app/core/constants/map';
import { FullscreenControl, GeoJSONFeature, GeoJSONSource, GeoJSONSourceRaw, GeolocateControl, Map, map, Marker, NavigationControl,  } from "@tomtom-international/web-sdk-maps";
import { services } from "@tomtom-international/web-sdk-services";
import SearchBox from '@tomtom-international/web-sdk-plugin-searchbox';
import { ParkService } from '@app/core/services/park.service';
import { Subscription } from 'rxjs';

interface MapMarker {
  marker: Marker,
  component: ComponentRef<MapMarkerComponent>,
  park: ChangablePark,
  subscription: Subscription,
  feature: Feature
}

@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.scss']
})
export class MapComponent implements OnInit, AfterViewInit, OnDestroy{

  @Input()
  searchBoxParent?: HTMLElement;

  @Input()
  isUserMap = false;

  searchBox?: SearchBox;
  appMap!: Map;

  @Output()
  markerOnClick = new EventEmitter<{event:any; element:MapMarkerComponent}>();

  markersOnTheMap: {[key:number]: MapMarker}= {};
  searchMarker?: Marker;

  featureCollection = <FeatureCollection>{
    type: "FeatureCollection",
    features: []
  };

  eventListenersAdded = false;
  markerClusterLoaded = false;

  unsubscribe: Subscription[] = [];

  constructor(
    private viewRef: ViewContainerRef,
    private parkService: ParkService
  ) {

  }


  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.loadMap();

      let sub = this.parkService.parkInformer.subscribe((data) => {
        if(!this.isUserMap || true === data.isUserPark) {
          if(data.isDeleted) {
            const mapMarker = this.markersOnTheMap[data.park.id];

            const featureIndex = this.featureCollection.features.indexOf(mapMarker.feature);
            this.featureCollection.features.splice(featureIndex,1);

            mapMarker.subscription.unsubscribe();
            delete this.markersOnTheMap[data.park.id];
          }
          else {
            const mapMarker: MapMarker = {
              ...this.makeMarker(data.park),
              subscription: data.park.subject.subscribe(() => this.handleDataChanged(mapMarker)),
              feature: this.getFeature(data.park)
            };
            this.markersOnTheMap[data.park.id] = mapMarker;
            this.featureCollection.features.push(mapMarker.feature)
          }
        }
      });
      this.unsubscribe.push(sub);
    }, 0);
  }

  handleDataChanged(mapMarker: MapMarker) {
    const curPark = mapMarker.component.instance.park;
    const newPark = mapMarker.park;

    if(newPark.latitude != curPark.latitude || newPark.longitude != curPark.longitude) {
      mapMarker.marker.remove();
      mapMarker.marker.setLngLat({lat: newPark.latitude, lng: newPark.longitude});
      (<any>mapMarker.feature.geometry).coordinates = [newPark.longitude, newPark.latitude];
      mapMarker.marker.addTo(this.appMap);
    }

    mapMarker.component.instance.park = {...mapMarker.park};
    mapMarker.component.changeDetectorRef.detectChanges();
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

  getFeature(park: ChangablePark) {
    return <Feature>{
        id: park.id,
        type: "Feature",
        geometry: {
          type: "Point",
          coordinates: [park.longitude, park.latitude],
        },
        properties: {
          id: park.id,
          parkPoint: true
        }
    }
  }

  addMarkerClusterToMap() {
    this.appMap.on("load", (e:any) => {
      this.appMap.addSource(clusterSourceId, {
        type:'geojson',
        data: this.featureCollection,
        cluster: true,
        clusterMaxZoom: 14,
        clusterRadius: 200,
      });

      this.appMap.addLayer(clusterCircleLayer);

      this.appMap.addLayer(clusterSymbolLayer);
    });

    this.appMap.on("sourcedata", (e:any) => {
      if(e.sourceId !== clusterSourceId) return;

      const source = <any>this.appMap.getSource(clusterSourceId);

      if(!source.loaded()) {
        /*this.setFeatures();
        this.markerClusterLoaded = true;*/
        return;
      };

      this.refreshMarkers();
      if (!this.eventListenersAdded) {
        this.appMap.on('move',e => this.refreshMarkers());
        this.appMap.on('moveend',e => this.refreshMarkers);
        this.eventListenersAdded = true;
      }
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

  makeMarkerElement(park: Park): [HTMLElement, ComponentRef<MapMarkerComponent>] {
    let componentRef = this.viewRef.createComponent(MapMarkerComponent);
    componentRef.instance.onClick
      .subscribe((event:any) => {
        this.markerOnClick.emit({
          event: event,
          element: componentRef.instance
        });
      });

    componentRef.instance.park = {...park};

    return [
      (componentRef.hostView as EmbeddedViewRef<any>).rootNodes[0] as HTMLElement,
      componentRef
    ];
  }

  makeMarker(park: ChangablePark) {
    let [markerElement, component] = this.makeMarkerElement(park);
    let marker = new Marker(markerElement)
      .setLngLat({lat: park.latitude, lng: park.longitude})
      .remove()

    return {
      marker: marker,
      component: component,
      park: park
    };
  }

  refreshMarkers() {
    Object.values(this.markersOnTheMap).forEach((mapMarker: MapMarker) => {
      mapMarker.marker.remove();
    });

    const sourceFeatures = this.appMap.querySourceFeatures(clusterSourceId);

    sourceFeatures.forEach((feature: Feature) => {
      if (feature.properties && feature.properties.parkPoint) {
        let id = parseInt(feature.properties.id, 10);
        let marker = this.markersOnTheMap[id].marker;
        marker.addTo(this.appMap);
      }
    });
  }

  ngOnDestroy(): void {
    this.unsubscribe.forEach(x => x.unsubscribe());
  }
}
