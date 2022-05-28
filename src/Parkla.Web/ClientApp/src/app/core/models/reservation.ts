import { AppUser } from "./app-user";
import { ParkSpace } from "./park-space";

export interface Reservation {
  id: number;
  userId: number;
  user: AppUser;
  spaceId: number;
  space: ParkSpace;
  startTime: Date;
  endTime: Date;
}
