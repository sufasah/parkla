import { TimeUnit } from "../enums/TimeUnit";

export interface Pricing {
  id: number;
  areaId: number;
  type: string;
  unit: TimeUnit;
  amount: number;
  price: number;
}
