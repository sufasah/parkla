import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ParkArea } from '@app/core/models/park-area';
import { ParkAreaService } from '@app/core/services/park-area.service';
import { SignalrService } from '@app/core/services/signalr.service';
import { RouteUrl } from '@app/core/utils/route';
import { AreaDataViewComponent } from '@app/shared/components/area-dataview/area-dataview.component';
import { ISubscription } from '@microsoft/signalr';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-m-park-areas',
  templateUrl: './m-park-areas.component.html',
  styleUrls: ['./m-park-areas.component.scss']
})
export class MParkAreasComponent implements OnInit, OnDestroy {

  @ViewChild(AreaDataViewComponent)
  dataView!: AreaDataViewComponent;

  readonly pageSize = 6;

  loading = false;
  parkid!: string;

  unsubscribe: Subscription[] = [];

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private confirmService: ConfirmationService,
    private messageService: MessageService,
    private parkAreaService: ParkAreaService,
  ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(paramMap => {
        this.parkid = paramMap.get("parkid")!;
    });

  }

  goMap() {
    this.router.navigateByUrl(RouteUrl.mParkMap());
  }

  newArea() {
    this.router.navigateByUrl(RouteUrl.mNewParkArea(this.parkid))
  }

  editArea(area: ParkArea) {
    this.router.navigateByUrl(RouteUrl.mEditParkArea(this.parkid,area.id))
  }

  deleteArea(area: ParkArea) {
    this.confirmService.confirm({
      message: `Are you sure to delete the park area with '${area.name}'?`,
      accept: () => {
        this.parkAreaService.deleteArea(area).subscribe({
          next: () => {
            this.messageService.add({
              summary: "Park Area Deletion",
              closable: true,
              severity: "success",
              life: 1500,
              detail: `The park area with '${area.name}' is deleted`
            });
            this.dataView.refresh();
          },
          error: (err: HttpErrorResponse) => {
            this.messageService.add({
              summary: "Park Area Deletion",
              closable: true,
              severity: "error",
              life:5000,
              detail: err.error.message
            });
          }
        });

      }
    });
  }

  onError(error: string) {
    this.messageService.add({
      summary: "Fetch Park Areas",
      closable: true,
      severity: "error",
      life:5000,
      detail: error
    });
    this.loading = false;
  }

  ngOnDestroy(): void {
    this.unsubscribe.forEach(x => x.unsubscribe());
  }
}
