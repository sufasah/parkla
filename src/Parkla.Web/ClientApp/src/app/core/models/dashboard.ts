import { AppUser } from "./app-user";
import { Park } from "./park"

export interface Dashboard {
  topPopularParks: Park[];
  totalParks: number;
  minParkPrice: number | null;
  averageParkPrice: number | null;
  maxParkPrice: number | null;
  totalEmptySpaces: number;
  totalOccupiedSpaces: number;
  totalAreas: number;
  totalReservationEnabledAreas: number;
  totalSpaces: number;
  totalReservedSpaces: number;
  totalReservations: number;
  totalEarning: number;
  mostActiveUsers: AppUser[];
  minStatusDataTransferDelayInSeconds: number;
  averageStatusDataTransferDelayInSeconds: number;
  maxStatusDataTransferDelayInSeconds: number;
  totalCarsUsedSpaces: number;
}
