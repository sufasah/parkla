import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, OnDestroy } from '@angular/core';
import { mockParks } from '@app/mock-data/parking-lots';
import { MessageService } from 'primeng/api';
import { delay, ReplaySubject, Subject, Subscription } from 'rxjs';
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

  private _parks = new Map<number, ChangablePark>();
  private _signalrBeforeStreamDone = new Set<number>();
  private _allParksStreamDone = false;
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

      if(!this._allParksStreamDone)
        this._signalrBeforeStreamDone.add(park.id);

      if(isDelete)
        this.deleteMemoryPark(park);
      else
        this.setOrAddMemoryPark(park);
    });

    const sub2 = this.signalrService.connectedEvent.subscribe(() => {
      this._allParksStreamDone = false;
      const sub3 = this.signalrService.GetAllParksAsStream({
        next: (park: Park) => {
          if(park && park.id && !this._signalrBeforeStreamDone.has(park.id))
            this.addMemoryParkIfNotExist(park)
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
          this._allParksStreamDone = true;
          this._signalrBeforeStreamDone.clear();
          sub3.dispose();
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

  private addMemoryParkIfNotExist(park: Park) {
    const curPark = this._parks.get(park.id);
    if(!curPark)
      this.addMemoryPark(park);
  }

  private setMemoryPark(target: ChangablePark, source: Park) {
    delete (<any>source)['subject'];
    Object.assign(target,source);
    target.subject.next(undefined);
  }

  private addMemoryPark(park: Park) {
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
    if(curPark) {
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

  getPark(id: number) {
    return this.httpClient.get<Park>(apiParks+"/"+id);
  }

  addPark(park: Park) {
    return this.httpClient.post<Park>(apiParks, {
      name: park.name,
      userId: park.user.id,
      location: park.location,
      latitude: park.latitude,
      longitude: park.longitude,
      extras: park.extras
    });
  }

  updatePark(park: Park) {
    return this.httpClient.put<Park>(apiParks, {
      id: park.id,
      userId: park.user.id,
      name: park.name,
      location: park.location,
      latitude: park.latitude,
      longitude: park.longitude,
      extras: park.extras
    });
  }

  deletePark(id: number) {
    return this.httpClient.delete<Park>(apiParks, {body:{
      id: id,
      userId: this.authService.accessToken?.sub
    }});
  }

  ngOnDestroy(): void {
    this.unsubscribe.forEach(x => x.unsubscribe());
  }
}
