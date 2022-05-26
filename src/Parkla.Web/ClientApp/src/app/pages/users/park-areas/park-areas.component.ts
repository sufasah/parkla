import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ParkArea } from '@app/core/models/park-area';
import { AreaDataViewComponent } from '@app/shared/components/area-dataview/area-dataview.component';
import { MessageService } from 'primeng/api';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-areas',
  templateUrl: './park-areas.component.html',
  styleUrls: ['./park-areas.component.scss']
})
export class ParkAreasComponent implements OnInit {

  @ViewChild(AreaDataViewComponent)
  dataView!: AreaDataViewComponent;

  readonly pageSize = 6;

  parkid!: string;
  loading = false;

  unsubscribe: Subscription[] = [];

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService
  ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(paramMap => {
      this.parkid = paramMap.get("parkid")!;
    });
  }

  goMap() {
    this.router.navigate([".."]);
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
}
