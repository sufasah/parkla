import { HttpErrorResponse } from '@angular/common/http';
import { Component, NgZone, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ParkArea } from '@app/core/models/park-area';
import { ParkAreaService } from '@app/core/services/park-area.service';
import { RouteUrl } from '@app/core/utils/route';
import { MenuItem, Message, MessageService } from 'primeng/api';

@Component({
  selector: 'app-m-new-park-area',
  templateUrl: './m-new-park-area.component.html',
  styleUrls: ['./m-new-park-area.component.scss']
})
export class MNewParkAreaComponent implements OnInit {

  area: ParkArea = <any>{
    reservationsEnabled: true,
    pricings:[]
  };

  adding = false;

  bcModel: MenuItem[] = [
    {icon: 'pi pi-map', routerLink: "/"+RouteUrl.mParkMap()},
    {label: 'Park Areas', routerLink: "/"+RouteUrl.mParkAreas(this.getParkId())},
    {label: `New Park Area`, styleClass: "last-item"},
  ];

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService,
    private ngZone: NgZone,
    private parkAreaService: ParkAreaService
  ) {

  }

  ngOnInit(): void {
  }

  goAreas() {
    const parkid = this.getParkId();;
    this.router.navigateByUrl(RouteUrl.mParkAreas(parkid));
  }

  getParkId() {
    return this.route.snapshot.paramMap.get("parkid")!;
  }

  addArea(form: NgForm) {
    if(this.adding) return;
    if(form.invalid){
      var keys = Object.keys(form.controls);
      keys.forEach(e => {
        form.controls[e].markAsDirty()
      });
      return;
    }

    this.adding = true;
    this.area.parkId = this.getParkId();
    this.parkAreaService.addArea(this.area).subscribe({
      next: area => {
        this.messageService.add({
          life:1500,
          severity:'success',
          summary: 'Area Added',
          detail: 'Park area has been added successfully',
          data: {
            navigate: true,
            navigateTo: RouteUrl.mParkAreas(this.getParkId())
          }
        });
        this.adding = false;
      },
      error: (err:HttpErrorResponse) => {
        this.messageService.add({
          life:5000,
          severity:"error",
          summary: "Add Fail",
          detail: err.error.message,
          icon: "pi-lock",
        });
        this.adding = false;
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
