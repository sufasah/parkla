import { SpaceStatus } from "../enums/SpaceStatus";
import { Point } from "../types/parkmap";
import { ParkSpaceReal } from "./park-space-real";
import { Pricing } from "./pricing";
import { Reservation } from "./reservation";

export interface ParkSpace {
  id: number;
  areaId: number;
  name: string;
  realSpaceId: number;
  realSpace?: ParkSpaceReal;
  pricingId: number;
  pricing: Pricing;
  statusUpdateTime: Date;
  status: SpaceStatus;
  templatePath: SpacePath;
  isReserved: boolean;
  reservations: Reservation[];
  xmin: number;
}

export type SpacePath = [Point,Point,Point,Point];
