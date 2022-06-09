import { FullscreenControl, GeolocateControl, map, NavigationControl } from "@tomtom-international/web-sdk-maps";
import { ttkey } from "../constants/private";

export const TomTomDefaultLat = 41.015137;
export const TomTomDefaultLng = 28.979530;
export const TomTomDefaultZoom = 12;

export const makeTomTomMap = (container: string) => {
  let rmap = map({
    key: ttkey,
    container: container,
    zoom: TomTomDefaultZoom,
    language: "tr-TR",
    center: {
      lat: TomTomDefaultLat,
      lng: TomTomDefaultLng,
    },
  });

  rmap.addControl(new NavigationControl());
  rmap.addControl(new FullscreenControl());
  rmap.addControl(new GeolocateControl({
    trackUserLocation: true,
    showUserLocation: true,
    positionOptions: {
      enableHighAccuracy: true,
    },
    showAccuracyCircle: true
  }));

  return rmap;
};
