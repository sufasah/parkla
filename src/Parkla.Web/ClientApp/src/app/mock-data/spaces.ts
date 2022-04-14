import { ParkSpace } from "@app/core/models/park-space";
import { mockSpaceReservations } from "./spaceReservations";

export const mockSpaces: ParkSpace[] = [
  {
    id:"178",
    status: "empty",
    templatePath: [
      [165,21],
      [165,83],
      [197,83],
      [197,21]
    ],
    isReserved: false,
    reservations: mockSpaceReservations
  },
  {
    id:"177",
    status: "occupied",
    templatePath: [
      [133,21],
      [133,83],
      [165,83],
      [165,21]
    ],
    isReserved: false,
    reservations: mockSpaceReservations
  },
  {
    id:"181",
    status: "empty",
    templatePath: [
      [261,21],
      [261,84],
      [293,83],
      [294,21]
    ],
    isReserved: false,
    reservations: mockSpaceReservations
  },
  {
    id:"184",
    status: "occupied",
    templatePath: [
      [358,22],
      [358,84],
      [391,84],
      [391,21]
    ],
    isReserved: false,
    reservations: mockSpaceReservations
  },
  {
    id:"183",
    status: "empty",
    templatePath: [
      [326,21],
      [326,82],
      [358,83],
      [358,22]
    ],
    isReserved: false,
    reservations: mockSpaceReservations
  }
];
