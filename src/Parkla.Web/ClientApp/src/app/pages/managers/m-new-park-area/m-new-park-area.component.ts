import { AfterViewInit, Component, ContentChild, ElementRef, OnInit, Renderer2, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ParkArea } from '@app/core/models/park-area';
import { RouteUrl } from '@app/core/utils/route.util';
import { MessageService } from 'primeng/api';
import { delay, of } from 'rxjs';

@Component({
  selector: 'app-m-new-park-area',
  templateUrl: './m-new-park-area.component.html',
  styleUrls: ['./m-new-park-area.component.scss']
})
export class MNewParkAreaComponent implements OnInit, AfterViewInit {

  @ContentChild("imgRef")
  templateImage: HTMLImageElement

  parkId: number;
  area: ParkArea = <any>{
    pricings: []
  };
  adding = false;
  templateModalVisible = false;
  timeUnitOptions = ["minutes", "hours", "days", "months"]

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService)
  {
    this.parkId = route.snapshot.params.parkid;

    this.templateImage = document.createElement("img");
  }
  ngAfterViewInit(): void {
    setTimeout(() => {

      console.log(this.templateImage);
    }, 3000);

  }

  ngOnInit(): void {
    this.templateImage.onload = () => {

    };

    this.templateImage.onerror = () => {
    }

    this.templateImage.src = this.area.templateImg;
  }

  goAreas() {
    this.router.navigateByUrl(RouteUrl.mParkAreas(this.parkId));
  }

  addArea(form: NgForm) {

    console.log(this.area);
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
    of(true).pipe(delay(2000)).subscribe(success => {
      if(success){
        this.messageService.add({
          life:1500,
          severity:'success',
          summary: 'Added',
          detail: 'Park area is added successfully',
        })
      }
      else {
        this.messageService.add({
          life:1500,
          severity:"error",
          summary: "Add Fail",
          detail: "Park area isn't added successfully",
          icon: "pi-lock",
        })
      }
      this.adding = false;
    });
  }

  addPricing() {
    this.area.pricings?.push(<any>{
      price: {}
    });
  }

  removePricing(index: number) {
    this.area.pricings?.splice(index,1);
  }

  showTemplateModal() {
    this.templateModalVisible = true;
  }

  templateModalSelect() {
    this.templateModalVisible = false;
  }

  messageClose() {
  }

  isTimeInterval(val: any) {
    return 'beginningTime' in val && 'endTime' in val;
  }

  isPerTime(val: any) {
    return 'timeUnit' in val && 'timeAmount' in val;
  }
}
