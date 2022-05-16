import { Pricing } from "./pricing";

export interface Reservation {
  id: number;
  parkId: number;
  parkName: string;
  areaName: string;
  pricing: Pricing //name and pricing
  timeFrom: Date;
  timeTo: Date;
}
