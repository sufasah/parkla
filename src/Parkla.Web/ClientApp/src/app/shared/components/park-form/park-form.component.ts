import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Park } from '@app/core/models/park';
import { makeTomTomMap } from '@app/core/utils/tomtom';
import { Map, Marker } from '@tomtom-international/web-sdk-maps';

@Component({
  selector: 'app-park-form',
  templateUrl: './park-form.component.html',
  styleUrls: ['./park-form.component.scss']
})
export class ParkFormComponent implements OnInit {

  @Output()
  formSubmit = new EventEmitter<NgForm>();

  _park!: Park;
  @Input()
  set park(value: Park) {
    this._park = value
  }
  get park() {
    return this._park;
  }

  @Output()
  parkChange = new EventEmitter<Park>();

  @Input()
  loading = false;

  @Input()
  submitLabel = "Submit";

  latLngMarker?: Marker;
  mapModalVisible = false;

  selectLatLngMap! : Map;

  constructor() { }

  ngOnInit(): void {
  }

  submit(ngForm: NgForm) {
    this.formSubmit.emit(ngForm);
  }


  setLatLng(lat:number, lng:number) {
    this.park.latitude = lat;
    this.park.longitude = lng;

    this.latLngMarker?.remove();
    this.latLngMarker = new Marker()
      .setLngLat([lng,lat])
      .addTo(this.selectLatLngMap);
  }

  addExtra() {
    if(this.park.extras.length < 10)
      this.park.extras.push("");
  }

  removeExtra(index: number) {
    this.park.extras.splice(index,1);
  }

  showMapModal() {
    this.mapModalVisible = true;
    if(!this.selectLatLngMap) {
      setTimeout(() => {
        this.selectLatLngMap = makeTomTomMap("selectLatLngMap");
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

  track(index: number, obj: any): any {
    return index;
  }

}
