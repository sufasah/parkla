import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ParkArea } from '@app/core/models/park-area';
import { ParkSpace, SpacePath } from '@app/core/models/park-space';
import { ParkSpaceReal } from '@app/core/models/park-space-real';
import { RouteUrl } from '@app/core/utils/route.util';
import { mockAreas } from '@app/mock-data/areas';
import { EditAreaTemplateComponent } from '@app/shared/components/edit-area-template/edit-area-template.component';
import { ConfirmationService, MessageService } from 'primeng/api';
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

  parkId: number;
  area: ParkArea = <any>{
    pricings:[],
    spaces: []
  };
  editing = false;
  templateModalVisible = false;
  spaceModalVisible = false;
  timeUnitOptions = ["minutes", "hours", "days", "months"]
  imageLoading = true;
  spaceAdding = false;
  editingSpace: ParkSpace = <any>{};
  realSpaces: ParkSpaceReal[] = <any>[{
    id: 1,
    name: "realspace"
  }, {
    id: 2,
    name: "x park y area space"
  }, {
    id: 3,
    name: "wireless spacecode2512"
  },{
    id: 4,
    name: "realspace"
  }, {
    id: 5,
    name: "x park y area space"
  }, {
    id: 6,
    name: "wireless spacecode2512"
  },{
    id: 7,
    name: "realspace"
  }, {
    id: 8,
    name: "x park y area space"
  }, {
    id: 9,
    name: "wireless spacecode2512"
  }];

  _selectedRealSpace?: ParkSpaceReal;
  set selectedRealSpace(value: ParkSpaceReal | undefined) {
    this._selectedRealSpace = value;
    this.editingSpace.realSpace = value ? {...value} : undefined;
  }
  get selectedRealSpace() {
    return this._selectedRealSpace;
  }

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService,
    private confirmationService: ConfirmationService)
  {
    this.parkId = route.snapshot.params.parkid;
  }

  ngOnInit(): void {
    // get park from service with parkid
    this.area = mockAreas[0];
  }

  ngAfterViewInit(): void {
    this.templateImage.nativeElement.onload = (event) => {
      this.imageLoading = false;
    };

    this.templateImage.nativeElement.onerror = (event) => {
      this.templateImage.nativeElement.src = "https://nebosan.com.tr/wp-content/uploads/2018/06/no-image.jpg";
    }

    this.templateImage.nativeElement.src = this.area.templateImg;


  }

  goAreas() {
    this.router.navigateByUrl(RouteUrl.mParkAreas(this.parkId));
  }

  editArea(form: NgForm) {

    console.log(this.area);
    console.log(form);

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
          life:1500,
          severity:"error",
          summary: "Edit Fail",
          detail: "Park area isn't edited successfully",
          icon: "pi-lock",
        })
      }
      this.editing = false;
    });
  }

  addPricing() {
    if(!this.area.pricings)
      this.area.pricings = [];

    this.area.pricings?.push(<any>{
      price: {},
    });
  }

  removePricing(index: number) {
    this.area.pricings?.splice(index,1);
  }

  showTemplateModal() {
    this.templateModalVisible = true;
  }

  spaceModalDone() {
    this.spaceModalVisible = false;
  }

  editTemplateDone() {
    this.spaceAdding = false;
    this.templateModalVisible = false;
  }

  addTemplateSpace() {
    this.spaceAdding = !this.spaceAdding;
  }

  addTemplateSpaceDone(spacePath: SpacePath) {
    this.area.spaces.push(<any>{
      templatePath: [...spacePath],
    });
  }

  setTemplateImageButton(event: Event) {
    this.spaceAdding = false;

    const btn = <HTMLButtonElement>event.target;
    const input = <HTMLInputElement>btn.nextElementSibling;

    input.click()
  }

  setTemplateImage(event: Event) {
    const input = <HTMLInputElement>event.target;
    const fr = new FileReader();
    fr.readAsDataURL(input.files![0]);
    fr.onload = (event) => {
      this.area = {...this.area, templateImg: <string>fr.result};
    }
  }

  clearTemplateSpace() {
    this.spaceAdding = false;
    this.confirmationService.confirm({
      header: "Clear",
      message: "Are you sure to clear all spaces on park area?",
      accept: () => {
        this.area = {...this.area, spaces: []};
      },
      icon: "pi pi-trash"
    })
  }

  spaceClicked(space: ParkSpace) {
    this.spaceModalVisible = true;
    this.spaceAdding = false;
    this.editingSpace = space;
  }

  spaceRightClicked(space: ParkSpace) {
    this.confirmationService.confirm({
      header: "Delete Space",
      icon: "pi pi-trash",
      message: "Are you sure to delete the selected space?",
      accept: () => {
        let index = this.area.spaces.indexOf(space);
        if(index != -1) {
          this.area.spaces.splice(index,1);
          this.editAreaTemplateRef.drawCanvas();

        }
        this.spaceModalVisible = false;
      }
    });
  }

  clearTable(table: Table, searchInput: HTMLInputElement) {
    table.clear();
    searchInput.value = "";
  }

  messageClose() {
  }

  isTimeInterval(val: any) {
    return 'beginningTime' in val && 'endTime' in val;
  }

  isPerTime(val: any) {
    return 'timeUnit' in val && 'timeAmount' in val;
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
