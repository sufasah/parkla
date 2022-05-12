import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { mockParks } from '@app/mock-data/parking-lots';
import { BehaviorSubject, ReplaySubject, Subject } from 'rxjs';
import { apiParks } from '../constants/http';
import { ChangablePark, Park } from '../models/park';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class ParkService {

  private _parks = <{[id: number]: ChangablePark}>{};

  parkInformer = new ReplaySubject<{
    park:ChangablePark,
    isUserPark: boolean,
    isDeleted: boolean
  }>(undefined, 60*1000);

  constructor(
    private httpClient: HttpClient,
    private authService: AuthService
  ) { }

  LoadParks() {
    let signalrparkaddordelete = (park: Park, isDelete: boolean) => {
      if(!park) return;

      if(isDelete)
        this.deleteMemoryPark(park);
      else
        this.setOrAddMemoryPark(park);
    };

    this.getAllParks().subscribe(parks => {
      parks.forEach(park => {
        if(park)
          this.addMemoryParkIfNotExist(park)
      });

    });
  }

  private setOrAddMemoryPark(park: Park) {
    if(!park.id) return;

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
    if(!park.id) return;
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
    if(!park.id) return;
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
      location: park.location,
      latitude: park.latitude,
      longitude: park.longitude,
      extras: park.extras
    });
  }

  updatePark(park: Park) {
    return this.httpClient.put<Park>(apiParks, {
      id: park.id,
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
}
