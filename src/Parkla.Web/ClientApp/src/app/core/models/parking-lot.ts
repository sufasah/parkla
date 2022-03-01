export interface ParkingLot {
  id: string;
  name: string;
  longitude: number;
  latitude: number;
  location: string;
  status: ParkingLotStatus;
  extras: [string];
  pricing: [ParkSpan | ParkSpanPerTime];
}

export interface ParkingLotStatus {
  timestamp: number;
  freeSpace: number;
  reservedSpace: number;
  occupiedSpace: number;
}

export interface Price {
  type: string;
  amount: number;
}

export interface ParkSpan {
  beginning: string;
  end: string;
  type: string;
  price: Price;
}

export interface ParkSpanPerTime {
  timeUnit: "minutes" | "hours" | "days" | "months";
  amount: number;
  price: Price;
}
