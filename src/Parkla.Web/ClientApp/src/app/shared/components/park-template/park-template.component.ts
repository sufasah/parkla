import { AfterViewInit, Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { ParkSpace, Point, SpacePath } from '@app/core/models/parking-lot';
import { BaseType, select, Selection } from 'd3-selection';
import { zoom, ZoomBehavior, ZoomTransform } from 'd3-zoom';

@Component({
  selector: 'app-park-template',
  templateUrl: './park-template.component.html',
  styleUrls: ['./park-template.component.scss']
})
export class ParkTemplateComponent implements OnInit, AfterViewInit {

  @ViewChild("parkBody")
  bodyRef!: ElementRef<HTMLDivElement>;

  @ViewChild("parkCanvas")
  canvasRef!:ElementRef<HTMLCanvasElement>;

  canvas!:HTMLCanvasElement;

  ctx!:any;

  parkingLotImage: HTMLImageElement = new Image();

  @Input()
  parkSpaces: ParkSpace[] = []

  private zoomBehavior: ZoomBehavior<any,any> = zoom();
  private selection!: Selection<BaseType, unknown, HTMLElement, any>;

  constructor() { }

  ngOnInit(): void {
    this.zoomBehavior.on("zoom",(e) => {
      this.canvas.style.transform = `translate(${e.transform.x}px, ${e.transform.y}px) scale(${e.transform.k})`;
      this.canvas.style.transformOrigin = "0 0";
    });
  }

  ngAfterViewInit(): void {

    this.canvas = this.canvasRef.nativeElement;
    this.ctx = this.canvas.getContext('2d')!;

    this.selection = select(".park-body");
    this.selection.call(this.zoomBehavior);

    this.parkingLotImage.src = "https://www.realserve.com.au/wp-content/uploads/CarParkingPlans/CAR-PARKING-PLAN-SERVICE-BY.jpg";

    this.parkingLotImage.onload = () => {
      this.initCanvas();

      this.canvas.onclick = (e) => {
        let selectedSpace = this.parkSpaces.find(space => this.isPointInSpace(
          space.templatePath,
          [e.offsetX, e.offsetY]
        ));

        if(selectedSpace){
          console.log("space id: "+selectedSpace.id);
        }
      }

      this.drawCanvas();
    };
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

      this.parkSpaces.forEach(space => {
      if(space.status == "free")
      this.drawFreeSpace(space.templatePath);
      else
      this.drawOccupiedSpace(space.templatePath);
    });
  }

  drawFreeSpace(path: SpacePath){
    this.ctx.fillStyle = "rgba(0,255,0,0.25)";
    this.ctx.strokeStyle = "rgba(0,255,0,0.75)";
    this.drawSpace(path);
  }

  drawOccupiedSpace(path: SpacePath){
    this.ctx.fillStyle = "rgba(255,0,0,0.25)";
    this.ctx.strokeStyle = "rgba(255,0,0,0.75)";
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

}
