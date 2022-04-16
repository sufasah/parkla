import { NgModule } from '@angular/core';
import { ParkMapComponent } from './park-map/park-map.component';
import { MParkMapComponent } from './m-park-map/m-park-map.component';
import { MParkAreaComponent } from './m-park-area/m-park-area.component';
import { MParkAreasComponent } from './m-park-areas/m-park-areas.component';
import { MProfileComponent } from './m-profile/m-profile.component';
import { MNewParkComponent } from './m-new-park/m-new-park.component';
import { MNewParkAreaComponent } from './m-new-park-area/m-new-park-area.component';
import { MParkDashboardComponent } from './m-park-dashboard/m-park-dashboard.component';
import { MEditParkComponent } from './m-edit-park/m-edit-park.component';
import { MEditParkAreaComponent } from './m-edit-park-area/m-edit-park-area.component';
import { MEditTemplateComponent } from './m-edit-template/m-edit-template.component';
import { MDashboardComponent } from './m-dashboard/m-dashboard.component';
import { MParkAreaQRComponent } from './m-park-area-qr/m-park-area-qr.component';
import { FileUploadDirective } from './m-new-park-area/file-upload.directive';

@NgModule({
  declarations: [

  
    ParkMapComponent,
         MParkMapComponent,
         MParkAreaComponent,
         MParkAreasComponent,
         MProfileComponent,
         MNewParkComponent,
         MNewParkAreaComponent,
         MParkDashboardComponent,
         MEditParkComponent,
         MEditParkAreaComponent,
         MEditTemplateComponent,
         MDashboardComponent,
         MParkAreaQRComponent,
         FileUploadDirective
  ],
  imports: [

  ],
  providers: [],
  exports: [],
})
export class ManagersModule { }
