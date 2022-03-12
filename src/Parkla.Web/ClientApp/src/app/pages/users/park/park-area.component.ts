import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RSRoute } from '@app/core/constants/ref-sharing.const';
import { ParkArea, ParkingLot, ParkSpace } from '@app/core/models/parking-lot';
import { RefSharingService } from '@app/core/services/ref-sharing.service';
import { RouteUrl } from '@app/core/utils/route.util';
import { mockAreas } from '@app/mock-data/areas';
import { ParkTemplateComponent } from '@app/shared/components/park-template/park-template.component';
import { MessageService } from 'primeng/api';

declare var $:any;

  @Component({
  selector: 'app-park',
  templateUrl: './park-area.component.html',
  styleUrls: ['./park-area.component.scss']
})
export class ParkAreaComponent implements OnInit, AfterViewInit {

  @ViewChild("parkTemplate")
  parkTemlate?: ParkTemplateComponent;

  selectedArea!: ParkArea;

  selectedSpace?: ParkSpace;

  dialogVisible = false;

  timeRange:[Date?, Date?] = [
    new Date(),
    new Date(Date.now()+60000*15)
  ];

  minDate = new Date();

  maxDate = new Date(Date.now()+1000*60*60*24*3);

  constructor(
    private refSharingService: RefSharingService,
    private router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService) { }

  ngOnInit(): void {
    let area = this.refSharingService.getData<ParkArea>(RSRoute.areasSelectedArea);

    if(!!area)
      this.selectedArea = area;
    else {
      //get from server
      this.selectedArea = mockAreas[0];
    }
  }

  ngAfterViewInit(): void {

  }

  spaceClicked(space: ParkSpace) {
    this.selectedSpace = space;

    if(this.selectedArea.reservationsEnabled) {
      if(space.status == "empty") {
        this.showReserveModal();
        //show time interval selected
        //show pricig table
        //show reservation intervals and available intervals between them
        //if reserved show reserved user's username
        //if user wallet is not enough it must not be possible to confirm
      }
      else if((<any>space).isReserved) {
        this.showReserveModal();
      }
      else {
        if(!this.selectedArea.notReservedOccupiable) {
          this.showReserveModal();
        }
        else {
          this.messageService.add({
            life:1500,
            severity:'error',
            summary: 'Occupied Reservation',
            detail: 'It is not possible to reserve occupied space.',
          });
        }
      }
    }
  }

  reserveSpace() {
    console.log("reserve space");
  }

  goAreas() {
    let parkid = this.route.snapshot.params["parkid"];
    this.refSharingService.removeData(RSRoute.areasSelectedArea);
    this.router.navigate([RouteUrl.parkAreas(parkid)]);
  }

  showReserveModal() {
    this.dialogVisible = true;
  }

  timeRangeChange(timeRange:any) {
    this.timeRange = timeRange;

    if(!this.timeRange[0] || !this.timeRange[1]) return;

    this.selectedArea.spaces.forEach(space => {
      if(this.selectedArea.reservationsEnabled) {
        space.reservations?.forEach(reservation => {
          (<any>space).isReserved =
            (reservation.startTime < this.timeRange[1]! &&
            reservation.startTime >= this.timeRange[0]!) ||
            (reservation.endTime > this.timeRange[0]! &&
            reservation.endTime <= this.timeRange[1]!);
        })
      }
    })

    this.parkTemlate?.drawCanvas();
  }
}
