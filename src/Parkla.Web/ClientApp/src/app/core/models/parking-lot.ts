export interface ParkingLot {
  id: string;
  name: string;
  lat: number;
  lng: number;
  location: string;
  status: ParkingLotStatus;
  areas: ParkArea[];
  extras?: [string];
  pricing?: [ParkSpan | ParkSpanPerTime];
}

export interface ParkArea {
  name: string;
  templateImg: string;
  reservationsEnabled: boolean;
  spaces: ParkSpace[];
}

export interface ParkingLotStatus {
  timestamp: number;
  emptySpace: number;
  reservedSpace: number;
  occupiedSpace: number;
}

export interface Price {
  moneyUnit: string;
  amount: number;
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
    startTime: Date,
    endTime: Date
  }] | [];
}
