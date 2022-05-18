import { HttpErrorResponse } from '@angular/common/http';
import { Component, NgZone, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ParkArea } from '@app/core/models/park-area';
import { ParkSpaceReal } from '@app/core/models/park-space-real';
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
    id: this.getAreaId(),
    parkId: this.getParkId(),
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
        Object.assign(this.area, area);
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
        this.area = {...this.area, spaces: spaces ?? []};
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
    if(this.editing) return;
    if(form.invalid){
      var keys = Object.keys(form.controls);
      keys.forEach(e => {
        form.controls[e].markAsDirty()
      });
      return;
    }
    for(let i=0; i < this.area.spaces.length; i++) {
      let space = this.area.spaces[i];
      if(!space.name || !space.realSpace || space.name.length == 0 || space.name.length > 30) {
        return;
      }
    }

    this.editing = true;
    this.areaService.updateArea({
      ...this.area,
      templateImage: this.extractDataURI(this.area.templateImage) ?? this.area.templateImage
    }, true).subscribe({
      next: area => {
        this.area = area;
        this.area.parkId = this.getParkId();

        this.messageService.add(
          {life:1500,
          severity:'success',
          summary: 'Template Update',
          detail: 'Area template and spaces has been updated successfully'
        });
        this.editing = false;
      },
      error: (err: HttpErrorResponse) => {
        this.messageService.add({
          life:5000,
          severity:'error',
          summary: 'Template Update Fail',
          detail: err.error.message
        });
        this.editing = false;
      }
    });
  }

  onRealSpaceFetchError(err: HttpErrorResponse) {
    this.messageService.add({
      life:5000,
      severity:'error',
      summary: 'Fetch Real Spaces',
      detail: err.error.message
    })
  }

  onRealSpaceAddError(err: HttpErrorResponse) {
    this.messageService.add({
      life:5000,
      severity:'error',
      summary: 'Add Real Space',
      detail: err.error.message
    });
  }

  onRealSpaceDeleteError(err: HttpErrorResponse) {
    this.messageService.add({
      life:5000,
      severity:'error',
      summary: 'Delete Real Space',
      detail: err.error.message
    });
  }

  goArea() {
    const parkid = this.getParkId();
    const areaid = this.getAreaId();
    this.router.navigateByUrl(RouteUrl.mParkArea(parkid, areaid));
  }

  getParkId() {
    return this.route.snapshot.paramMap.get("parkid")!;
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

  extractDataURI(dataURI:string | null) {
    if(!dataURI || !dataURI.match(/data:.+\/.+;base64,.+/)) return null;
    const split = dataURI.split(',');
    const value = split[1];
    const mime = split[0].split(";")[0].split(":")[1]
    return `${mime},${value}`;
  }
}
