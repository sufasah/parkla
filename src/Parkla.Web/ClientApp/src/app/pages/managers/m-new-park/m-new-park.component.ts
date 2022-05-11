import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { ttkey } from '@app/core/constants/private';
import { RSRoute } from '@app/core/constants/ref-sharing';
import { Park } from '@app/core/models/park';
import { ParkService } from '@app/core/services/park.service';
import { RefSharingService } from '@app/core/services/ref-sharing.service';
import { RouteUrl } from '@app/core/utils/route';
import { makeTomTomMap } from '@app/core/utils/tomtom';
import { FullscreenControl, GeolocateControl, map, Map, Marker, NavigationControl } from '@tomtom-international/web-sdk-maps';
import { MessageService } from 'primeng/api';
import { delay, of } from 'rxjs';

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
    this.park.lat = lat;
    this.park.lng = lng;

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

    //add opeartion to the server and result
    /*this.parkService.addPark().subscribe(success => {
      if(success){
        this.messageService.add({
          life:1500,
          severity:'success',
          summary: 'Added',
          detail: 'Parking lot is added successfully',
        })
      }
      else {
        this.messageService.add({
          life:1500,
          severity:"error",
          summary: "Add Fail",
          detail: "Parking lot isn't added successfully",
          icon: "pi-lock",
        })
      }
      this.adding = false;
    });*/
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
    this.park.lat = <any>undefined;
    this.park.lng = <any>undefined;
    this.latLngMarker?.remove();
    this.mapModalVisible = false;
  }

  messageClose() {
  }
}
