import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { ttkey } from '@app/core/constants/private.const';
import { RSRoute } from '@app/core/constants/ref-sharing.const';
import { ParkingLot } from '@app/core/models/parking-lot';
import { RefSharingService } from '@app/core/services/ref-sharing.service';
import { RouteUrl } from '@app/core/utils/route.util';
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

  park: ParkingLot = <any>{};

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
    private refSharingService: RefSharingService,
    private router: Router,
    private messageService: MessageService) { }

  ngOnInit(): void {

  }

  ngAfterViewInit(): void {

  }

  goMap() {
    this.refSharingService.removeData(RSRoute.mapSelectedPark);
    this.router.navigateByUrl(RouteUrl.mParkMap());
  }

  addPark(form: NgForm) {
    this.park.extras = this.extrasModel.map(x => x.val);

    console.log(this.park);
    console.log(form);

    if(form.invalid){
      var keys = Object.keys(form.controls);
      keys.forEach(e => {
        form.controls[e].markAsDirty()
      });
      return;
    }

    this.adding = true;
    //add opeartion to the server and result
    of(false).pipe(delay(2000)).subscribe(success => {
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
        this.selectLatLngMap = map({
          key: ttkey,
          container: "selectLatLngMap",
          zoom: 12,
          language: "tr-TR",
          center: {
            lat: 41.015137,
            lng: 28.979530,
          },
        });

        this.selectLatLngMap.addControl(new NavigationControl());
        this.selectLatLngMap.addControl(new FullscreenControl());
        this.selectLatLngMap.addControl(new GeolocateControl({
          trackUserLocation: true,
          showUserLocation: true,
          positionOptions: {
            enableHighAccuracy: true,
          },
          showAccuracyCircle: true
        }));

        this.selectLatLngMap.on("click",(event) => {
          this.setLatLng(event.lngLat.lat,event.lngLat.lng);
        })
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
