import { AfterViewInit, Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Park } from '@app/core/models/park';
import { RouteUrl } from '@app/core/utils/route';
import { Map, Marker } from '@tomtom-international/web-sdk-maps';
import { Message, MessageService } from 'primeng/api';
import { delay, of } from 'rxjs';
import { makeTomTomMap } from '@app/core/utils/tomtom';
import { ParkService } from '@app/core/services/park.service';
import { Route } from '@tomtom-international/web-sdk-services';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '@app/core/services/auth.service';

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
    private route: ActivatedRoute,
    private parkService: ParkService,
    private messageService: MessageService,
    private authService: AuthService) { }

  ngOnInit(): void {

    this.route.paramMap.subscribe(params => {
      const id = Number(params.get("parkid"));
      this.parkService.getPark(id).subscribe({
        next: (park) => {
          this.park = park;
        },
        error: (err: HttpErrorResponse) => {
          this.messageService.add({
            life:5000,
            severity:"error",
            summary: "Fetching Park",
            detail: err.error.message,
            icon: "pi-lock",
            data: {
              navigate: true,
              navigateTo: RouteUrl.mParkMap()
            }
          })
        }
      });
    })

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
    if(this.editing) return;
    if(form.invalid){
      var keys = Object.keys(form.controls);
      keys.forEach(e => {
        form.controls[e].markAsDirty()
      });
      return;
    }

    this.park.extras = this.extrasModel.map(x => x.val);
    this.editing = true;
    this.park.user = <any>{id: this.authService.accessToken?.sub};

    this.parkService.updatePark(this.park).subscribe({
      next: park => {
        this.park = park;
        this.messageService.add({
          life:1500,
          severity:'success',
          summary: 'Edited',
          detail: 'Parking lot is edited successfully',
        });
        this.editing = false;
      },
      error: (err: HttpErrorResponse) => {
        this.messageService.add({
          life:5000,
          severity:"error",
          summary: "Edit Fail",
          detail: err.error.message,
          icon: "pi-lock"
        })
        this.editing = false;
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
    if(message.data && message.data.navigate) {
      this.router.navigateByUrl(message.data.navigateTo);
    }
  }

}
