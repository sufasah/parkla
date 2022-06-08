import { HttpErrorResponse } from '@angular/common/http';
import { AfterViewInit, Component, NgZone, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { Park } from '@app/core/models/park';
import { AuthService } from '@app/core/services/auth.service';
import { ParkService } from '@app/core/services/park.service';
import { RouteUrl } from '@app/core/utils/route';
import { MenuItem, Message, MessageService } from 'primeng/api';

@Component({
  selector: 'app-m-new-park',
  templateUrl: './m-new-park.component.html',
  styleUrls: ['./m-new-park.component.scss']
})
export class MNewParkComponent implements OnInit, AfterViewInit {

  park: Park = <any>{extras: []};

  adding = false;

  bcModel: MenuItem[] = [
    {icon: 'pi pi-map', routerLink: "/"+RouteUrl.mParkMap()},
    {label: `New Park`, styleClass: "last-item"},
  ];

  constructor(
    private router: Router,
    private messageService: MessageService,
    private parkService: ParkService,
    private ngZone: NgZone,
    private authService: AuthService) { }

  ngOnInit(): void {
    this.park.user = <any>{id: this.authService.accessToken?.sub};
  }

  ngAfterViewInit(): void {

  }

  goMap() {
    this.router.navigateByUrl(RouteUrl.mParkMap());
  }

  addPark(form: NgForm) {
    if(this.adding) return;
    if(form.invalid){
      var keys = Object.keys(form.controls);
      keys.forEach(e => {
        form.controls[e].markAsDirty()
      });
      return;
    }

    this.adding = true;

    this.parkService.addPark(this.park).subscribe({
      next: park => {
        this.messageService.add({
          life:1500,
          severity:'success',
          summary: 'Added',
          detail: 'Parking lot has been added successfully',
          data: {
            navigate: true,
            navigateTo: RouteUrl.mParkMap()
          }
        });
      },
      error: (err: HttpErrorResponse) => {
        this.messageService.add({
          life:5000,
          severity:"error",
          summary: "Add Fail",
          detail: err.error.message,
          icon: "pi-lock",
        })
        this.adding = false;
      }
    });
  }

  messageClose(message: Message) {
    if(message.data?.navigate) {
      this.ngZone.run(() => {
        this.router.navigateByUrl(message.data.navigateTo);
      });
    }
  }
}
