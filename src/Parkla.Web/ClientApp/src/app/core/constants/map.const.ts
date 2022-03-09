import { CircleLayer, SymbolLayer } from "@tomtom-international/web-sdk-maps";

export const clusterSourceId = "park-points";

export const clusterCircleLayerId = "park-clusters";

export const clusterCircleLayer: CircleLayer = {
  id: clusterCircleLayerId,
  type: "circle",
  source: clusterSourceId,
  filter: ["has", "point_count"],
  paint: {
    'circle-color': [
      'step',
      ['get', 'point_count'],
      '#EC619F',
      5,
      '#008D8D',
      10,
      '#004B7F'
    ],
    'circle-radius': [
        'step',
        ['get', 'point_count'],
        25,
        5,
        30,
        10,
        35
    ],
    'circle-stroke-width': 1,
    'circle-stroke-color': 'white',
    'circle-stroke-opacity': 1
  }
};

export const clusterSymbolLayer: SymbolLayer = {
  id: 'cluster-count',
  type: 'symbol',
  source: clusterSourceId,
  filter: ['has', 'point_count'],
  layout: {
      'text-field': '{point_count_abbreviated}',
      'text-size': 16
  },
  paint: {
      'text-color': 'white'
  }
};
