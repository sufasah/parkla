import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ParkSpace, SpacePath } from '@app/core/models/parking-lot';
import { select } from 'd3-selection';
import { zoom, zoomIdentity, ZoomTransform } from "d3-zoom";

declare var $:any;

  @Component({
  selector: 'app-park',
  templateUrl: './park.component.html',
  styleUrls: ['./park.component.scss']
})
export class ParkComponent implements OnInit, AfterViewInit {


  areas = [

  ]

  areaSuggestions = []

  selectedArea = null;

  timeRange:[Date?,Date?] = [];

  minDate = new Date();
  maxDate = new Date(Date.now()+1000*60*60*24*3);

  @ViewChild("parkCanvas")
  canvasRef!:ElementRef<HTMLCanvasElement>;

  @ViewChild("parkHeader")
  headerRef!:ElementRef<HTMLDivElement>;

  @ViewChild("parkBody")
  bodyRef!: ElementRef<HTMLDivElement>;

  canvas!:HTMLCanvasElement;

  ctx!:any;

  parkingLotImage: HTMLImageElement = new Image();

  parkSpaces: ParkSpace[] = [
    {
      id:"178",
      status: "free",
      templatePath: [
        [165,21],
        [165,83],
        [197,83],
        [197,21]
      ],
      reservations: [{
        startTime: new Date(),
        endTime: new Date(Date.now()+1000*60*60)
      }]
    },
    {
      id:"177",
      status: "occupied",
      templatePath: [
        [133,21],
        [133,83],
        [165,83],
        [165,21]
      ],
      reservations: [{
        startTime: new Date(),
        endTime: new Date(Date.now()+1000*60*60)
      }]
    },
    {
      id:"181",
      status: "free",
      templatePath: [
        [261,21],
        [261,84],
        [293,83],
        [294,21]
      ],
      reservations: [{
        startTime: new Date(),
        endTime: new Date(Date.now()+1000*60*60)
      }]
    },
    {
      id:"184",
      status: "occupied",
      templatePath: [
        [358,22],
        [358,84],
        [391,84],
        [391,21]
      ],
      reservations: [{
        startTime: new Date(),
        endTime: new Date(Date.now()+1000*60*60)
      }]
    },
    {
      id:"183",
      status: "free",
      templatePath: [
        [326,21],
        [326,82],
        [358,83],
        [358,22]
      ],
      reservations: [{
        startTime: new Date(),
        endTime: new Date(Date.now()+1000*60*60)
      }]
    }
  ]

  constructor() { }

  ngOnInit(): void {
    this.areaSuggestions = <any>[
      "a",
      "b",
      "c"
    ];
  }

  ngAfterViewInit(): void {

    this.canvas = this.canvasRef.nativeElement;
    this.ctx = this.canvas.getContext('2d')!;

    const zom:any = zoom()
      .on("zoom",(e) => {
        let transform = e.transform;
        this.canvas.style.transform = "translate(" + transform.x + "px," + transform.y + "px) scale(" + transform.k + ")";
        this.canvas.style.transformOrigin = "0 0";
      });

    let selection = select(".park-body").call(zom);

    this.parkingLotImage.src = "https://www.realserve.com.au/wp-content/uploads/CarParkingPlans/CAR-PARKING-PLAN-SERVICE-BY.jpg";

    this.parkingLotImage.onload = () => {
      this.canvas.style.width = this.parkingLotImage.width+"px";
      this.canvas.style.height = this.parkingLotImage.height+"px";
      this.canvas.width = this.parkingLotImage.width;
      this.canvas.height = this.parkingLotImage.height;

      let pwStr = selection.style("width");
      let phStr = selection.style("height");
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

      zom.scaleExtent([ratio, 20])
        .translateExtent([
          [-translateLeft/ratio, -translateTop/ratio],
          [(pWidth-translateLeft)/ratio, (pHeight-translateTop)/ratio]
        ]);

      selection.call(
        zom.transform,
        new ZoomTransform(
          ratio,
          translateLeft,
          translateTop
      ));

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

  searchArea(e:any) {
    console.log(e.query );
    if(!e.query) this.areaSuggestions = <any>["a","b","c"];
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

  drawFreeSpace(path:  [[number,number],[number,number],[number,number],[number,number]]){
    this.ctx.fillStyle = "rgba(0,255,0,0.25)";
    this.ctx.strokeStyle = "rgba(0,255,0,0.75)";
    this.ctx.beginPath();
    this.ctx.moveTo(path[0][0],path[0][1]);
    this.ctx.lineTo(path[1][0],path[1][1]);
    this.ctx.lineTo(path[2][0],path[2][1]);
    this.ctx.lineTo(path[3][0],path[3][1]);
    this.ctx.closePath();
    this.ctx.fill();
    this.ctx.stroke();
  }

  drawOccupiedSpace(path:  [[number,number],[number,number],[number,number],[number,number]]){
    this.ctx.fillStyle = "rgba(255,0,0,0.25)";
    this.ctx.strokeStyle = "rgba(255,0,0,0.75)";
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

type Point = [number,number];
