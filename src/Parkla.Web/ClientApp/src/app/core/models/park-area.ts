import { ParkSpace } from "./park-space";
import { ParkSpaceStatus } from "./park-space-status";
import { Price } from "./price";
import { Pricing } from "./pricing";

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
  pricings?: Pricing[];
}
