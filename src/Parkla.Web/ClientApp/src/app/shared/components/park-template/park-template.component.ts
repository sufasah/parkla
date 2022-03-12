import { AfterViewInit, Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ParkArea, ParkingLot, ParkSpace, Point, SpacePath } from '@app/core/models/parking-lot';
import { ParkTemplateClickEvent } from '@app/core/types/types';
import { BaseType, select, Selection } from 'd3-selection';
import { zoom, ZoomBehavior, ZoomTransform } from 'd3-zoom';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-park-template',
  templateUrl: './park-template.component.html',
  styleUrls: ['./park-template.component.scss']
})
export class ParkTemplateComponent implements OnInit, AfterViewInit {

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

  private _selectedTimeRange: [Date?, Date?] = [];

  @Input()
  set selectedTimeRange(value) {
    value[0]?.setSeconds(0);
    value[1]?.setSeconds(0);
    this._selectedTimeRange = value;
    this.selectedTimeRangeChanges(value);
  }

  @Output()
  spaceClicked = new EventEmitter<ParkTemplateClickEvent>();

  selectedSpace!: ParkSpace;

  //@Input()
  //editMode: boolean = false;

  get selectedTimeRange() {
    return this._selectedTimeRange;
  }


  canvas!:HTMLCanvasElement;

  ctx!:any;

  parkingLotImage: HTMLImageElement = new Image();

  private zoomBehavior: ZoomBehavior<any,any> = zoom();

  private selection!: Selection<BaseType, unknown, HTMLElement, any>;

  private viewInitialized = false;

  imageLoading = true;

  dialogVisible = false;

  constructor(private messageService: MessageService) { }

  ngOnInit(): void {
    this.zoomBehavior.on("zoom",(e) => {
      this.canvas.style.transform = `translate(${e.transform.x}px, ${e.transform.y}px) scale(${e.transform.k})`;
      this.canvas.style.transformOrigin = "0 0";
    });

    this.parkingLotImage.onload = () => {
      this.imageLoading = false;
      this.initCanvas();
      this.canvas.onclick = (e) => this.canvasOnClick(e);
      this.drawCanvas();
    };
  }

  ngAfterViewInit(): void {
    this.viewInitialized = true;

    this.canvas = this.canvasRef.nativeElement;
    this.ctx = this.canvas.getContext('2d')!;

    this.selection = select(".park-body");
    this.selection.call(this.zoomBehavior);

  }

  initCanvas() {
    this.canvas.style.width = this.parkingLotImage.width+"px";
    this.canvas.style.height = this.parkingLotImage.height+"px";
    this.canvas.width = this.parkingLotImage.width;
    this.canvas.height = this.parkingLotImage.height;

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
      this.parkingLotImage, 0, 0,
      this.parkingLotImage.width,
      this.parkingLotImage.height);

    this.parkArea.spaces.forEach(space => {
      if(this.parkArea.reservationsEnabled){
        for(let i = 0; i < space.reservations!.length; i++){
          let reservation = space.reservations![i];

          let isSpaceReserved =
            (reservation.startTime < this.selectedTimeRange[1]! &&
            reservation.startTime >= this.selectedTimeRange[0]!) ||
            (reservation.endTime > this.selectedTimeRange[0]! &&
            reservation.endTime <= this.selectedTimeRange[1]!)

          if(isSpaceReserved || (space.status == "occupied" && !this.parkArea.notReservedOccupiable)) {
            (<any>space).isDrawnReserved = true;

            if(space.status == "empty")
              this.drawEmptyReservedSpace(space.templatePath);
            else
              this.drawOccupiedReservedSpace(space.templatePath);

            return;
          }
          else {
            (<any>space).isDrawnReserved = false;
          }
        }
      }

      if(space.status == "empty")
        this.drawEmptySpace(space.templatePath);
      else
        this.drawOccupiedSpace(space.templatePath);
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
    let selectedSpace = this.parkArea.spaces.find(space => this.isPointInSpace(
      space.templatePath,
      [e.offsetX, e.offsetY]
    ));

    if(!selectedSpace) return;

    if(this.parkArea.reservationsEnabled) {
      if(selectedSpace.status == "empty") {
        this.selectedSpace = selectedSpace;
        this.showReserveModal();
        //show time interval selected
        //show pricig table
        //show reservation intervals and available intervals between them
        //if reserved show reserved user's username
        //if user wallet is not enough it must not be possible to confirm
      }
      else {
        if((<any>selectedSpace).isDrawnReserved) {
        this.selectedSpace = selectedSpace;
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

  showReserveModal() {
    this.dialogVisible = true;

  }

  selectedTimeRangeChanges(value: [Date?, Date?]) {
    if(this.viewInitialized && value[0] && value[1]) {
      this.drawCanvas();
    }
  }

  parkAreaChanges(value: ParkArea) {
    this.imageLoading = true;
    this.parkingLotImage.src = value.templateImg;
  }

  reserveSpace() {
    console.log("reserve space");
  }

}
