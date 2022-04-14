import { Price } from "./price";

export interface Pricing {
  type: string;
  timeUnit: "minutes" | "hours" | "days" | "months";
  timeAmount: number;
  price: Price;
}
