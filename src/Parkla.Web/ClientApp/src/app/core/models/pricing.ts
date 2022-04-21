import { TimeUnit } from "../enums/TimeUnit";

export interface Pricing {
  type: string;
  timeUnit: TimeUnit;
  timeAmount: number;
  price: number;
}
