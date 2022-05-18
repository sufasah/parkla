import { SpaceStatus } from "../enums/SpaceStatus";

export interface ParkSpaceReal {
  id: number;
  parkid: string;
  areaid: number;
  spaceid?: number;
  name: string;
  statusUpdateTime: number;
  status: SpaceStatus;
  xmin: number;
};
