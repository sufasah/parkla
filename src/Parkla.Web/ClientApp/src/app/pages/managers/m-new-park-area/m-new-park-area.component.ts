import { HttpErrorResponse } from '@angular/common/http';
import { Component, NgZone, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ParkArea } from '@app/core/models/park-area';
import { ParkAreaService } from '@app/core/services/park-area.service';
import { RouteUrl } from '@app/core/utils/route';
import { Message, MessageService } from 'primeng/api';

@Component({
  selector: 'app-m-new-park-area',
  templateUrl: './m-new-park-area.component.html',
  styleUrls: ['./m-new-park-area.component.scss']
})
export class MNewParkAreaComponent implements OnInit {

  area: ParkArea = <any>{
    reservationsEnabled: true,
    pricings:[],
    spaces: []
  };

  adding = false;

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
    return Number(this.route.snapshot.paramMap.get("parkid"));
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
    for(let i=0; i < this.area.spaces.length; i++) {
      let space = this.area.spaces[i];
      if(!space.name || !space.realSpace || space.name.length == 0 || space.name.length > 30) {
        return;
      }
    }

    this.adding = true;
    this.area.parkId = this.getParkId();
    this.parkAreaService.addArea(this.area).subscribe({
      next: area => {
        this.messageService.add({
          life:1500,
          severity:'success',
          summary: 'Area Added',
          detail: 'Park area has beenadded successfully',
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

  dataURItoBlob(dataURI:string) {
    const split = dataURI.split(',');
    const value = split[1];
    const mime = split[0].split(";")[0].split(":")[1]

    const byteString = window.atob(value);
    const ab = new ArrayBuffer(byteString.length);
    const ia = new Uint8Array(ab);

    for (let i = 0; i < byteString.length; i++)
        ia[i] = byteString.charCodeAt(i);

    var blob = new Blob([ab], {type: mime});
    return blob;
  }
}
