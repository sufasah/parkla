import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ParkArea } from '@app/core/models/park-area';
import { ParkSpace, SpacePath } from '@app/core/models/park-space';
import { ParkSpaceReal } from '@app/core/models/park-space-real';
import { RouteUrl } from '@app/core/utils/route';
import { mockAreas } from '@app/mock-data/areas';
import { EditAreaTemplateComponent } from '@app/shared/components/edit-area-template/edit-area-template.component';
import { ConfirmationService, Message, MessageService } from 'primeng/api';
import { Table } from 'primeng/table';
import { delay, of } from 'rxjs';
@Component({
  selector: 'app-m-edit-park-area',
  templateUrl: './m-edit-park-area.component.html',
  styleUrls: ['./m-edit-park-area.component.scss']
})
export class MEditParkAreaComponent implements OnInit {

  @ViewChild("templateImageRef")
  templateImage!: ElementRef<HTMLImageElement>

  @ViewChild(EditAreaTemplateComponent)
  editAreaTemplateRef!: EditAreaTemplateComponent

  area: ParkArea = <any>{
    pricings:[],
    spaces: []
  };

  editing = false;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService
  ) {
  }

  ngOnInit(): void {
    // get park from service with parkid
    this.area = mockAreas[0];
  }

  goAreas() {
    const parkid = this.getParkId();
    this.router.navigateByUrl(RouteUrl.mParkAreas(parkid));
  }

  getParkId() {
    return Number(this.route.snapshot.paramMap.get("parkid"));
  }

  editArea(form: NgForm) {
    if(this.editing) return;
    if(form.invalid){
      var keys = Object.keys(form.controls);
      keys.forEach(e => {
        form.controls[e].markAsDirty()
      });
      return;
    }
    for(let i=0; i < this.area.spaces.length; i++) {
      let space = this.area.spaces[i];
      if(!space.name || !space.realSpace || space.name.length == 0 || space.name.length > 30) {
        return;
      }
    }

    this.editing = true;
    //add opeartion to the server and result
    of(true).pipe(delay(2000)).subscribe(success => {
      if(success){
        this.messageService.add({
          life:1500,
          severity:'success',
          summary: 'Edited',
          detail: 'Park area is edited successfully',
        })
      }
      else {
        this.messageService.add({
          life:5000,
          severity:"error",
          summary: "Edit Fail",
          detail: "Park area isn't edited successfully",
          icon: "pi-lock",
        })
      }
      this.editing = false;
    });
  }

  messageClose(message: Message) {
    if(message.data && message.data.navigate) {
      this.router.navigateByUrl(RouteUrl.mParkAreas(this.getParkId()));
    }
  }

  dataURItoBlob(dataURI:string) {
    const split = dataURI.split(',');
    const value = split[1];
    const mime = split[0].split(";")[0].split(":")[1]

    const byteString = window.atob(value);
    const ab = new ArrayBuffer(byteString.length);
    const ia = new Uint8Array(ab);

    for (let i = 0; i < byteString.length; i++)
        ia[i] = byteString.charCodeAt(i);

    var blob = new Blob([ab], {type: mime});
    return blob;
  }
}
