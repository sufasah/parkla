import { FullscreenControl, GeolocateControl, map, NavigationControl } from "@tomtom-international/web-sdk-maps";
import { ttkey } from "../constants/private";

export const makeTomTomMap = (container: string) => {
  let rmap = map({
    key: ttkey,
    container: container,
    zoom: 12,
    language: "tr-TR",
    center: {
      lat: 41.015137,
      lng: 28.979530,
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
