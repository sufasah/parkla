import { NgModule } from '@angular/core';
import { ParkAreaComponent } from './park/park-area.component';
import { ReservationsComponent } from './reservations/reservations.component';
import { ParkAreasComponent } from './areas/park-areas.component';
import { LoadMoneyComponent } from './load-money/load-money.component';

@NgModule({
  declarations: [

    ParkAreasComponent,
      LoadMoneyComponent
  ],
  imports: [

  ],
  providers: [],
  exports: [],
})
export class UsersModule { }
