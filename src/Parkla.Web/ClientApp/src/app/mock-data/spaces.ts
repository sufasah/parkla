import { ParkSpace } from "@app/core/models/parking-lot";

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
    reservations: [{
      username:"a",
      startTime: new Date(Date.now()+1000*60*60),
      endTime: new Date(Date.now()+1000*60*60*2)
    }]
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
    reservations: [{
      username:"b",
      startTime: new Date(Date.now()+1000*60*60),
      endTime: new Date(Date.now()+1000*60*60*2)
    }]
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
    reservations: [{
      username:"c",
      startTime: new Date(Date.now()+1000*60*60),
      endTime: new Date(Date.now()+1000*60*60*2)
    }]
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
    reservations: [{
      username:"testuser",
      startTime: new Date(Date.now()+1000*60*60),
      endTime: new Date(Date.now()+1000*60*60*2)
    }]
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
    reservations: [{
      username:"e",
      startTime: new Date(Date.now()+1000*60*60),
      endTime: new Date(Date.now()+1000*60*60*2)
    }]
  }
];
