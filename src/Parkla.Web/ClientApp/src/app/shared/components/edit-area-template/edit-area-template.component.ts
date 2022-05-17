import { AfterViewInit, Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { templatesUrl, tmeplateNoImageUrl } from '@app/core/constants/http';
import { ParkArea } from '@app/core/models/park-area';
import { ParkSpace, SpacePath } from '@app/core/models/park-space';
import { Point } from '@app/core/types/parkmap';
import { BaseType, select, Selection } from 'd3-selection';
import { zoom, ZoomBehavior, ZoomTransform } from 'd3-zoom';

@Component({
  selector: 'app-edit-area-template',
  templateUrl: './edit-area-template.component.html',
  styleUrls: ['./edit-area-template.component.scss']
})
export class EditAreaTemplateComponent implements OnInit, AfterViewInit {

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

  private _selectingSpacePath = false;
  @Input("selectingSpacePath")
  set selectingSpacePath(value: boolean) {
    this._selectingSpacePath = value;
    if(!value) {
      let tmp = this.spacePathPoint;
      this.spacePathPoint = 0;

      if(tmp > 0)
        this.drawCanvas();
    }
  }
  get selectingSpacePath() {
    return this._selectingSpacePath;
  }

  @Output("selectingSpacePathChange")
  selectingSpacePathChange = new EventEmitter<boolean>();

  @Output()
  spaceClicked = new EventEmitter<ParkSpace>();

  @Output()
  spaceRightClicked = new EventEmitter<ParkSpace>();

  @Output()
  spacePathSelected = new EventEmitter<SpacePath>();

  canvas!:HTMLCanvasElement;

  ctx!:any;

  ParkImage: HTMLImageElement = new Image();

  private zoomBehavior: ZoomBehavior<any,any> = zoom();

  private selection!: Selection<BaseType, unknown, HTMLElement, any>;

  imageLoading = true;

  dialogVisible = false;


  parkAreaChangedAtLeastOnce = false;

  spacePathPoint = 0;
  spacePath: SpacePath = [[0,0],[0,0],[0,0],[0,0]];

  constructor() {
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
      this.ParkImage.src = tmeplateNoImageUrl;
    }
  }

  ngAfterViewInit(): void {
    this.canvas = this.canvasRef.nativeElement;
    this.canvas.onclick = (e) => this.canvasOnClick(<PointerEvent>e);
    this.canvas.oncontextmenu = (e) => this.canvasOnRightClick(<PointerEvent>e);
    this.ctx = this.canvas.getContext('2d')!;
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
      if(space.name && space.realSpace && space.name.length > 0 && space.name.length <= 30)
        this.drawValidSpace(space.templatePath);
      else
        this.drawInvalidSpace(space.templatePath);
    });

    this.drawSpacePath();
  }

  drawSpacePath() {
    this.ctx.fillStyle = "rgba(152,192,202,1)";
    this.ctx.strokeStyle = "rgba(102,142,152,1)";
    this.ctx.beginPath();

    for(let i=0; i < this.spacePathPoint; i++) {
      let point = this.spacePath[i];
      if(i!=0) {
        this.ctx.lineTo(point[0], point[1]);
      }
      this.ctx.arc(point[0], point[1], 1, 2 * Math.PI, false);
    }
    this.ctx.stroke();
  }

  drawValidSpace(path: SpacePath){
    this.ctx.fillStyle = "rgba(0,0,0,0.25)";
    this.ctx.strokeStyle = "rgba(0,0,0,0.75)";
    this.drawSpace(path);
  }

  drawInvalidSpace(path: SpacePath){
    this.ctx.fillStyle = "rgba(0,0,255,0.25)";
    this.ctx.strokeStyle = "rgba(0,0,255,0.75)";
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
    let len = polygon.length;
    for (let i = 0, j = len - 1; i < len; j = i++) {
      let xi = polygon[i][0], yi = polygon[i][1];
      let xj = polygon[j][0], yj = polygon[j][1];
      let intersect = ((yi > y) !== (yj > y))
          && (x < (xj - xi) * (y - yi) / (yj - yi) + xi);
      if (intersect) inside = !inside;
    }
    return inside;
  }

  canvasOnClick(e: PointerEvent) {
    if(this.selectingSpacePath) {

      this.spacePath[this.spacePathPoint] = [e.offsetX, e.offsetY];
      this.spacePathPoint++;

      if(this.spacePathPoint == 4) {
        this.spacePathSelected.emit(this.spacePath);
        this.selectingSpacePath = false;
        this.selectingSpacePathChange.emit(false);
      }
      this.drawCanvas();
    }
    else {
      let selectedSpace = this.parkArea.spaces.find(space => this.isPointInSpace(
        space.templatePath,
        [e.offsetX, e.offsetY]
      ));

      if(selectedSpace)
        this.spaceClicked.emit(selectedSpace);
    }
  }

  canvasOnRightClick(e: PointerEvent) {
    e.preventDefault();

    let selectedSpace = this.parkArea.spaces.find(space => this.isPointInSpace(
      space.templatePath,
      [e.offsetX, e.offsetY]
    ));

    if(selectedSpace)
      this.spaceRightClicked.emit(selectedSpace);
  }

  parkAreaChanges(value: ParkArea) {
    if(!this.parkAreaChangedAtLeastOnce) {
      this.parkAreaChangedAtLeastOnce = true;
      return;
    }

    this.imageLoading = true;

    if(!value.templateImage) {
      this.ParkImage.src = tmeplateNoImageUrl;
      return;
    }

    if(!value.templateImage.startsWith("data:"))
      this.ParkImage.src = `${templatesUrl}/${value.templateImage}`;
    else
      this.ParkImage.src = value.templateImage;
  }
}
