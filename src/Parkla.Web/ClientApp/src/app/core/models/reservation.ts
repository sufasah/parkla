import { ParkArea, ParkSpan, ParkSpanPerTime} from "@core/models/parking-lot";

export interface Reservation {
  parkId: number;
  parkName: string;
  areaName: string;
  pricing: ParkSpan | ParkSpanPerTime //name and pricing
  timeFrom: Date;
  timeTo: Date;
}
