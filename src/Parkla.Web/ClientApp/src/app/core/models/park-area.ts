import { ParkSpace } from "./park-space";
import { ParkSpaceStatus } from "./park-space-status";
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
  minPrice: number;
  maxPrice: number;
  avgPrice: number;
  pricings?: Pricing[];
}
