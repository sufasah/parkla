import { AfterViewInit, Component, ComponentRef, EmbeddedViewRef, EventEmitter, Input, OnDestroy, OnInit, Output, ViewContainerRef } from '@angular/core';
import { ttkey } from '@app/core/constants/private';
import { ChangablePark, Park } from '@app/core/models/park';
import { MapMarkerComponent } from '@app/shared/components/map-marker/map-marker.component';
import { Feature, FeatureCollection } from 'geojson';
import { clusterCircleLayer, clusterCircleLayerId, clusterSourceId, clusterSymbolLayer} from '@app/core/constants/map';
import { FullscreenControl, GeoJSONFeature, GeoJSONSource, GeoJSONSourceRaw, GeolocateControl, Map, map, Marker, NavigationControl, TTEvent,  } from "@tomtom-international/web-sdk-maps";
import { services } from "@tomtom-international/web-sdk-services";
import SearchBox from '@tomtom-international/web-sdk-plugin-searchbox';
import { InformerItem, ParkService } from '@app/core/services/park.service';
import { Subscription } from 'rxjs';
import { makeTomTomMap } from '@app/core/utils/tomtom';

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
  markerOnClick = new EventEmitter<{event:any; changablePark: ChangablePark}>();

  markersOnTheMap = new Map<string, MapMarker>();
  searchMarker?: Marker;

  featureCollection = <FeatureCollection>{
    type: "FeatureCollection",
    features: []
  };

  eventListenersAdded = false;
  featureCollectionChanged = false;

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
    }, 0);
  }

  removeExistingPark(data: InformerItem) {
    const mapMarker = this.markersOnTheMap.get(data.park.id)!;

    mapMarker.marker.remove();
    mapMarker.subscription.unsubscribe();
    this.markersOnTheMap.delete(data.park.id);

    const featureIndex = this.featureCollection.features.indexOf(mapMarker.feature);
    this.featureCollection.features.splice(featureIndex,1);
    this.featureCollectionChanged = true;
  }

  addNewPark(data: InformerItem) {
    const mapMarker: MapMarker = {
      ...this.makeMarker(data.park),
      subscription: data.park.subject.subscribe(() => this.handleDataChanged(mapMarker)),
      feature: this.getFeature(data.park)
    };
    this.markersOnTheMap.set(data.park.id, mapMarker);
    this.featureCollection.features.push(mapMarker.feature)
    this.featureCollectionChanged = true;
  }

  handleDataChanged(mapMarker: MapMarker) {
    const curPark = mapMarker.component.instance.park;
    const newPark = mapMarker.park;

    if(newPark.latitude != curPark.latitude || newPark.longitude != curPark.longitude) {
      mapMarker.marker.remove();
      mapMarker.marker.setLngLat({lat: newPark.latitude, lng: newPark.longitude});
      (<any>mapMarker.feature.geometry).coordinates = [newPark.longitude, newPark.latitude];
      this.featureCollectionChanged = true;
    }

    mapMarker.component.instance.park = {...mapMarker.park};
    mapMarker.component.changeDetectorRef.detectChanges();
  }

  onMarkerClusterLoad() {
    setInterval(() => {
      if(this.featureCollectionChanged){
        this.setFeatureCollection();
        this.featureCollectionChanged = false;
      }
    },2000);

    let sub = this.parkService.parkInformer.subscribe((data) => {
      if(!this.isUserMap || true === data.isUserPark) {
        if(data.isDeleted) {
          this.removeExistingPark(data);
        }
        else {
          this.addNewPark(data);
        }
      }
    });

    this.parkService.copyParks.forEach((park) => {
      const data: InformerItem = {
        park: park,
        isDeleted: false,
        isUserPark: this.parkService.isUserPark(park)
      };

      if(!this.isUserMap || true === data.isUserPark) {
          this.addNewPark(data);
      }
    })

    this.unsubscribe.push(sub);
  }

  setFeatureCollection() {
    const source = <GeoJSONSource>this.appMap.getSource(clusterSourceId);
    source.setData(this.featureCollection);
  }

  loadMap() {
    this.appMap = makeTomTomMap("appMap");

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

      if(!e.isSourceLoaded) {
        source.load();
        return;
      }

      this.refreshMarkers();
      if (!this.eventListenersAdded) {
        this.eventListenersAdded = true;
        this.onMarkerClusterLoad();
        this.appMap.on('move',e => this.refreshMarkers());
        this.appMap.on('moveend',e => this.refreshMarkers);
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

  makeMarkerElement(park: ChangablePark): [HTMLElement, ComponentRef<MapMarkerComponent>] {
    let componentRef = this.viewRef.createComponent(MapMarkerComponent);
    componentRef.instance.onClick
      .subscribe((event:any) => {
        this.markerOnClick.emit({
          event: event,
          changablePark: park
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
    this.markersOnTheMap.forEach((mapMarker: MapMarker) => {
      mapMarker.marker.remove();
    });

    const sourceFeatures = this.appMap.querySourceFeatures(clusterSourceId);

    sourceFeatures.forEach((feature: Feature) => {
      if(feature.properties && feature.properties.parkPoint) {
        let id = feature.properties.id;
        let marker = this.markersOnTheMap.get(id)!.marker;
        marker.addTo(this.appMap);
      }
    });
  }

  ngOnDestroy(): void {
    this.unsubscribe.forEach(x => x.unsubscribe());
  }
}
