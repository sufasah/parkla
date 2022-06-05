import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Dashboard } from '@app/core/models/dashboard';
import { AuthService } from '@app/core/services/auth.service';
import { UserService } from '@app/core/services/user.service';
import { RouteUrl } from '@app/core/utils/route';
import { ChartData, ChartOptions } from 'chart.js';
import * as moment from 'moment';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-m-dashboard',
  templateUrl: './m-dashboard.component.html',
  styleUrls: ['./m-dashboard.component.scss']
})
export class MDashboardComponent implements OnInit {
  model2 = {
    freeSpace: 300,
    occupiedSpace: 200,
    reservedSpace: 500,
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

  model: Dashboard = <any>{};

  spacesPieData: ChartData<"pie"> = {
    labels: ["Empty", "Occupied", "Unknown"],
    datasets: [
      {
        data: [-1,-1,-1],
        backgroundColor: ["#6EBF8B","#D82148","#151D3B"],
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

  dashboardLoading = true;

  constructor(
    private router: Router,
    private userService: UserService,
    private authService: AuthService,
    private messageService: MessageService
  ) { }

  ngOnInit(): void {
    this.userService.getDashboard(Number(this.authService.accessToken?.sub!)).subscribe({
      next: model => {
        this.model = model;
        this.dashboardLoading = false;

        this.spacesPieData.datasets[0].data = [
          this.model.totalEmptySpaces,
          this.model.totalOccupiedSpaces,
          this.model.totalSpaces - this.model.totalEmptySpaces - this.model.totalOccupiedSpaces
        ];
        this.spacesPieData = {...this.spacesPieData};

      },
      error: (err: HttpErrorResponse) => {
        this.messageService.add({
          summary: "Fetch Dashboard Data",
          closable: true,
          severity: "error",
          life:5000,
          detail: err.error.message
        });
        this.dashboardLoading = false;
      }
    })
  }

  goMap() {
    this.router.navigateByUrl(RouteUrl.mParkMap());
  }
}
