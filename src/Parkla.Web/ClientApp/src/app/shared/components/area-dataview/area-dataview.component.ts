import { HttpErrorResponse } from '@angular/common/http';
import { Component, ContentChild, EventEmitter, Input, OnInit, Output, TemplateRef, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ParkArea } from '@app/core/models/park-area';
import { AuthService } from '@app/core/services/auth.service';
import { ParkAreaService } from '@app/core/services/park-area.service';
import { RouteUrl } from '@app/core/utils/route';
import { LazyLoadEvent, SelectItem } from 'primeng/api';
import { DataView } from 'primeng/dataview';

@Component({
  selector: 'app-area-dataView',
  templateUrl: './area-dataview.component.html',
  styleUrls: ['./area-dataview.component.scss']
})
export class AreaDataViewComponent implements OnInit {

  areas!: ParkArea[];

  totalRecords = 0;

  @Input()
  pageSize = 10;

  nextRecord = 0;

  @Input()
  loading = false;

  @Output()
  onFetchError = new EventEmitter<string>();

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
      label: "Avarage Price Low to High",
      value: "!avaragePrice"
    },
    {
      label: "Avarage Price High to Low",
      value: "avaragePrice"
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

  @ContentChild(TemplateRef,{static:false})
  testTemplateRef!: TemplateRef<any>;

  searchTOID?: NodeJS.Timeout;
  lastSearchSeconds = -10;
  lastSearchInput: string | null = null;

  orderBy: string | null = null;
  asc: boolean | null = null;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private parkAreaService: ParkAreaService,
    private authService: AuthService
  ) {

  }

  ngOnInit(): void {
    const paramMap = this.route.snapshot.queryParamMap;

    if(paramMap.has("page")) {
      const page = Number(paramMap.get("page"));
      if(page > 1)
        this.nextRecord = page * this.pageSize - this.pageSize;
    }
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
    console.log(event);

  }

  goArea(area:ParkArea) {
    let parkid = this.getParkId();

    this.router.navigateByUrl( this.authService.asManager
      ? RouteUrl.mParkArea(parkid, area.id)
      : RouteUrl.parkArea(parkid,area.id)
    );
  }

  getParkId() {
    return Number(this.route.snapshot.paramMap.get("parkid"));
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
          this.nextRecord,
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
    this.parkAreaService.getAreasPage(
      nextRecord,
      this.pageSize,
      search,
      orderBy,
      asc
    ).subscribe({
      next: response => {
        if(response.headers.has("x-total-records"))
          this.totalRecords = Number(response.headers.get("x-total-records"));

        this.router.navigate([], {
          relativeTo: this.route,
          queryParams: {
            ...this.route.snapshot.queryParams,
            page: (nextRecord + this.pageSize) / this.pageSize
          }
        });

        this.areas = response.body!;
        this.loading = false;
      },
      error: (err: HttpErrorResponse) => {
        this.onFetchError.emit(err.error.message);
      }
    });
  }
}
