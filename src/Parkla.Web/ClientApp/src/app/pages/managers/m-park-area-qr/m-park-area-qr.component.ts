import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-m-park-area-qr',
  templateUrl: './m-park-area-qr.component.html',
  styleUrls: ['./m-park-area-qr.component.scss']
})
export class MParkAreaQRComponent implements OnInit {

  url="";

  constructor(private router: Router) { }

  ngOnInit(): void {
    this.url = "http://localhost:4200"+this.router.url.substring(0,this.router.url.length-3);
  }

}
