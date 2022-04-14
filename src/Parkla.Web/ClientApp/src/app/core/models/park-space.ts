import { Point } from "./parking-lot";
import { SpaceReservation } from "./space-reservation";

export interface ParkSpace {
  id: string;
  status: "empty" | "occupied";
  templatePath: SpacePath;
  isReserved: boolean;
  reservations: SpaceReservation[];
}

export type SpacePath = [Point,Point,Point,Point];
