import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ParkArea } from '@app/core/models/park-area';
import { RouteUrl } from '@app/core/utils/route';
import { ConfirmationService, LazyLoadEvent, MessageService } from 'primeng/api';
import { ParkAreaService } from '@app/core/services/park-area.service';
import { HttpErrorResponse } from '@angular/common/http';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-m-park-areas',
  templateUrl: './m-park-areas.component.html',
  styleUrls: ['./m-park-areas.component.scss']
})
export class MParkAreasComponent implements OnInit, OnDestroy {

  readonly pageSize = 6;

  pageNumber: number | null = null;
  totalPages: number = 0;

  areas: ParkArea[] = [];
  unsubscribe: Subscription[] = [];

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private confirmService: ConfirmationService,
    private messageService: MessageService,
    private parkAreaService: ParkAreaService
  ) { }

  ngOnInit(): void {
    const paramMap = this.route.snapshot.queryParamMap;

    if(paramMap.has("page")) {
      const page = Number(paramMap.get("page"));
      if(page == this.pageNumber) return;
      this.pageNumber = page;
    }
    else {
      this.pageNumber = 1;
    }

  }

  goMap() {
    this.router.navigateByUrl(RouteUrl.mParkMap());
  }

  getParkId() {
    return Number(this.route.snapshot.paramMap.get("parkid"));
  }

  newArea() {
    let parkid = this.getParkId();
    this.router.navigateByUrl(RouteUrl.mNewParkArea(parkid))
  }

  editArea(area: ParkArea) {
    let parkid = this.getParkId();
    this.router.navigateByUrl(RouteUrl.mEditParkArea(parkid,area.id))
  }

  deleteArea(area: ParkArea) {
    this.confirmService.confirm({
      message: 'Are you sure to delete xxx named park area with xxx id?',
      accept: () => {
        // TODO: DELETE PARK AREA

        this.messageService.add({
          summary: "Park Area Deletion",
          closable: true,
          severity: "error",
          life:5000,
          detail: "The park area with xxx id and xxx name is deleted."
        })
      }
    });
  }

  ngOnDestroy(): void {
    this.unsubscribe.forEach(x => x.unsubscribe());
  }

  loadData(evt: LazyLoadEvent) {
    const pageNumber = (evt.first! / this.pageSize)+1;
    this.parkAreaService.getAreasPage(pageNumber, this.pageSize).subscribe({
      next: response => {
        if(response.headers.has("x-total-pages"))
          this.totalPages = Number(response.headers.get("x-total-pages"));

        this.router.navigate([], {
          relativeTo: this.route,
          queryParams: {
            ...this.route.snapshot.queryParams,
            page: pageNumber
          }
        });

        this.areas = response.body!;
      },
      error: (err: HttpErrorResponse) => {
        this.messageService.add({
          summary: "Fetch Park Areas",
          closable: true,
          severity: "error",
          life:5000,
          detail: err.error.message
        });
      }
    });
  }
}
