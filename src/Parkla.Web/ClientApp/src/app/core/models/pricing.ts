export interface Pricing {
  type: string;
  timeUnit: "minutes" | "hours" | "days" | "months";
  timeAmount: number;
  price: number;
}
