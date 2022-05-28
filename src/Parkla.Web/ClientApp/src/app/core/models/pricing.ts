import { TimeUnit } from "../enums/TimeUnit";

export interface Pricing {
  id: number;
  areaId: number;
  type: string;
  unit: TimeUnit;
  amount: number;
  price: number;
}

export const getPricePerHour = (pricing: Pricing) => {
  const price = pricing.price;
  const amount = pricing.amount;
  switch (pricing.unit) {
    case "MINUTE":
      return price * 60 / amount;
    case "DAY":
      return price / 24 / amount;
    case "MONTH":
      return price / 720 / amount;
    default:
      return price / amount;
  }
};
