import { HttpErrorResponse } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ChangablePark, Park } from '@app/core/models/park';
import { AuthService } from '@app/core/services/auth.service';
import { ParkService } from '@app/core/services/park.service';
import { RouteUrl } from '@app/core/utils/route';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-map-dialog',
  templateUrl: './map-dialog.component.html',
  styleUrls: ['./map-dialog.component.scss']
})
export class MapDialogComponent implements OnInit {

  @Input()
  visible = false;

  @Input()
  managerMode = false;

  park!: Park;

  parkChangeSubscription?: Subscription;

  extraStyleColors = [
    "--green-500","--yellow-500","--indigo-500","--cyan-600","--red-500","--teal-500","--blue-500","--purple-500","--gray-900","--orange-600"
  ];

  get spaceCount() {
    let total = this.park.emptySpace +
      this.park.occupiedSpace;

    return total == 0 ? 1 : total;
  }

  constructor(
    private router: Router,
    private authService: AuthService,
    private confirmService: ConfirmationService,
    private messageService: MessageService,
    private parkService: ParkService
  ) { }

  ngOnInit(): void {

  }

  showDialog(park: ChangablePark) {
    this.parkChangeSubscription?.unsubscribe();
    this.park = park;

    this.parkChangeSubscription = park.subject.subscribe(() => {
      this.park = {...park};
    });
    this.visible = true;
  }

  navigateToParkAreas(park:Park) {

    this.router.navigateByUrl(this.authService.asManager
      ? RouteUrl.mParkAreas(park.id)
      : RouteUrl.parkAreas(park.id)
    );
  }

  editPark(park: Park) {
    this.router.navigateByUrl(RouteUrl.mEditPark(park.id));
  }

  deletePark(park: Park) {
    this.confirmService.confirm({
      message: `Are you sure to delete the park with '${park.name}' name?`,
      accept: () => {
        this.parkService.deletePark(park).subscribe({
          next: () => {
            this.messageService.add({
              summary: "Park Deletion",
              closable: true,
              severity: "success",
              life:1500,
              detail: `The park with '${park.name}' name is deleted.`
            });
          },
          error: (err: HttpErrorResponse) => {
            this.messageService.add({
              summary: "Park Deletion",
              closable: true,
              severity: "error",
              life:5000,
              detail: err.error.message
            });
          }
        })

        this.visible = false;
      }
    });
  }
}
