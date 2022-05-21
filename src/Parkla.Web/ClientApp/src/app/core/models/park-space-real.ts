import { SpaceStatus } from "../enums/SpaceStatus";

export interface ParkSpaceReal {
  id: number;
  parkid: string;
  areaid: number;
  spaceId?: number;
  name: string;
  statusUpdateTime: Date;
  status: SpaceStatus;
  xmin: number;
};
