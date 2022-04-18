import { ParkArea } from "@app/core/models/park-area";
import { mockSpaces } from "./spaces";

export const mockAreas: ParkArea[] = [{
    id: 1,
    name: "basement1",
    description: "description1 description1 description1 description1 description1 ",
    reservationsEnabled: true,
    notReservedOccupiable: true,
    status: {
      timestamp: Date.now(),
      emptySpace: 20,
      reservedSpace: 10,
      occupiedSpace: 5
    },
    spaces: mockSpaces,
    templateImg: "https://www.realserve.com.au/wp-content/uploads/CarParkingPlans/CAR-PARKING-PLAN-SERVICE-BY.jpg",
    minPrice: 10,
    maxPrice: 20,
    avgPrice: 15
  },
  {
    id: 2,
    name: "basement2",
    reservationsEnabled: true,
    notReservedOccupiable: false,
    status: {
      timestamp: Date.now(),
      emptySpace: 5,
      reservedSpace: 10,
      occupiedSpace: 20
    },
    spaces: mockSpaces,
    templateImg: "https://www.atlanticenter.com/wp-content/uploads/2019/04/floor-plan-first-basement-floor-indoor-parking-spaces-to-rent-milan-1.jpg",
    minPrice: 20,
    maxPrice: 50,
    avgPrice: 25
  },
  {
    id: 3,
    name: "basement3",
    description: "description3 ",
    reservationsEnabled: false,
    notReservedOccupiable: false,
    status: {
      timestamp: Date.now(),
      emptySpace: 5,
      reservedSpace: 5,
      occupiedSpace: 5
    },
    spaces: mockSpaces,
    templateImg: "https://www.atlanticenter.com/wp-content/uploads/2019/04/floor-plan-first-basement-floor-indoor-parking-spaces-to-rent-milan-1.jpg",
    minPrice: 23,
    maxPrice: 32,
    avgPrice: 29
  },
  {
    id: 4,
    name: "basement4",
    description: "description444444444444444444444444444444444",
    reservationsEnabled: false,
    notReservedOccupiable: false,
    status: {
      timestamp: Date.now(),
      emptySpace: 3,
      reservedSpace: 4,
      occupiedSpace: 7
    },
    spaces: mockSpaces,
    templateImg: "https://www.atlanticenter.com/wp-content/uploads/2019/04/floor-plan-first-basement-floor-indoor-parking-spaces-to-rent-milan-1.jpg",
    minPrice: 24,
    maxPrice: 31,
    avgPrice: 30
  },
  {
    id: 5,
    name: "basement5",
    reservationsEnabled: false,
    notReservedOccupiable: false,
    status: {
      timestamp: Date.now(),
      emptySpace: 9,
      reservedSpace: 3,
      occupiedSpace: 5
    },
    spaces: mockSpaces,
    templateImg: "https://www.atlanticenter.com/wp-content/uploads/2019/04/floor-plan-first-basement-floor-indoor-parking-spaces-to-rent-milan-1.jpg",
    minPrice: 25,
    maxPrice: 30,
    avgPrice: 28
  },
  {
    id: 6,
    name: "basement6",
    reservationsEnabled: false,
    notReservedOccupiable: false,
    status: {
      timestamp: Date.now(),
      emptySpace: 290,
      reservedSpace: 310,
      occupiedSpace: 155
    },
    spaces: mockSpaces,
    templateImg: "https://www.atlanticenter.com/wp-content/uploads/2019/04/floor-plan-first-basement-floor-indoor-parking-spaces-to-rent-milan-1.jpg",
    minPrice: 41,
    maxPrice: 45,
    avgPrice: 43
  },
  {
    id: 7,
    name: "basement7",
    reservationsEnabled: false,
    notReservedOccupiable: false,
    status: {
      timestamp: Date.now(),
      emptySpace: 15,
      reservedSpace: 42,
      occupiedSpace: 242
    },
    spaces: mockSpaces,
    templateImg: "https://www.atlanticenter.com/wp-content/uploads/2019/04/floor-plan-first-basement-floor-indoor-parking-spaces-to-rent-milan-1.jpg",
    minPrice: 33,
    maxPrice: 35,
    avgPrice: 34
  },
  {
    id: 8,
    name: "basement8",
    description: "description8 description8 description8 description8 description8 description8 description8 description8 description8 description8",
    reservationsEnabled: false,
    notReservedOccupiable: false,
    status: {
      timestamp: Date.now(),
      emptySpace: 1001,
      reservedSpace: 5,
      occupiedSpace: 5
    },
    spaces: mockSpaces,
    templateImg: "https://www.atlanticenter.com/wp-content/uploads/2019/04/floor-plan-first-basement-floor-indoor-parking-spaces-to-rent-milan-1.jpg",
    minPrice: 32,
    maxPrice: 44,
    avgPrice: 43
  },
  {
    id: 9,
    name: "basement9",
    reservationsEnabled: false,
    notReservedOccupiable: false,
    status: {
      timestamp: Date.now(),
      emptySpace: 141,
      reservedSpace: 124,
      occupiedSpace: 421
    },
    spaces: mockSpaces,
    templateImg: "https://www.atlanticenter.com/wp-content/uploads/2019/04/floor-plan-first-basement-floor-indoor-parking-spaces-to-rent-milan-1.jpg",
    minPrice: 21,
    maxPrice: 23,
    avgPrice: 22
  },
  {
    id: 10,
    name: "basement10",
    reservationsEnabled: false,
    notReservedOccupiable: false,
    status: {
      timestamp: Date.now(),
      emptySpace: 0,
      reservedSpace: 150,
      occupiedSpace: 5
    },
    spaces: mockSpaces,
    templateImg: "https://www.atlanticenter.com/wp-content/uploads/2019/04/floor-plan-first-basement-floor-indoor-parking-spaces-to-rent-milan-1.jpg",
    minPrice: 22,
    maxPrice: 26,
    avgPrice: 24
  },
  {
    id: 11,
    name: "basement11",
    reservationsEnabled: false,
    notReservedOccupiable: false,
    status: {
      timestamp: Date.now(),
      emptySpace: 1,
      reservedSpace: 1,
      occupiedSpace: 1
    },
    spaces: mockSpaces,
    templateImg: "https://www.atlanticenter.com/wp-content/uploads/2019/04/floor-plan-first-basement-floor-indoor-parking-spaces-to-rent-milan-1.jpg",
    minPrice: 23,
    maxPrice: 25,
    avgPrice: 24
  },
];
