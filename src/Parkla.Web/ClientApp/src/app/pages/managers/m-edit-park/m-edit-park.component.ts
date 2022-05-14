import { AfterViewInit, Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Park } from '@app/core/models/park';
import { RouteUrl } from '@app/core/utils/route';
import { Map, Marker } from '@tomtom-international/web-sdk-maps';
import { Message, MessageService } from 'primeng/api';
import { ParkService } from '@app/core/services/park.service';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '@app/core/services/auth.service';

@Component({
  selector: 'app-m-edit-park',
  templateUrl: './m-edit-park.component.html',
  styleUrls: ['./m-edit-park.component.scss']
})
export class MEditParkComponent implements OnInit, AfterViewInit {
  park: Park = <any>{extras: []};

  editing = false;

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

    this.editing = true;
    this.park.user = <any>{id: this.authService.accessToken?.sub};
    console.log(this.park);

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

  messageClose(message: Message) {
    if(message.data && message.data.navigate) {
      this.router.navigateByUrl(message.data.navigateTo);
    }
  }

}
