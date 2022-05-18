import { TimeUnit } from "@app/core/enums/TimeUnit";
import { Reservation } from "@app/core/models/reservation";

export const mockReservations: Reservation[] = [
  {
    id: 1,
    parkId: "1",
    parkName: "seapark",
    areaName: "basement5",
    pricing: {
      id:1,
      areaId: 2,
      type: "truck",
      unit: TimeUnit.MINUTE,
      amount: 30,
      price: 50
    },
    timeFrom: new Date(),
    timeTo: new Date(Date.now()+1000*60*30)
  }
];
