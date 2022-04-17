import { ParkSpace } from "@app/core/models/park-space";
import { mockSpaceReservations } from "./spaceReservations";

export const mockSpaces: ParkSpace[] = [
  {
    id: 1,
    name: "178",
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
    id: 2,
    name: "177",
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
    id: 3,
    name: "181",
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
    id: 4,
    name: "184",
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
    id: 5,
    name: "183",
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
