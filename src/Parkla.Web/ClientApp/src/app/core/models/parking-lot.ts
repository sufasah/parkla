import { ParkArea } from "./park-area";
import { ParkSpaceStatus } from "./park-space-status";
import { Price } from "./price";

export interface ParkingLot {
  id: number;
  name: string;
  lat: number;
  lng: number;
  location: string;
  status: ParkSpaceStatus;
  areas: ParkArea[];
  minPrice: Price;
  maxPrice: Price;
  avgPrice: Price;
  extras: string[];
}

export type Point = [number,number];
