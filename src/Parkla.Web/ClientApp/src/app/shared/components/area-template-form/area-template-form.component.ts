import { HttpErrorResponse } from '@angular/common/http';
import { AfterViewInit, Component, EventEmitter, Input, OnInit, Output, TemplateRef, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ParkArea } from '@app/core/models/park-area';
import { ParkSpace, SpacePath } from '@app/core/models/park-space';
import { ParkSpaceReal } from '@app/core/models/park-space-real';
import { RealParkSpaceService } from '@app/core/services/real-park-space.service';
import { ConfirmationService, LazyLoadEvent } from 'primeng/api';
import { Table } from 'primeng/table';
import { EditAreaTemplateComponent } from '../edit-area-template/edit-area-template.component';

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

  constructor(
    private confirmationService: ConfirmationService,
    private realSpaceService: RealParkSpaceService
  ) { }

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
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

  clearTemplateSpace() {
    this.spaceAdding = false;
    this.confirmationService.confirm({
      header: "Clear",
      message: "Are you sure to clear all spaces on park area?",
      accept: () => {
        this.area.spaces = [];
        this.editAreaTemplateRef.drawCanvas();
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
        if(response.headers.has("x-total-records"))
          this.totalRecords = Number(response.headers.get("x-total-records"));

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
        this.nextRecord,
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
    this.confirmationService.confirm({
      header: "Real Space Deletion",
      message: `Are you sure to delete the realspace with '${realSpace.id}' id and '${realSpace.name}' name ?`,
      accept: () => {
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
      },
      icon: "pi pi-trash"
    })
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
