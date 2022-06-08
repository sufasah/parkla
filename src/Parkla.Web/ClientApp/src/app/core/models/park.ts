import { Subject } from "rxjs";
import { AppUser } from "./app-user";
import { ParkArea } from "./park-area";
export interface Park {
  id: string;
  user: AppUser,
  name: string;
  latitude: number;
  longitude: number;
  location: string;
  extras: string[];
  statusUpdateTime: Date;
  emptySpace: number;
  reservedSpace: number;
  occupiedSpace: number;
  minPrice: number;
  averagePrice: number;
  maxPrice: number;
  areas: ParkArea[];
  xmin: number;
}

export type ChangablePark =  {subject: Subject<void>} & Park;
