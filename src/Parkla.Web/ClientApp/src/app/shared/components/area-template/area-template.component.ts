import { AfterViewInit, Component, ElementRef, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { templatesUrl, templateNoImageUrl } from '@app/core/constants/http';
import { ParkArea } from '@app/core/models/park-area';
import { ParkSpace, SpacePath } from '@app/core/models/park-space';
import { Reservation } from '@app/core/models/reservation';
import { SignalrService } from '@app/core/services/signalr.service';
import { Point } from '@app/core/types/parkmap';
import { BaseType, select, Selection } from 'd3-selection';
import { zoom, ZoomBehavior, ZoomTransform } from 'd3-zoom';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-area-template',
  templateUrl: './area-template.component.html',
  styleUrls: ['./area-template.component.scss']
})
export class ParkTemplateComponent implements OnInit, AfterViewInit, OnDestroy {

  @ViewChild("canvasParent")
  bodyRef!: ElementRef<HTMLDivElement>;

  @ViewChild("canvas")
  canvasRef!:ElementRef<HTMLCanvasElement>;

  private _parkArea!: ParkArea;
  @Input()
  set parkArea(value: ParkArea) {
    this._parkArea = value;
    this.parkAreaChanges(value);
  }
  get parkArea() {
    return this._parkArea;
  }

  @Output()
  spaceClicked = new EventEmitter<ParkSpace>();

  @Output()
  reservationChanges = new EventEmitter<{reservation:Reservation, isDelete:boolean}>();

  canvas!:HTMLCanvasElement;

  ctx!:any;

  ParkImage: HTMLImageElement = new Image();

  private zoomBehavior: ZoomBehavior<any,any> = zoom();

  private selection!: Selection<BaseType, unknown, HTMLElement, any>;

  imageLoading = true;

  dialogVisible = false;

  parkAreaChangedAtLeastOnce = false;

  spaceChangesSubscription?: Subscription;
  reservationChangesSubscription?: Subscription;

  selectedSpace?: ParkSpace;

  constructor(
    private signalrService: SignalrService
  ) { }

  ngOnDestroy(): void {
    this.unregisterSpaceChanges();
  }

  ngOnInit(): void {
    this.zoomBehavior.on("zoom",(e) => {
      this.canvas.style.transform = `translate(${e.transform.x}px, ${e.transform.y}px) scale(${e.transform.k})`;
      this.canvas.style.transformOrigin = "0 0";
    });

    this.ParkImage.onload = () => {
      this.imageLoading = false;
      this.selection = select(".park-body");
      this.selection.call(this.zoomBehavior);
      this.initCanvas();
      this.drawCanvas();
    };

    this.ParkImage.onerror = () => {
      this.ParkImage.src = templateNoImageUrl;
    }

    this.signalrService.connectedEvent.subscribe((connected) => {
      if(connected === null) return;

      if(connected) {
        this.registerSpaceChanges();
        this.registerReservationChanges();
      }
      else {
        this.unregisterSpaceChanges();
        this.unregisterReservationChanges();
      }
    });
  }

  registerSpaceChanges() {
    this.spaceChangesSubscription = this.signalrService.registerParkSpaceChanges(this.parkArea.id,(space, isDelete) => {
      const index = this.parkArea.spaces.findIndex(x => x.id == space.id);
      const oldSpace = this.parkArea.spaces[index];
      if(index == -1) {
        this.parkArea.spaces.push(space);
        this.drawCanvas();
        return;
      }

      space.status = <any>space.status.toUpperCase();

      if(isDelete) {
        this.parkArea.spaces.splice(index, 1);
        if(this.selectedSpace && this.selectedSpace.id == space.id) this.spaceClicked.emit(undefined);
      }
      else if(oldSpace.statusUpdateTime == null || oldSpace.statusUpdateTime <= space.statusUpdateTime) {
        this.parkArea.spaces[index] = space;
        if(this.selectedSpace && this.selectedSpace.id == space.id) this.spaceClicked.emit(space);
      }

      this.drawCanvas();
    });
  }

  unregisterSpaceChanges() {
    this.spaceChangesSubscription?.unsubscribe();
  }

  registerReservationChanges() {
    this.reservationChangesSubscription = this.signalrService.registerReservationChanges(this.parkArea.id,(reservation, isDelete) => {
      reservation.startTime = new Date(reservation.startTime);
      reservation.endTime = new Date(reservation.endTime);

      if(isDelete) {
        let spaceIndex = this.parkArea.spaces.findIndex(x => x.id == reservation.spaceId);
        if (spaceIndex == -1) return;

        let reservationIndex = this.parkArea.spaces[spaceIndex].reservations.findIndex(x => x.id == reservation.id);
        if(reservationIndex == -1) return;

        let deletedReservation = this.parkArea.spaces[spaceIndex].reservations.splice(reservationIndex, 1)[0];
        deletedReservation.space = this.parkArea.spaces[spaceIndex];
        this.reservationChanges.emit({reservation: deletedReservation, isDelete: true});
      }
      else {
        let spaceIndex = this.parkArea.spaces.findIndex(x => x.id == reservation.spaceId);
        if (spaceIndex == -1) return;

        const reservations = this.parkArea.spaces[spaceIndex].reservations;
        let newIndex = 0;
        while(newIndex < reservations.length && reservations[newIndex].endTime <= reservation.startTime)
          newIndex++;

        reservations.splice(newIndex, 0, reservation);
        reservation.space = this.parkArea.spaces[spaceIndex];

        this.reservationChanges.emit({reservation: reservation, isDelete: false});
      }
    });
  }

  unregisterReservationChanges() {
    this.reservationChangesSubscription?.unsubscribe();
  }

  ngAfterViewInit(): void {
    this.canvas = this.canvasRef.nativeElement;
    this.ctx = this.canvas.getContext('2d')!;

    this.canvas.onclick = (e) => this.canvasOnClick(e);
  }

  initCanvas() {
    this.canvas.style.width = this.ParkImage.width+"px";
    this.canvas.style.height = this.ParkImage.height+"px";
    this.canvas.width = this.ParkImage.width;
    this.canvas.height = this.ParkImage.height;

    let pwStr = this.selection.style("width");
    let phStr = this.selection.style("height");
    let pWidth = Number(pwStr.substring(0,pwStr.length-2));
    let pHeight = Number(phStr.substring(0,phStr.length-2));
    let wRatio = pWidth / this.canvas.width;
    let hRatio = pHeight / this.canvas.height;
    let ratio = Math.min(wRatio,hRatio);
    let translateLeft = ratio == hRatio
      ? pWidth/2 - this.canvas.width*ratio/2
      : 0;
    let translateTop = ratio == wRatio
      ? pHeight/2 - this.canvas.height*ratio/2
      : 0;

    this.zoomBehavior.scaleExtent([ratio, 20])
      .translateExtent([
        [-translateLeft/ratio, -translateTop/ratio],
        [(pWidth-translateLeft)/ratio, (pHeight-translateTop)/ratio]
      ]);

    this.selection.call(
      this.zoomBehavior.transform,
      new ZoomTransform(
        ratio,
        translateLeft,
        translateTop
    ));
  }

  drawCanvas() {
    this.ctx.clearRect(0,0,this.canvas.width,this.canvas.height);

    this.ctx.drawImage(
      this.ParkImage, 0, 0,
      this.ParkImage.width,
      this.ParkImage.height
    );

    this.parkArea.spaces.forEach(space => {
      if(this.parkArea.reservationsEnabled && space.reservations){
        if(space.isReserved) {
          if(space.status.toUpperCase() == "EMPTY")
            this.drawEmptyReservedSpace(space.templatePath);
          else if(space.status.toUpperCase() == "OCCUPIED")
            this.drawOccupiedReservedSpace(space.templatePath);
          else
            this.drawUnknwonSpace(space.templatePath);
          return;
        }
      }

      if(space.status.toUpperCase() == "EMPTY")
        this.drawEmptySpace(space.templatePath);
      else if(space.status.toUpperCase() == "OCCUPIED")
        this.drawOccupiedSpace(space.templatePath);
      else
        this.drawUnknwonSpace(space.templatePath);
    });
  }

  drawEmptySpace(path: SpacePath){
    this.ctx.fillStyle = "rgba(0,255,0,0.25)";
    this.ctx.strokeStyle = "rgba(0,255,0,0.75)";
    this.drawSpace(path);
  }

  drawOccupiedSpace(path: SpacePath){
    this.ctx.fillStyle = "rgba(255,0,0,0.25)";
    this.ctx.strokeStyle = "rgba(255,0,0,0.75)";
    this.drawSpace(path);
  }

  drawEmptyReservedSpace(path: SpacePath){
    this.ctx.fillStyle = "rgba(255, 255, 102, 0.40)";
    this.ctx.strokeStyle = "rgba(255, 255, 102, 0.80)";
    this.drawSpace(path);
  }

  drawOccupiedReservedSpace(path: SpacePath){
    this.ctx.fillStyle = "rgba(255, 153, 51, 0.40)";
    this.ctx.strokeStyle = "rgba(255, 153, 51, 0.80)";
    this.drawSpace(path);
  }

  drawUnknwonSpace(path: SpacePath){
    this.ctx.fillStyle = "rgba(100,100,100,0.25)";
    this.ctx.strokeStyle = "rgba(100,100,100,0.75)";
    this.drawSpace(path);
  }

  drawSpace(path: SpacePath) {
    this.ctx.beginPath();
    this.ctx.moveTo(path[0][0],path[0][1]);
    this.ctx.lineTo(path[1][0],path[1][1]);
    this.ctx.lineTo(path[2][0],path[2][1]);
    this.ctx.lineTo(path[3][0],path[3][1]);
    this.ctx.closePath();
    this.ctx.fill();
    this.ctx.stroke();
  }

  isPointInSpace(polygon: SpacePath, p: Point): boolean {
    let x = p[0], y = p[1];
    let inside = false;
    let len = polygon.length
    for (let i = 0, j = len - 1; i < len; j = i++) {
      let xi = polygon[i][0], yi = polygon[i][1];
      let xj = polygon[j][0], yj = polygon[j][1];
      let intersect = ((yi > y) !== (yj > y))
          && (x < (xj - xi) * (y - yi) / (yj - yi) + xi);
      if (intersect) inside = !inside;
    }
    return inside;
  }

  canvasOnClick(e:any) {
    this.selectedSpace = this.parkArea.spaces.find(space => this.isPointInSpace(
      space.templatePath,
      [e.offsetX, e.offsetY]
    ));

    if(!this.selectedSpace) return;

    this.spaceClicked.emit(this.selectedSpace);
  }

  parkAreaChanges(value: ParkArea) {
    if(!this.parkAreaChangedAtLeastOnce) {
      this.parkAreaChangedAtLeastOnce = true;
      return;
    }

    this.imageLoading = true;

    if(!value.templateImage) {
      this.ParkImage.src = templateNoImageUrl;
      return;
    }

    if(!value.templateImage.startsWith("data:"))
      this.ParkImage.src = `${templatesUrl}/${value.templateImage}`;
    else
      this.ParkImage.src = value.templateImage;
  }

}
