import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RSRoute } from '@app/core/constants/ref-sharing.const';
import { ParkArea } from '@app/core/models/parking-lot';
import { RefSharingService } from '@app/core/services/ref-sharing.service';
import { RouteUrl } from '@app/core/utils/route.util';
import { mockAreas } from '@app/mock-data/areas';
import { SelectItem } from 'primeng/api';
import { DataView } from 'primeng/dataview';

@Component({
  selector: 'app-area-dataView',
  templateUrl: './area-dataview.component.html',
  styleUrls: ['./area-dataview.component.scss']
})
export class AreaDataViewComponent implements OnInit {


  @Input()
  areas!: ParkArea[];

  sortOptions: SelectItem[];

  sortOrder!: number;

  sortField!: string;

  @ViewChild("dv")
  dataView!: DataView;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private refSharingService: RefSharingService) {

    this.sortOptions = [
      {
        label: "Min Price Low to High",
        value: "!minPrice.balance"
      },
      {
        label: "Min Price High to Low",
        value: "minPrice.balance"
      },
      {
        label: "Avarage Price Low to High",
        value: "!avgPrice.balance"
      },
      {
        label: "Avarage Price High to Low",
        value: "avgPrice.balance"
      },
      {
        label: "Max Price Low to High",
        value: "!maxPrice.balance"
      },
      {
        label: "Max Price High to Low",
        value: "maxPrice.balance"
      },
    ];
  }

  ngOnInit(): void {
    this.areas[0]
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

  onSearchInput(event:any) {
    //contains"(Default), "startsWith", "endsWith", "equals", "notEquals", "in", "lt", "lte", "gt" and "gte"
    this.dataView.filter(event.target.value,"contains");

  }

  goArea(area:ParkArea) {
    this.refSharingService.setData(RSRoute.areasSelectedArea,area);
    let parkid = this.route.snapshot.params["parkid"];
    this.router.navigate([RouteUrl.parkArea(parkid,area.id)])
  }
}
