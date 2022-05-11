import { HttpErrorResponse } from '@angular/common/http';
import { AfterViewInit, Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { Park } from '@app/core/models/park';
import { ParkService } from '@app/core/services/park.service';
import { RouteUrl } from '@app/core/utils/route';
import { makeTomTomMap } from '@app/core/utils/tomtom';
import { Map, Marker } from '@tomtom-international/web-sdk-maps';
import { Message, MessageService } from 'primeng/api';

@Component({
  selector: 'app-m-new-park',
  templateUrl: './m-new-park.component.html',
  styleUrls: ['./m-new-park.component.scss']
})
export class MNewParkComponent implements OnInit, AfterViewInit {

  selectLatLngMap! : Map;

  park: Park = <any>{};

  extrasModel: {val:string}[] = []

  latLngMarker?: Marker;

  adding = false;
  mapModalVisible = false;

  setLatLng(lat:number, lng:number) {
    this.park.latitude = lat;
    this.park.longitude = lng;

    this.latLngMarker?.remove();
    this.latLngMarker = new Marker()
      .setLngLat([lng,lat])
      .addTo(this.selectLatLngMap);
  }

  constructor(
    private router: Router,
    private messageService: MessageService,
    private parkService: ParkService) { }

  ngOnInit(): void {

  }

  ngAfterViewInit(): void {

  }

  goMap() {
    this.router.navigateByUrl(RouteUrl.mParkMap());
  }

  addPark(form: NgForm) {
    if(form.invalid){
      var keys = Object.keys(form.controls);
      keys.forEach(e => {
        form.controls[e].markAsDirty()
      });
      return;
    }

    this.park.extras = this.extrasModel.map(x => x.val);
    this.adding = true;

    this.parkService.addPark(this.park).subscribe({
      next: park => {
        this.messageService.add({
          life:1500,
          severity:'success',
          summary: 'Added',
          detail: 'Parking lot is added successfully',
          data: {
            navigate: true,
            navigateTo: RouteUrl.mParkMap()
          }
        });
      },
      error: (err: HttpErrorResponse) => {
        this.messageService.add({
          life:1500,
          severity:"error",
          summary: "Add Fail",
          detail: err.error.message,
          icon: "pi-lock",
        })
        this.adding = false;
      }
    });
  }

  addExtra() {
    this.extrasModel.push({val:""});
  }

  removeExtra(index: number) {
    this.extrasModel.splice(index,1);
  }

  showMapModal() {
    this.mapModalVisible = true;
    if(!this.selectLatLngMap) {
      setTimeout(() => {
        this.selectLatLngMap = makeTomTomMap();
        this.selectLatLngMap.on("click",(event) => {
          this.setLatLng(event.lngLat.lat,event.lngLat.lng);
        });
      }, 0);
    }
  }

  mapModalSelect() {
    this.mapModalVisible = false;
  }

  mapModalCancel() {
    this.park.latitude = <any>undefined;
    this.park.longitude = <any>undefined;
    this.latLngMarker?.remove();
    this.mapModalVisible = false;
  }

  messageClose(message: Message) {
    if(message.data?.navigate) {
      this.router.navigateByUrl(message.data.navigateTo);
    }
  }
}
