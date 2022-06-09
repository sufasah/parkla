import { HttpErrorResponse } from '@angular/common/http';
import { Component, ContentChild, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { templatesUrl, templateNoImageUrl } from '@app/core/constants/http';
import { ParkArea } from '@app/core/models/park-area';
import { AuthService } from '@app/core/services/auth.service';
import { ParkAreaService } from '@app/core/services/park-area.service';
import { SignalrService } from '@app/core/services/signalr.service';
import { RouteUrl } from '@app/core/utils/route';
import { ISubscription } from '@microsoft/signalr';
import { LazyLoadEvent, MessageService, SelectItem } from 'primeng/api';
import { DataView } from 'primeng/dataview';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-area-dataView',
  templateUrl: './area-dataview.component.html',
  styleUrls: ['./area-dataview.component.scss']
})
export class AreaDataViewComponent implements OnInit, OnDestroy {

  @Input()
  pageSize = 10;

  @Input()
  loading = false;

  @Input()
  managerMode = false;

  @Output()
  onFetchError = new EventEmitter<string>();

  areas!: ParkArea[];

  totalRecords = 0;

  nextRecord = 0;

  areaDeleteVisible = false;

  sortOptions: SelectItem[] = [
    {
      label: "Min Price Low to High",
      value: "!minPrice"
    },
    {
      label: "Min Price High to Low",
      value: "minPrice"
    },
    {
      label: "Average Price Low to High",
      value: "!averagePrice"
    },
    {
      label: "Average Price High to Low",
      value: "averagePrice"
    },
    {
      label: "Max Price Low to High",
      value: "!maxPrice"
    },
    {
      label: "Max Price High to Low",
      value: "maxPrice"
    },
  ];;

  sortOrder!: number;

  sortField!: string;

  @ViewChild("dv")
  dataView!: DataView;

  searchTOID?: NodeJS.Timeout;
  lastSearchSeconds = -10;
  lastSearchInput: string | null = null;

  orderBy: string | null = null;
  asc: boolean | null = null;

  unsubscribe: Subscription[] = [];
  parkReservationStreamSubscription?: ISubscription<any>;
  parkAreaChangesSubscription?: Subscription;

  deletingArea?: ParkArea;

  signalrSubscribed = false;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private parkAreaService: ParkAreaService,
    private authService: AuthService,
    private signalrService: SignalrService,
    private messageService: MessageService
  ) {

  }

  newArea() {
    this.router.navigateByUrl(RouteUrl.mNewParkArea(this.getParkId()))
  }

  ngOnDestroy(): void {
    this.unsubscribe.forEach(x => x.unsubscribe());
    this.stopReservedCountStream();
    this.unregisterParkAreaChanges();
  }

  ngOnInit(): void {
    const paramMap = this.route.snapshot.queryParamMap;

    if(paramMap.has("page")) {
      const page = Number(paramMap.get("page"));
      if(page > 1)
        this.nextRecord = page * this.pageSize - this.pageSize;
    }
  }

  startReservedCountStream(areaIds:number[]) {
    this.stopReservedCountStream();
    this.parkReservationStreamSubscription = this.signalrService.getParkAreasReservedSpaceCountAsStream(
      areaIds,
      {
        next: (item: {areaId: number, reservedSpaceCount: number}) => {
          let area = this.areas.find(x => x.id == item.areaId);
          if(area) {
            area.reservedSpace = item.reservedSpaceCount;
          }
        },
        error: (err:any) => {
          console.log(err);
        },
        complete: () => {
          this.parkReservationStreamSubscription?.dispose();
        }
      }
    );
  }

  stopReservedCountStream() {
    this.parkReservationStreamSubscription?.dispose();
  }

  registerParkAreaChanges() {
    this.parkAreaChangesSubscription = this.signalrService.registerParkAreaChanges(this.getParkId(), (area, isDelete) => {

      const oldAreaIndex = this.areas.findIndex(x => x.id == area.id);
      if(oldAreaIndex != -1) {
        if(isDelete) {
          this.areas.splice(oldAreaIndex, 1);
        }
        else {
          if(this.areas[oldAreaIndex].xmin <= area.xmin)
            this.areas[oldAreaIndex] = {...area};
        }
      }
      this.dataView.cd.detectChanges();
    });
  }

  unregisterParkAreaChanges() {
    this.parkAreaChangesSubscription?.unsubscribe();
  }

  onSortChange(event:any) {
    let value = event.value;

    if (value.indexOf('!') === 0) {
      this.sortOrder = 1;
      this.sortField = value.substring(1, value.length);
    }
    else {
      this.sortOrder = -1;
      this.sortField = value;
    }
  }

  goArea(area:ParkArea) {
    let parkid = this.getParkId();

    this.router.navigateByUrl( this.authService.asManager
      ? RouteUrl.mParkArea(parkid, area.id)
      : RouteUrl.parkArea(parkid,area.id)
    );
  }

  getParkId() {
    return this.route.snapshot.paramMap.get("parkid")!;
  }

  loadData(evt: LazyLoadEvent) {
    if(this.loading) return;

    const nextRecord = evt.first!;
    const sortField = evt.sortField;
    const asc = evt.sortOrder == 1 ? true : false;

    if(
      this.nextRecord == nextRecord &&
      this.orderBy ==  sortField &&
      this.asc == asc
    ) return;

    setTimeout(() => {
      this.orderBy = sortField ?? null;
      this.asc = asc;
      this.nextRecord = nextRecord;

      this.fetchAreas(
        nextRecord,
        this.lastSearchInput,
        this.orderBy,
        this.asc);
    }, 0);
  }

  onSearchInput(evt: any) {
      const seconds = Number.parseInt((evt.timeStamp/1000).toFixed(0));
      const data = evt.target.value;

      if(seconds - this.lastSearchSeconds < 2)
        clearTimeout(this.searchTOID!);

      this.searchTOID = setTimeout(() => {
        this.fetchAreas(
          0,
          data,
          this.orderBy,
          this.asc
        );
      }, 1000);

      this.lastSearchSeconds = seconds;
      this.lastSearchInput = data;
  }

  refresh() {
    this.fetchAreas(
      this.nextRecord,
      this.lastSearchInput,
      this.orderBy,
      this.asc
    );
  }

  fetchAreas(
    nextRecord: number,
    search: string | null = null,
    orderBy: string | null = null,
    asc: boolean | null = null
  ) {
    this.loading = true;
    this.stopReservedCountStream();

    this.parkAreaService.getAreasPage(
      nextRecord,
      this.pageSize,
      this.getParkId(),
      search,
      orderBy,
      asc
    ).subscribe({
      next: response => {

        this.areas = response.body!;

        if(!this.signalrSubscribed) {
          this.signalrSubscribed = true;

          const sub = this.signalrService.connectedEvent.subscribe((connected) => {
            if(connected === null) return;

            if(connected) {
              this.startReservedCountStream(this.areas.map(x => x.id));
              this.registerParkAreaChanges();
            }
            else {
              this.stopReservedCountStream();
            }
          });

          this.unsubscribe.push(sub);
        }

        if(response.headers.has("x-total-records")) {
          this.totalRecords = Number(response.headers.get("x-total-records"));
          this.nextRecord = nextRecord;

          this.router.navigate([], {
            relativeTo: this.route,
            queryParams: {
              ...this.route.snapshot.queryParams,
              page: (nextRecord + this.pageSize) / this.pageSize
            }
          });
        }
        this.loading = false;
        if(this.signalrService.isConnected)
          this.startReservedCountStream(this.areas.map(x => x.id));
      },
      error: (err: HttpErrorResponse) => {
        this.loading = false;
        this.onFetchError.emit(err.error.message);
      }
    });
  }

  getImageUrl(area: ParkArea) {
    return area.templateImage ? `${templatesUrl}/${area.templateImage}` : templateNoImageUrl
  }

  onImageError(image: HTMLImageElement) {
    image.src = templateNoImageUrl;
  }

  editArea(area: ParkArea) {
    this.router.navigateByUrl(RouteUrl.mEditParkArea(this.getParkId(), area.id))
  }

  deleteArea(area: ParkArea) {
    this.deletingArea = area;
    this.areaDeleteVisible = true;
  }

  deleteConfirm() {
    this.areaDeleteVisible = false;
    this.parkAreaService.deleteArea(this.deletingArea!).subscribe({
      next: () => {
        this.messageService.add({
          summary: "Park Area Deletion",
          closable: true,
          severity: "success",
          life: 1500,
          detail: `The park area with '${this.deletingArea!.name}' is deleted`
        });
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

  cancelConfirm() {
    this.areaDeleteVisible = false;
  }
}
