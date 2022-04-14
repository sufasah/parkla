import { SpaceReservation } from "@app/core/models/space-reservation";

const SECOND = 1000;
const MINUTE = 60*SECOND;
const HOUR = 60*MINUTE;
const DAY = 24*HOUR;


export const mockSpaceReservations: SpaceReservation[] = [
  {
    username:"a",
    startTime: new Date(Date.now()+30*MINUTE),
    endTime: new Date(Date.now()+HOUR)
  },
  {
    username:"b",
    startTime: new Date(Date.now()+2*HOUR),
    endTime: new Date(Date.now()+3*HOUR)
  },
  {
    username:"c",
    startTime: new Date(Date.now()+3*HOUR+30*MINUTE),
    endTime: new Date(Date.now()+3*HOUR+45*MINUTE)
  },
  {
    username:"d",
    startTime: new Date(Date.now()+6*HOUR),
    endTime: new Date(Date.now()+12*HOUR)
  },
  {
    username:"e",
    startTime: new Date(Date.now()+2*DAY),
    endTime: new Date(Date.now()+3*DAY)
  },
  {
    username:"f",
    startTime: new Date(Date.now()+4*DAY+30*MINUTE),
    endTime: new Date(Date.now()+4*DAY+35*MINUTE)
  },
  {
    username:"g",
    startTime: new Date(Date.now()+4*DAY+35*MINUTE),
    endTime: new Date(Date.now()+4*DAY+40*MINUTE)
  },
  {
    username:"h",
    startTime: new Date(Date.now()+4*DAY+45*MINUTE),
    endTime: new Date(Date.now()+4*DAY+50*MINUTE)
  },
  {
    username:"i",
    startTime: new Date(Date.now()+4*DAY+50*MINUTE),
    endTime: new Date(Date.now()+4*DAY+55*MINUTE)
  },
  {
    username:"j",
    startTime: new Date(Date.now()+4*DAY+HOUR),
    endTime: new Date(Date.now()+4*DAY+HOUR+10*MINUTE)
  },
  {
    username:"k",
    startTime: new Date(Date.now()+4*DAY+HOUR+20*MINUTE),
    endTime: new Date(Date.now()+4*DAY+HOUR+30*MINUTE)
  },
  {
    username:"l",
    startTime: new Date(Date.now()+4*DAY+HOUR+50*MINUTE),
    endTime: new Date(Date.now()+4*DAY+2*HOUR+10*MINUTE)
  },
  {
    username:"m",
    startTime: new Date(Date.now()+4*DAY+3*HOUR+10*MINUTE),
    endTime: new Date(Date.now()+4*DAY+3*HOUR+20*MINUTE)
  },
  {
    username:"n",
    startTime: new Date(Date.now()+4*DAY+3*HOUR+30*MINUTE),
    endTime: new Date(Date.now()+4*DAY+3*HOUR+35*MINUTE)
  }
]
