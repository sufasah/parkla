import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ParkingLot } from '@app/core/models/parking-lot';
import { RouteUrl } from '@app/core/utils/route.util';
import { ChartData, ChartOptions } from 'chart.js';
import * as moment from 'moment';
import { UIChart } from 'primeng/chart';

@Component({
  selector: 'app-m-dashboard',
  templateUrl: './m-dashboard.component.html',
  styleUrls: ['./m-dashboard.component.scss']
})
export class MDashboardComponent implements OnInit {
  model = {
    freeSpace: 300,
    occupiedSpace: 200,
    reservedSpace: 500,
    parkCount: 63,
    areaCount: 241,
    topData: <ParkingLot[]>[
      {name: "name", location: "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"},
      {name: "name2", location: "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"},
      {name: "name3", location: "cccccccccccccccccccccccccccccc"},
      {name: "name4", location: "dddddddddddddddddddddddddddddd"},
      {name: "name5", location: "eeeeeeeeeeeeeeeeeeeeeeeeeeeeee"},
      {name: "name6", location: "ffffffffffffffffffffffffffffff"},
      {name: "name7", location: "gggggggggggggggggggggggggggggg"},
      {name: "name8", location: "uuuuuuuuuuuuuuuuuuuuuuuuuuuuuu"},
      {name: "name9", location: "hhhhhhhhhhhhhhhhhhhhhhhhhhhhhh"},
      {name: "name10", location: "iiiiiiiiiiiiiiiiiiiiiiiiiiiiii"},
      {name: "name11", location: "jjjjjjjjjjjjjjjjjjjjjjjjjjjjjj"},
      {name: "name12", location: "kkkkkkkkkkkkkkkkkkkkkkkkkkkkkk"},
      {name: "name13", location: "llllllllllllllllllllllllllllll"},
      {name: "name14", location: "mmmmmmmmmmmmmmmmmmmmmmmmmmmmmm"},
      {name: "name15", location: "nnnnnnnnnnnnnnnnnnnnnnnnnnnnnn"},
      {name: "name16", location: "oooooooooooooooooooooooooooooo"},
      {name: "name17", location: "pppppppppppppppppppppppppppppp"},
      {name: "name18", location: "rrrrrrrrrrrrrrrrrrrrrrrrrrrrrr"},
      {name: "name19", location: "ssssssssssssssssssssssssssssss"},
      {name: "name20", location: "tttttttttttttttttttttttttttttt"},
    ],
    weekData: [
      [5,5,5,5,5,5,5],
      [5,5,5,5,5,5,5],
      [1,1,1,1,1,1,1],
      [2,2,2,2,2,2,2],
      [2,2,2,2,2,2,2],
      [5,5,5,5,5,5,5],
      [2,2,2,2,2,2,2],
      [3,3,3,3,3,3,3],
      [15,15,15,15,15,15,15],
      [20,20,20,20,20,20,20],
      [10,10,10,10,10,10,10],
      [3,3,3,3,3,3,3],
      [3,3,3,3,3,3,3],
      [3,3,3,3,3,3,3],
      [1,1,1,1,1,1,1],
      [10,10,10,10,10,10,10],
      [1,1,1,1,1,1,1],
      [1,1,1,1,1,1,1],
      [2,2,2,2,2,2,2],
      [1,1,1,1,1,1,1],
      [1,1,1,1,1,1,1],
      [2,2,2,2,2,2,2],
      [1,1,1,1,1,1,1],
      [1,1,1,1,1,1,1],
    ],
    countryParks: [
      {name: "Turkey", count: 30},
      {name: "United States", count: 10},
      {name: "England", count: 5},
      {name: "Azerbaijan", count: 20},
      {name: "Germany", count: 15},
    ]
  }

  spacesPieData: ChartData<"pie"> = {
    labels: ["Free", "Occupied", "Reserved"],
    datasets: [
      {
        data: [
          this.model.freeSpace,
          this.model.occupiedSpace,
          this.model.reservedSpace
        ],
        backgroundColor: ["#151D3B","#D82148","#6EBF8B"],
        hoverBackgroundColor: ["#DADBBD","#DADBBD","#DADBBD"]
      }
    ]
  }

  spacesPieOptions: ChartOptions<"pie"> = {
    plugins: {
      legend: {
        labels: {
          color: '#000',
          font: {
            size: 16
          }
        },
        onClick: undefined,
      },
      tooltip: {
        callbacks: {
          afterLabel: (item) => {
            return (
              <number>item.raw * 100 /
              item.dataset.data
                .reduce((p,c) => p+c, 0)
            ).toFixed(2) + "%";
          },
        },
        bodyFont: {
          size: 13,
          weight: "bolder"
        }
      }
    }
  }
  getDayLabels() {
    const result = [];

    for(let i=0; i<90; i++){
      const m = moment().subtract(90-i,"days");
      const val = m.format("DD.MM.YYYY")
      result.push(val);
    }

    return result;
  }

  avgSpaceUsageLineData: ChartData<"line"> = {
    labels: this.getDayLabels(),
    datasets: [
      {
        label: "Avarage Space Usage Time",
        fill: false,
        borderColor: '#333',
        yAxisID: 'y',
        tension: .4,
        data: [1,2,3,4,5,6,7,8,9,10,1,2,3,4,5,6,7,8,9,10,1,2,3,4,5,6,7,8,9,10,1,2,3,4,5,6,7,8,9,10,1,2,3,4,5,6,7,8,9,10,1,2,3,4,5,6,7,8,9,10,1,2,3,4,5,6,7,8,9,10,1,2,3,4,5,6,7,8,9,10,1,2,3,4,5,6,7,8,9,10],
      }
    ]
  }

  avgSpaceUsageLineOptions: ChartOptions<"line"> = {
    plugins: {
      legend: {
        labels: {
          color: '#000',
          font: {
            size: 16
          }
        },
        onClick: undefined,
      },
      tooltip: {
        bodyFont: {
          size: 16,
        },
        titleFont: {
          size: 16
        }
      }
    },
    scales: {
      x: {
        ticks: {
          color: '#495057'
        },
        grid: {
          drawOnChartArea: false,
          color: '#ebedef'
        }
      },
      y: {
        type: 'linear',
        display: true,
        position: 'left',
        ticks: {
          color: '#495057'
        },
        grid: {
          color: '#ebedef'
        }
      }
    }
  }

  constructor(private router: Router) { }

  ngOnInit(): void {
  }

  goMap() {
    this.router.navigateByUrl(RouteUrl.mParkMap());
  }
}
