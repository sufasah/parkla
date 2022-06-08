import { DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Dashboard } from '@app/core/models/dashboard';
import { AuthService } from '@app/core/services/auth.service';
import { UserService } from '@app/core/services/user.service';
import { RouteUrl } from '@app/core/utils/route';
import { ChartData, ChartOptions } from 'chart.js';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-m-dashboard',
  templateUrl: './m-dashboard.component.html',
  styleUrls: ['./m-dashboard.component.scss']
})
export class MDashboardComponent implements OnInit {
  model: Dashboard = <any>{};

  spaceUsageMinColor(opacity: number) {
    return `rgba(255, 198, 0, ${opacity})`;
  }
  spaceUsageAvgColor(opacity: number) {
    return `rgba(95, 208, 104, ${opacity})`;
  }
  spaceUsageMaxColor(opacity: number) {
    return `rgba(38, 120, 163, ${opacity})`;
  }
  spaceUsageSumColor(opacity: number) {
    return `rgba(5, 55, 66, ${opacity})`;
  }

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

  dateToLabel(date: Date) {
    return this.datePipe.transform(date, "yyyy-MM-dd");
  }

  spaceUsageLineData: ChartData<"line"> = {
    labels: [],
    datasets: [
      {
        label: "Average",
        fill: false,
        yAxisID: 'y',
        tension: .4,
        data: [],
        borderColor: this.spaceUsageAvgColor(1),
        backgroundColor: this.spaceUsageAvgColor(.5),
        borderDash: [2,2]
      },
      {
        label: "Min",
        fill: false,
        yAxisID: 'y',
        tension: .4,
        data: [],
        borderColor: this.spaceUsageMinColor(1),
        backgroundColor: this.spaceUsageMinColor(.5),
        borderDash: [2,2]
      },
      {
        label: "Max",
        fill: false,
        yAxisID: 'y',
        tension: .4,
        data: [],
        borderColor: this.spaceUsageMaxColor(1),
        backgroundColor: this.spaceUsageMaxColor(.5),
        borderDash: [2,2]
      },
      {
        label: "Sum",
        fill: false,
        yAxisID: 'y',
        tension: .4,
        data: [],
        borderColor: this.spaceUsageSumColor(1),
        backgroundColor: this.spaceUsageSumColor(.5),
        borderDash: [2,2]
      }
    ]
  }

  carCountUsedSpaceLineData: ChartData<"line"> = {
    labels: [],
    datasets: [
      {
        label: "Cars Used Space",
        fill: true,
        yAxisID: 'y',
        tension: .4,
        data: [],
        borderColor: this.spaceUsageSumColor(1),
        backgroundColor: this.spaceUsageSumColor(.5),
      },
    ]
  }

  earningLineData: ChartData<"line"> = {
    labels: [],
    datasets: [
      {
        label: "Earning",
        fill: true,
        yAxisID: 'y',
        tension: .4,
        data: [],
        borderColor: this.spaceUsageAvgColor(1),
        backgroundColor: this.spaceUsageAvgColor(.5),
      }
    ]
  }

  lineOptions: ChartOptions<"line"> = {
    plugins: {
      legend: {
        labels: {
          color: '#333',
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
        },
        title: {
          display: true,
          font: {
            size: 16,
            weight: "700"
          },
          text: "Days"
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
        },
        title: {
          display: true,
          font: {
            size: 16,
            weight: "700"
          }
        }
      }
    }
  }

  spaceUsageLineOptions: ChartOptions<"line"> = {...this.lineOptions};
  earningLineOptions: ChartOptions<"line"> = {...this.lineOptions};
  carCountUsedSpaceLineOptions: ChartOptions<"line"> = {...this.lineOptions};

  dashboardLoading = true;

  constructor(
    private router: Router,
    private userService: UserService,
    private authService: AuthService,
    private messageService: MessageService,
    private datePipe: DatePipe
  ) { }

  ngOnInit(): void {
    this.initSpaceUsageChart();
    this.initCarCountUsedSpaceChart();
    this.initEarningChart();

    this.fetchDashboard();
  }

  initSpaceUsageChart() {
    const o = this.spaceUsageLineOptions;

    o.scales! = {
      ...o.scales!,
      y: {
        ...o.scales!.y!,
        title: {
          ...o.scales!.y!.title!,
          text: "Space Usage Time in Minutes"
        }
      }
    }

    this.spaceUsageLineOptions = {...o};
  }

  initCarCountUsedSpaceChart() {
    const o = this.carCountUsedSpaceLineOptions;

    o.scales = {
      ...o.scales!,
      y: {
        ...o.scales!.y!,
        title: {
          ...o.scales!.y!.title!,
          text: "Count"
        }
      }
    }

    this.carCountUsedSpaceLineOptions = {...o};
  }

  initEarningChart() {
    const o = this.earningLineOptions!;

    o.scales = {
      ...o.scales!,
      y: {
        ...o.scales!.y!,
        title: {
          ...o.scales!.y!.title!,
          text: "Turkish Lira (TRY)"
        }
      }
    }

    this.earningLineOptions = {...o};
  }

  fetchDashboard() {
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

        model.spaceUsageTimePerDay.forEach(data => {
          this.spaceUsageLineData.labels?.push(this.dateToLabel(data.x));
          this.spaceUsageLineData.datasets[1].data.push(data.y.min);
          this.spaceUsageLineData.datasets[0].data.push(data.y.avg);
          this.spaceUsageLineData.datasets[2].data.push(data.y.max);
          this.spaceUsageLineData.datasets[3].data.push(data.y.sum);
        });
        this.spaceUsageLineData = {...this.spaceUsageLineData};

        this.earningLineData.labels = model.totalEarningPerDay.map(x => this.dateToLabel(x.x))
        this.earningLineData.datasets[0].data = model.totalEarningPerDay.map(x => x.y);
        this.earningLineData = {...this.earningLineData};

        this.carCountUsedSpaceLineData.labels = model.carCountUsedSpacePerDay.map(x => this.dateToLabel(x.x));
        this.carCountUsedSpaceLineData.datasets[0].data = model.carCountUsedSpacePerDay.map(x => x.y);
        this.carCountUsedSpaceLineData = {...this.carCountUsedSpaceLineData};
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
    });
  }

  goMap() {
    this.router.navigateByUrl(RouteUrl.mParkMap());
  }
}
