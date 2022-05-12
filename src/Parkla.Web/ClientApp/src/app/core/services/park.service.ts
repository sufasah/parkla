import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { apiParks } from '../constants/http';
import { Park } from '../models/park';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class ParkService {

  allParks = <{[id: number]: Park}>{};
  userParks = <{[id: number]: Park}>{};

  constructor(
    private httpClient: HttpClient,
    private authService: AuthService
  ) { }

  LoadParks() {
    let signalrparkaddordelete = (park: Park, isDelete: boolean) => {
      if(!park.id) return;

      if(isDelete) {
        delete this.allParks[park.id];
        delete this.userParks[park.id];
      }

      if(this.isUserPark(park)) {
          this.userParks[park.id] = park;
      }
      this.allParks[park.id] = park;
    };

    this.getAllParks().subscribe(parks => {
      parks.forEach(park => {
        if(this.isUserPark(park)) {
          this.userParks[park.id] = park;
        }
        this.allParks[park.id] = park;
      });
    });
  }

  private isUserPark(park: Park) {
    return park.user.id && this.authService.isLoggedIn() && park.user.id == Number(this.authService.accessToken!.sub)
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
