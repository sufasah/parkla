import { Component, OnInit } from '@angular/core';
import { PrimeNGConfig } from 'primeng/api';
import { ParkService } from './core/services/park.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {

  constructor(
    private primengConfig:PrimeNGConfig,
    private parkService: ParkService
  ) {
  }

  ngOnInit(): void {
    this.primengConfig.ripple = true;
    this.parkService.LoadParks();
  }

}
