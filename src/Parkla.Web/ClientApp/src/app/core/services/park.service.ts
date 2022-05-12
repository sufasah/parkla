import { HttpClient } from '@angular/common/http';
import { Injectable, OnDestroy } from '@angular/core';
import { mockParks } from '@app/mock-data/parking-lots';
import { delay, ReplaySubject, Subject, Subscription } from 'rxjs';
import { apiParks } from '../constants/http';
import { ChangablePark, Park } from '../models/park';
import { AuthService } from './auth.service';
import { SignalrService } from './signalr.service';

@Injectable({
  providedIn: 'root'
})
export class ParkService implements OnDestroy {

  private _parks = <{[id: number]: ChangablePark}>{};
  private _signalrBeforeRequest = new Set<number>();
  private _requestDone = false;
  private unsubscribe: Subscription[] = [];

  parkInformer = new ReplaySubject<{
    park:ChangablePark,
    isUserPark: boolean,
    isDeleted: boolean
  }>(undefined, 60*1000);

  constructor(
    private httpClient: HttpClient,
    private authService: AuthService,
    private signalrService: SignalrService
  ) { }

  LoadParks() {
    const sub = this.signalrService.registerParkChanges((park: Park, isDelete: boolean) => {
      if(!park || !park.id) return;

      if(!this._requestDone)
        this._signalrBeforeRequest.add(park.id);

      if(isDelete)
        this.deleteMemoryPark(park);
      else
        this.setOrAddMemoryPark(park);
    });

    this.unsubscribe.push(sub);

    this.getAllParks().pipe(delay(3000)).subscribe(parks => {
      this._requestDone = true;
      parks.forEach(park => {
        if(park && park.id && !this._signalrBeforeRequest.has(park.id))
          this.addMemoryParkIfNotExist(park)
      });
      this._signalrBeforeRequest.clear();
    });
  }

  private setOrAddMemoryPark(park: Park) {
    const curPark = this._parks[park.id]
    if(curPark)
      this.setMemoryPark(curPark, park)
    else
      this.addMemoryPark(park);
  }

  private addMemoryParkIfNotExist(park: Park) {
    const curPark = this._parks[park.id];
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

    this._parks[park.id] = cPark;
    this.parkInformer.next({
      park: cPark,
      isUserPark: this.isUserPark(park),
      isDeleted: false
    });
  }

  private deleteMemoryPark(park: Park) {
    const curPark = this._parks[park.id];
    if(curPark) {
      this.parkInformer.next({
        park: curPark,
        isUserPark: this.isUserPark(curPark),
        isDeleted: true
      })
      delete this._parks[park.id];
    }
  }

  private isUserPark(park: Park) {
    return !!park.user.id && this.authService.isLoggedIn() && park.user.id == Number(this.authService.accessToken!.sub)
  }

  getAllParks() {
    return this.httpClient.get<Park[]>(apiParks+"/all");
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

  deletePark(park: Park) {
    return this.httpClient.delete<Park>(apiParks, {body:{
      ...park
    }});
  }

  ngOnDestroy(): void {
    this.unsubscribe.forEach(x => x.unsubscribe());
  }
}
