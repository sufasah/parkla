import { AfterViewInit, Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ParkArea } from '@app/core/models/park-area';
import { ParkSpace, SpacePath } from '@app/core/models/park-space';
import { ParkSpaceReal } from '@app/core/models/park-space-real';
import { ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';
import { EditAreaTemplateComponent } from '../edit-area-template/edit-area-template.component';

@Component({
  selector: 'app-park-area-form',
  templateUrl: './park-area-form.component.html',
  styleUrls: ['./park-area-form.component.scss']
})
export class ParkAreaFormComponent implements OnInit, AfterViewInit {

  @Input()
  area!: ParkArea;

  @Output()
  areaChange = new EventEmitter<ParkArea>();

  @Output()
  formSubmit = new EventEmitter<NgForm>();

  @Input()
  loading = false;

  @Input()
  submitLabel = "Submit";

  @ViewChild("templateImageRef")
  templateImage!: ElementRef<HTMLImageElement>

  @ViewChild(EditAreaTemplateComponent)
  editAreaTemplateRef!: EditAreaTemplateComponent

  templateModalVisible = false;
  spaceModalVisible = false;
  timeUnitOptions = ["minutes", "hours", "days", "months"]
  imageLoading = true;
  spaceAdding = false;
  editingSpace: ParkSpace = <any>{};

  _selectedRealSpace?: ParkSpaceReal;
  set selectedRealSpace(value: ParkSpaceReal | undefined) {
    this._selectedRealSpace = value;
    this.editingSpace.realSpace = value ? {...value} : undefined;
  }
  get selectedRealSpace() {
    return this._selectedRealSpace;
  }

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

  constructor(
    private confirmationService: ConfirmationService
  ) { }

  ngOnInit(): void {
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

  submit(ngForm: NgForm) {
    this.formSubmit.emit(ngForm);
  }

  addPricing() {
    if(!this.area.pricings)
      this.area.pricings = [];

    this.area.pricings!.push(<any>{
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

  isTimeInterval(val: any) {
    return 'beginningTime' in val && 'endTime' in val;
  }

  isPerTime(val: any) {
    return 'timeUnit' in val && 'timeAmount' in val;
  }

}
