import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
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

  timeRange = new Date();

  minDate = new Date();
  maxDate = new Date(Date.now()+1000*60*60*24*3);

  @ViewChild("parkCanvas")
  canvasRef!:ElementRef<HTMLCanvasElement>;

  @ViewChild("header")
  headerRef!:ElementRef<HTMLDivElement>;

  canvas!:HTMLCanvasElement;

  ctx!:any;

  parkingLotImage: HTMLImageElement = new Image();


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
        console.log(e);

        this.canvas.style.transform = "translate(" + transform.x + "px," + transform.y + "px) scale(" + transform.k + ")";
        this.canvas.style.transformOrigin = "0 0";
      });

    var selection = select("#body").call(zom);

    /*this.canvas.style.height = window.screen.availHeight -
      this.headerRef.nativeElement.scrollTop -
      this.headerRef.nativeElement.scrollHeight + "px";
    this.canvas.width = this.canvas.offsetWidth;
    this.canvas.height = this.canvas.offsetHeight;*/

    this.canvas.parentElement!.style.height = window.screen.availHeight -
      this.headerRef.nativeElement.scrollTop -
      this.headerRef.nativeElement.scrollHeight + "px";

    this.parkingLotImage.src = "https://www.realserve.com.au/wp-content/uploads/CarParkingPlans/CAR-PARKING-PLAN-SERVICE-BY.jpg";


    this.parkingLotImage.onload = () => {
      this.parkingLotImage.onload = null;

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

        console.log(translateTop);

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
  }

}
