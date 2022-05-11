import { AfterViewInit, Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { Park } from '@app/core/models/park';
import { RouteUrl } from '@app/core/utils/route';
import { Map, Marker } from '@tomtom-international/web-sdk-maps';
import { MessageService } from 'primeng/api';
import { delay, of } from 'rxjs';
import { makeTomTomMap } from '@app/core/utils/tomtom';

@Component({
  selector: 'app-m-edit-park',
  templateUrl: './m-edit-park.component.html',
  styleUrls: ['./m-edit-park.component.scss']
})
export class MEditParkComponent implements OnInit, AfterViewInit {

  selectLatLngMap! : Map;

  park: Park = <any>{};

  extrasModel: {val:string}[] = []

  latLngMarker?: Marker;

  editing = false;
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
    private messageService: MessageService) { }

  ngOnInit(): void {
    // get park datas
    this.park = <any>{
      id: 6,
      name: "parknamehere",
      location: "locahitonhere",
      lat: 42,
      lng: 16,
      extras: ["extra1", "extra2", "extra3"]
    };

    this.extrasModel = this.park.extras.map(x => ({val:x}));
  }

  ngAfterViewInit(): void {

  }

  goMap() {
    this.router.navigateByUrl(RouteUrl.mParkMap());
  }

  editPark(form: NgForm) {
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

    this.editing = true;
    //add opeartion to the server and result
    of(true).pipe(delay(2000)).subscribe(success => {
      if(success){
        this.messageService.add({
          life:1500,
          severity:'success',
          summary: 'Edited',
          detail: 'Parking lot is edited successfully',
        })
      }
      else {
        this.messageService.add({
          life:1500,
          severity:"error",
          summary: "Edit Fail",
          detail: "Parking lot isn't edited successfully",
          icon: "pi-lock",
        })
      }
      this.editing = false;
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

  messageClose() {
  }

}
