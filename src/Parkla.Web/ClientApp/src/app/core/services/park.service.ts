import { HttpClient } from '@angular/common/http';
import { Injectable, OnDestroy } from '@angular/core';
import { MessageService } from 'primeng/api';
import { Subject, Subscription } from 'rxjs';
import { apiParks } from '../constants/http';
import { ChangablePark, Park } from '../models/park';
import { AuthService } from './auth.service';
import { SignalrService } from './signalr.service';

export interface InformerItem{
  park:ChangablePark,
  isUserPark: boolean,
  isDeleted: boolean
}

@Injectable({
  providedIn: 'root'
})
export class ParkService implements OnDestroy {

  private _parks = new Map<string, ChangablePark>();
  private unsubscribe: Subscription[] = [];

  get copyParks() {
    return new Map(this._parks);
  }

  parkInformer = new Subject<InformerItem>();

  constructor(
    private httpClient: HttpClient,
    private authService: AuthService,
    private signalrService: SignalrService,
    private messageService: MessageService
  ) { }

  LoadParks() {
    const sub = this.signalrService.registerParkChanges((park: Park, isDelete: boolean) => {
      if(!park || !park.id) return;

      if(isDelete)
        this.deleteMemoryPark(park);
      else
        this.setOrAddMemoryPark(park);
    });

    const sub2 = this.signalrService.connectedEvent.subscribe(() => {
      const sub3 = this.signalrService.GetAllParksAsStream({
        next: (park: Park) => {
          if(park && park.id)
            this.setOrAddMemoryPark(park)
        },
        error: (err:any) => {
          this.messageService.add({
            key: "global",
            life:5000,
            severity:"error",
            summary: "Fetch Parks",
            detail: err,
            icon: "pi-lock",
          });
        },
        complete: () => {
          sub3.dispose();
        }
      });

      const sub4 = this.signalrService.getAllParksReservationCountAsStream({
        next: (item: {parkId: string, reservedSpaceCount: number}) => {
          let park = this._parks.get(item.parkId)

          if(park && park.reservedSpace != item.reservedSpaceCount) {
            park.reservedSpace = item.reservedSpaceCount;
            park.subject.next();
          }
        },
        error: (err:any) => {
          console.log(err);
        },
        complete: () => {
          sub4.dispose();
        }
      });
    })

    this.unsubscribe.push(sub, sub2);
  }

  private setOrAddMemoryPark(park: Park) {
    const curPark = this._parks.get(park.id)
    if(curPark)
      this.setMemoryPark(curPark, park)
    else
      this.addMemoryPark(park);
  }

  private setMemoryPark(target: ChangablePark, source: Park) {
    if(target.xmin < source.xmin) {
      if(source.statusUpdateTime) source.statusUpdateTime = new Date(source.statusUpdateTime);

      delete (<any>source)['subject'];
      Object.assign(target,source);
      target.subject.next(undefined);
    }
  }

  private addMemoryPark(park: Park) {
    if(park.statusUpdateTime) park.statusUpdateTime = new Date(park.statusUpdateTime);

    const cPark: ChangablePark = {
      ...park,
      subject: new Subject()
    }

    this._parks.set(park.id,cPark);
    this.parkInformer.next({
      park: cPark,
      isUserPark: this.isUserPark(park),
      isDeleted: false
    });
  }

  private deleteMemoryPark(park: Park) {
    const curPark = this._parks.get(park.id);
    if(curPark && curPark.xmin <= park.xmin) {
      this.parkInformer.next({
        park: curPark,
        isUserPark: this.isUserPark(curPark),
        isDeleted: true
      })
      this._parks.delete(park.id);
    }
  }

  public isUserPark(park: Park) {
    return !!park.user.id && this.authService.isLoggedIn() && park.user.id == Number(this.authService.accessToken!.sub)
  }

  getAllParks() {
    return this.httpClient.get<Park[]>(apiParks+"/all");
  }

  getPark(id: string) {
    return this.httpClient.get<Park>(apiParks+"/"+id);
  }

  addPark(park: Park) {
    return this.httpClient.post<Park>(apiParks, {
      name: park.name,
      userId: park.user.id,
      location: park.location,
      latitude: park.latitude,
      longitude: park.longitude,
      extras: park.extras,
      xmin: park.xmin
    });
  }

  updatePark(park: Park) {
    return this.httpClient.put<Park>(apiParks, {
      id: park.id,
      name: park.name,
      location: park.location,
      latitude: park.latitude,
      longitude: park.longitude,
      extras: park.extras,
      xmin: park.xmin
    });
  }

  deletePark(park: Park) {
    return this.httpClient.delete<Park>(apiParks, {body:{
      id: park.id,
      xmin: park.xmin
    }});
  }

  ngOnDestroy(): void {
    this.unsubscribe.forEach(x => x.unsubscribe());
  }
}
