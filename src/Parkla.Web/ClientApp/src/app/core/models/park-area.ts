import { ParkSpace } from "./park-space";
import { Pricing } from "./pricing";

export interface ParkArea {
  id: number;
  parkId: string;
  name: string;
  description?: string;
  templateImage: string | null;
  reservationsEnabled: boolean;
  statusUpdateTime: Date;
  emptySpace: number;
  reservedSpace: number;
  occupiedSpace: number;
  spaces: ParkSpace[];
  pricings: Pricing[];
  minPrice: number;
  avaragePrice: number;
  maxPrice: number;
  xmin: number;
}
