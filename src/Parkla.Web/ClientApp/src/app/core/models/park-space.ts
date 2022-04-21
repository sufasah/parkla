import { SpaceStatus } from "../enums/SpaceStatus";
import { Point } from "../types/parkmap";
import { ParkSpaceReal } from "./park-space-real";
import { SpaceReservation } from "./space-reservation";

export interface ParkSpace {
  id: number;
  name: string;
  realSpace?: ParkSpaceReal;
  status: SpaceStatus;
  templatePath: SpacePath;
  isReserved: boolean;
  reservations: SpaceReservation[];
}

export type SpacePath = [Point,Point,Point,Point];
