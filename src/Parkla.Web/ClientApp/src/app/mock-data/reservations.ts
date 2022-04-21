import { TimeUnit } from "@app/core/enums/TimeUnit";
import { Reservation } from "@app/core/models/reservation";

export const mockReservations: Reservation[] = [
  {
    parkId: 1,
    parkName: "seapark",
    areaName: "basement5",
    pricing: {
      type: "truck",
      timeUnit: TimeUnit.MINUTE,
      timeAmount: 30,
      price: 50
    },
    timeFrom: new Date(),
    timeTo: new Date(Date.now()+1000*60*30)
  }
];
