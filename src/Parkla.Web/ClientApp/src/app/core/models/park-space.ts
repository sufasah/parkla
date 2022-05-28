import { SpaceStatus } from "../enums/SpaceStatus";
import { Point } from "../types/parkmap";
import { ParkSpaceReal } from "./park-space-real";
import { Pricing } from "./pricing";
import { SpaceReservation } from "./space-reservation";

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
  reservations: SpaceReservation[];
  xmin: number;
}

export type SpacePath = [Point,Point,Point,Point];
