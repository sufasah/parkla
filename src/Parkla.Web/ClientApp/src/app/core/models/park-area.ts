import { ParkSpace } from "./park-space";
import { ParkAreaStatus } from "./park-area-status";
import { Pricing } from "./pricing";

export interface ParkArea {
  id: number;
  name: string;
  description?: string;
  templateImg: string;
  reservationsEnabled: boolean;
  status: ParkAreaStatus;
  spaces: ParkSpace[];
  pricings: Pricing[];
  minPrice: number;
  maxPrice: number;
  avgPrice: number;
}
