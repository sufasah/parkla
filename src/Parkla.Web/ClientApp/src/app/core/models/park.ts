import { ParkArea } from "./park-area";
export interface Park {
  id: number;
  name: string;
  lat: number;
  lng: number;
  location: string;
  extras: string[];
  statusUpdateTime: number;
  emptySpace: number;
  reservedSpace: number;
  occupiedSpace: number;
  minPrice: number;
  maxPrice: number;
  avgPrice: number;
  areas: ParkArea[];
}
