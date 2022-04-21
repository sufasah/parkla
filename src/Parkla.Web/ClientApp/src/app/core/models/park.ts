import { ParkArea } from "./park-area";
import { ParkAreaStatus } from "./park-area-status";

export interface Park {
  id: number;
  name: string;
  lat: number;
  lng: number;
  location: string;
  status: ParkAreaStatus;
  areas: ParkArea[];
  minPrice: number;
  maxPrice: number;
  avgPrice: number;
  extras: string[];
}
