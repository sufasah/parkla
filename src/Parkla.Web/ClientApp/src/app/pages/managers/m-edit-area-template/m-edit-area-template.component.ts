import { HttpErrorResponse } from '@angular/common/http';
import { Component, NgZone, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ParkArea } from '@app/core/models/park-area';
import { ParkAreaService } from '@app/core/services/park-area.service';
import { ParkSpaceService } from '@app/core/services/park-space.service';
import { RouteUrl } from '@app/core/utils/route';
import { Message, MessageService } from 'primeng/api';

@Component({
  selector: 'app-m-edit-area-template',
  templateUrl: './m-edit-area-template.component.html',
  styleUrls: ['./m-edit-area-template.component.scss']
})
export class MEditAreaTemplateComponent implements OnInit {

  area: ParkArea = <any>{
    spaces: []
  };

  editing = false;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private areaService: ParkAreaService,
    private spaceService: ParkSpaceService,
    private messageService: MessageService,
    private ngZone: NgZone
  ) {

  }

  ngOnInit(): void {
    this.getData();
  }

  getData() {
    this.areaService.getArea(this.getAreaId()).subscribe({
      next: area => {
        this.area = area;
        this.getAreaSpaces();
      },
      error: (err: HttpErrorResponse) => {
        this.messageService.add({
          life:5000,
          severity:"error",
          summary: "Fetch Park Area",
          detail: err.error.message,
          icon: "pi-lock",
          data: {
            navigate: true,
            navigateTo: RouteUrl.mParkArea(this.getParkId(), this.getAreaId())
          }
        });
      }
    });
  }

  getAreaSpaces() {
    this.spaceService.getAreaParkSpaces(this.getAreaId(), false).subscribe({
      next: spaces => {
        this.area.spaces = spaces;
      },
      error: (err: HttpErrorResponse) => {
        this.messageService.add({
          life:5000,
          severity:"error",
          summary: "Fetch Park Spaces",
          detail: err.error.message,
          icon: "pi-lock",
          data: {
            navigate: true,
            navigateTo: RouteUrl.mParkArea(this.getParkId(), this.getAreaId())
          }
        });
      }
    });
  }

  editTemplate(form: NgForm) {
    console.log(form);

    if(this.editing) return;
    for(let i=0; i < this.area.spaces.length; i++) {
      let space = this.area.spaces[i];
      if(!space.name || !space.realSpace || space.name.length == 0 || space.name.length > 30) {
        return;
      }
    }

    this.editing = true;
  }

  goArea() {
    const parkid = this.getParkId();
    const areaid = this.getAreaId();
    this.router.navigateByUrl(RouteUrl.mParkArea(parkid, areaid));
  }

  getParkId() {
    return Number(this.route.snapshot.paramMap.get("parkid"))
  }

  getAreaId() {
    return Number(this.route.snapshot.paramMap.get("areaid"));
  }

  messageClose(message: Message) {
    if(message.data && message.data.navigate) {
      this.ngZone.run(() => {
        this.router.navigateByUrl(message.data.navigateTo);
      });
    }
  }
}
