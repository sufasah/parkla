import { ParkArea } from "@app/core/models/parking-lot";
import { mockSpaces } from "./spaces";

export const mockAreas: ParkArea[] = [{
  name: "basement5",
  reservationsEnabled: true,
  notReservedOccupiable: true,
  spaces: mockSpaces,
  templateImg: "https://www.realserve.com.au/wp-content/uploads/CarParkingPlans/CAR-PARKING-PLAN-SERVICE-BY.jpg"
  },
  {
    name: "heyyo",
    reservationsEnabled: false,
    notReservedOccupiable: false,
    spaces: mockSpaces,
    templateImg: "https://www.atlanticenter.com/wp-content/uploads/2019/04/floor-plan-first-basement-floor-indoor-parking-spaces-to-rent-milan-1.jpg"
  }
];
