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
  extras?: string[];
}

export interface ParkArea {
  id: number;
  name: string;
  description?: string;
  templateImg: string;
  reservationsEnabled: boolean;
  notReservedOccupiable: boolean;
  status: ParkSpaceStatus;
  spaces: ParkSpace[];
  minPrice: Price;
  maxPrice: Price;
  avgPrice: Price;
  pricing?: (ParkSpan | ParkSpanPerTime)[];
}

export interface ParkSpaceStatus {
  timestamp: number;
  emptySpace: number;
  reservedSpace: number;
  occupiedSpace: number;
}

export interface Price {
  moneyUnit: string;
  balance: number;
}

export interface ParkSpan {
  type: string;
  beginningTime: string;
  endTime: string;
  price: Price;
}

export interface ParkSpanPerTime {
  type: string;
  timeUnit: "minutes" | "hours" | "days" | "months";
  timeAmount: number;
  price: Price;
}

export type Point = [number,number];
export type SpacePath = [Point,Point,Point,Point];

export interface ParkSpace {
  id: string;
  status: "empty" | "occupied";
  templatePath: SpacePath
  reservations?: [{
    username: string;
    startTime: Date,
    endTime: Date
  }] | [];
}
