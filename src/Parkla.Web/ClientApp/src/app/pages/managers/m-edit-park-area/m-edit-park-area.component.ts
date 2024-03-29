import { HttpErrorResponse } from '@angular/common/http';
import { Component, ElementRef, NgZone, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ParkArea } from '@app/core/models/park-area';
import { ParkAreaService } from '@app/core/services/park-area.service';
import { RouteUrl } from '@app/core/utils/route';
import { EditAreaTemplateComponent } from '@app/shared/components/edit-area-template/edit-area-template.component';
import { MenuItem, Message, MessageService } from 'primeng/api';
@Component({
  selector: 'app-m-edit-park-area',
  templateUrl: './m-edit-park-area.component.html',
  styleUrls: ['./m-edit-park-area.component.scss']
})
export class MEditParkAreaComponent implements OnInit {

  @ViewChild("templateImageRef")
  templateImage!: ElementRef<HTMLImageElement>

  @ViewChild(EditAreaTemplateComponent)
  editAreaTemplateRef!: EditAreaTemplateComponent

  area: ParkArea = <any>{
    pricings:[]
  };

  editing = false;

  bcModel: MenuItem[] = [
    {icon: 'pi pi-map', routerLink: "/"+RouteUrl.mParkMap()},
    {label: 'Park Areas', routerLink: "/"+RouteUrl.mParkAreas(this.getParkId())},
    {label: '' + this.getAreaId(), routerLink: "/"+RouteUrl.mParkArea(this.getParkId(), this.getAreaId())},
    {label: `Edit Park Area`, styleClass: "last-item"},
  ];

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private ngZone: NgZone,
    private messageService: MessageService,
    private areaService: ParkAreaService,
  ) {
  }

  ngOnInit(): void {
    this.areaService.getArea(this.getAreaId()).subscribe({
      next: parkArea => {
        this.area = parkArea;
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
            navigateTo: RouteUrl.mParkAreas(this.getParkId())
          }
        });
      }
    });
  }

  goAreas() {
    const parkid = this.getParkId();
    this.router.navigateByUrl(RouteUrl.mParkAreas(parkid));
  }

  getParkId() {
    return this.route.snapshot.paramMap.get("parkid")!;
  }

  getAreaId() {
    return Number(this.route.snapshot.paramMap.get("areaid"));
  }

  editArea(form: NgForm) {
    if(this.editing) return;
    if(form.invalid){
      var keys = Object.keys(form.controls);
      keys.forEach(e => {
        form.controls[e].markAsDirty()
      });
      return;
    }

    this.editing = true;
    this.area.parkId = this.getParkId();
    this.areaService.updateArea(this.area).subscribe({
      next: area => {
        console.log(area);

        this.area = area;
        this.messageService.add({
          life:1500,
          severity:'success',
          summary: 'Edited',
          detail: 'Park area is edited successfully',
        });
        this.editing = false;
      },
      error: (err: HttpErrorResponse) => {
        this.messageService.add({
          life:5000,
          severity:"error",
          summary: "Edit Fail",
          detail: err.error.message,
          icon: "pi-lock",
        });
        this.editing = false;
      }
    });
  }

  messageClose(message: Message) {
    if(message.data && message.data.navigate) {
      this.ngZone.run(() => {
        this.router.navigateByUrl(RouteUrl.mParkAreas(this.getParkId()));
      });
    }
  }
}
