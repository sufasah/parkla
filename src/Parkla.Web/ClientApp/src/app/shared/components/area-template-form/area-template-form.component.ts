import { HttpErrorResponse } from '@angular/common/http';
import { AfterViewInit, Component, EventEmitter, Input, OnInit, Output, TemplateRef, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ParkArea } from '@app/core/models/park-area';
import { ParkSpace, SpacePath } from '@app/core/models/park-space';
import { ParkSpaceReal } from '@app/core/models/park-space-real';
import { Pricing } from '@app/core/models/pricing';
import { ParkAreaService } from '@app/core/services/park-area.service';
import { RealParkSpaceService } from '@app/core/services/real-park-space.service';
import { LazyLoadEvent } from 'primeng/api';
import { Table } from 'primeng/table';
import { EditAreaTemplateComponent } from '../edit-area-template/edit-area-template.component';

interface ParklaDialogModel {
  rightButtonLabel: string;
  title: string;
  text1: string;
  text2: string;
  confirm: () => void;
  cancel: () => void;
  data: any;
}

@Component({
  selector: 'app-area-template-form',
  templateUrl: './area-template-form.component.html',
  styleUrls: ['./area-template-form.component.scss']
})
export class AreaTemplateFormComponent implements OnInit, AfterViewInit {

  @Input()
  area!: ParkArea;

  @Output()
  areaChange = new EventEmitter<ParkArea>();

  @Output()
  formSubmit = new EventEmitter<NgForm>();

  @Input()
  loading = false;

  @ViewChild(EditAreaTemplateComponent)
  editAreaTemplateRef!: EditAreaTemplateComponent

  @Output()
  onRealSpaceFetchError = new EventEmitter<HttpErrorResponse>();

  @Output()
  onRealSpaceAddError = new EventEmitter<HttpErrorResponse>();

  @Output()
  onRealSpaceDeleteError = new EventEmitter<HttpErrorResponse>();

  @Output()
  onAddRealSpace = new EventEmitter<ParkSpaceReal>();

  editing = false;
  spaceModalVisible = false;
  imageLoading = true;
  spaceAdding = false;
  editingSpace: ParkSpace = <any>{};

  realSpaces: ParkSpaceReal[] = [];

  newRealSpaceName = "";

  totalRecords = 0;

  @Input()
  realSpacesPageSize = 10;

  nextRecord = 0;

  searchTOID?: NodeJS.Timeout;
  lastSearchSeconds = -10;
  lastSearchInput: string | null = null;
  realSpacesLoading = false;

  pricingSuggestions: Pricing[] = [];
  pricingEmptyMessage = "";

  deleteDialogVisible = false;
  parklaDialog?: ParklaDialogModel;

  constructor(
    private realSpaceService: RealParkSpaceService,
    private areaService: ParkAreaService
  ) { }

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
  }

  searchPricing(evt: any) {
    evt.query = evt.query.toLowerCase();
    this.areaService.getAreaPricings(this.area.id).subscribe({
      next: pricings => {
        if(!!evt.query){
          this.pricingSuggestions = pricings.filter(x =>
            x.type.toLowerCase().includes(evt.query) ||
            x.amount.toString().toLowerCase().includes(evt.query) ||
            x.unit.toLowerCase().includes(evt.query)
          );
        }
        else
          this.pricingSuggestions = pricings;

        //this.pricingSuggestions.unshift(<any>null);
        this.pricingEmptyMessage = "No Pricing Found"
      },
      error: (err: HttpErrorResponse) => {
        //this.pricingSuggestions = [null!];
        this.pricingEmptyMessage = err.error.message;
      }
    });
  }

  spaceModalDone() {
    this.spaceModalVisible = false;
    this.editAreaTemplateRef.drawCanvas();
  }

  submit(form: NgForm) {
    this.formSubmit.emit(form);
  }

  addTemplateSpace() {
    this.spaceAdding = !this.spaceAdding;
  }

  addTemplateSpaceDone(spacePath: SpacePath) {
    this.area.spaces.push(<any>{
      areaId: this.area.id,
      templatePath: [...spacePath],
    });
  }

  setTemplateImageButton(event: Event) {
    this.spaceAdding = false;

    const btn = <HTMLButtonElement>event.target;
    const input = <HTMLInputElement>btn.nextElementSibling;

    input.click()
  }

  setTemplateImage(event: any) {
    const input = <HTMLInputElement>event.target;
    if (input.files?.length == 0) return;

    const fr = new FileReader();
    fr.readAsDataURL(input.files![0]);
    fr.onload = (event) => {
      this.area.templateImage = <string>fr.result;
      this.editAreaTemplateRef.parkAreaChanges(this.area);
    };
  }

  clearTemplateSpaces() {
    this.spaceAdding = false;
    this.deleteDialogVisible = true;
    this.parklaDialog = {
      title: "Clear Park Spaces",
      text1: "Are you sure to clear all spaces of the park area?",
      text2: "",
      data: undefined,
      rightButtonLabel: "Clear",
      confirm: this.clearTemplateSpacesConfirm.bind(this),
      cancel: this.cancelDeleteDialog.bind(this)
    };
  }

  clearTemplateSpacesConfirm() {
    this.area.spaces = [];
    this.editAreaTemplateRef.drawCanvas();
    this.deleteDialogVisible = false;
  }

  spaceClicked(space: ParkSpace) {
    this.spaceModalVisible = true;
    this.spaceAdding = false;
    this.editingSpace = space;
  }

  spaceRightClicked(space: ParkSpace) {
    this.deleteDialogVisible = true;
    this.parklaDialog = {
      title: "Park Space Deletion",
      text1: space.id.toString(),
      text2: space.name,
      data: space,
      rightButtonLabel: "Delete",
      confirm: this.deleteSpaceConfirm.bind(this),
      cancel: this.cancelDeleteDialog.bind(this)
    };
  }

  deleteSpaceConfirm() {
    const space: ParkSpace = this.parklaDialog?.data;
    const index = this.area.spaces.indexOf(space);
    if(index != -1) {
      this.area.spaces.splice(index,1);
      this.editAreaTemplateRef.drawCanvas();
    }
    this.spaceModalVisible = false;
    this.deleteDialogVisible = false;
  }

  clearTable(table: Table, searchInput: HTMLInputElement) {
    //table.clear();
    searchInput.value = "";
    this.fetchRealSpaces(this.nextRecord);
  }

  loadRealSpaces(evt: LazyLoadEvent) {
    if(this.realSpacesLoading) return;

    const nextRecord = evt.first!;

    this.nextRecord = nextRecord;

    this.fetchRealSpaces(
      nextRecord,
      this.lastSearchInput
    );
}

  fetchRealSpaces(nextRecord: number, search: string | null = null) {
    this.realSpaceService.getPage(this.area.parkId, nextRecord, this.realSpacesPageSize, search).subscribe({
      next: response => {
        if(response.headers.has("x-total-records")) {
          this.totalRecords = Number(response.headers.get("x-total-records"));
          this.nextRecord = nextRecord;
        }

        this.realSpaces = response.body!;
        this.realSpacesLoading = false;
      },
      error: (err:HttpErrorResponse) => {
        this.realSpacesLoading = false;
        this.onRealSpaceFetchError.emit(err);
      }
    });
  }

  onSearchInput(evt: any) {
    const seconds = Number.parseInt((evt.timeStamp/1000).toFixed(0));
    const data = evt.target.value;

    if(seconds - this.lastSearchSeconds < 2)
      clearTimeout(this.searchTOID!);

    this.searchTOID = setTimeout(() => {
      this.fetchRealSpaces(
        0,
        data,
      );
    }, 1000);

    this.lastSearchSeconds = seconds;
    this.lastSearchInput = data;
  }

  addRealSpace() {
    this.realSpaceService.addRealSpace(<any>{
      parkId: this.area.parkId,
      name: this.newRealSpaceName
    }).subscribe({
      next: realSpace => {
        this.realSpaces.push(realSpace);
      },
      error: (err:HttpErrorResponse) => {
        this.onRealSpaceAddError.emit(err);
      }
    });
  }

  deleteRealSpace(realSpace: ParkSpaceReal) {
    this.deleteDialogVisible = true;
    this.parklaDialog = {
      title: "Real Space Deletion",
      text1: realSpace.id.toString(),
      text2: realSpace.name,
      data: realSpace,
      rightButtonLabel: "Delete",
      confirm: this.deleteRealSpaceConfirm.bind(this),
      cancel: this.cancelDeleteDialog.bind(this)
    };
  }

  deleteRealSpaceConfirm() {
    const realSpace: ParkSpaceReal = this.parklaDialog!.data;
    this.deleteDialogVisible = false;
    this.realSpaceService.deleteRealSpace(realSpace.id).subscribe({
      next: () => {
        this.realSpacesLoading = true;
        this.area.spaces.forEach(space => {
          if(space.realSpace && space.realSpace.id == realSpace.id)
            space.realSpace = undefined;
        });
        this.fetchRealSpaces(this.nextRecord, this.lastSearchInput);
      },
      error: (err:HttpErrorResponse) => {
        this.onRealSpaceDeleteError.emit(err);
      }
    });
  }

  cancelDeleteDialog() {
    this.deleteDialogVisible = false;
  }

  setSelectedRealSpace(realSpace: ParkSpaceReal) {
    if(realSpace) {
      let collision = false;
      let isRealSpaceBindedLocalSpace = false;
      //collision locally between editing spaces or
      //collision with db spaces
      for(let i=0; i<this.area.spaces.length; i++) {
        const space = this.area.spaces[i];
        if(space.realSpace && space.realSpace.id == realSpace.id && space != this.editingSpace) {
          collision = true; //local collision
          break;
        }
        else if(realSpace.spaceId == space.id ) {
          isRealSpaceBindedLocalSpace = true; // realSpace not binded local space
        }
      }
      if(realSpace.spaceId && !isRealSpaceBindedLocalSpace) {
        collision = true; // global collision
      }

      if(collision) return;
    }

    this.editingSpace.realSpace = realSpace;
  }

  getSelectedRealSpace() {
    return this.editingSpace.realSpace;
  }

  isRealSpaceSelected(space: ParkSpaceReal) {
    return !!this.area.spaces.find(x => x.realSpace && x.realSpace.id == space.id);
  }
}
