import { ParkArea } from "./park-area";
import { ParkSpaceStatus } from "./park-space-status";

export interface ParkingLot {
  id: number;
  name: string;
  lat: number;
  lng: number;
  location: string;
  status: ParkSpaceStatus;
  areas: ParkArea[];
  minPrice: number;
  maxPrice: number;
  avgPrice: number;
  extras: string[];
}

export type Point = [number,number];
