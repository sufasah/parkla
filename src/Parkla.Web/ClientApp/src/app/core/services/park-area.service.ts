import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { apiParkAreas } from '../constants/http';
import { ParkArea } from '../models/park-area';

@Injectable({
  providedIn: 'root'
})
export class ParkAreaService {

  constructor(private httpClient: HttpClient) { }

  getAreas() {
    return this.httpClient.get<ParkArea[]>(apiParkAreas+"/all");
  }

  getAreasPage(pageNumber: number, pageSize: number) {
    return this.httpClient.get<ParkArea[]>(apiParkAreas, {
      params: {
        pageNumber,
        pageSize
      },
      observe: "response"
    });
  }

  getArea(id:number) {
    return this.httpClient.get<ParkArea>(apiParkAreas+"/"+id);
  }

  addArea(area: ParkArea) {
    return this.httpClient.post<ParkArea>(apiParkAreas, {
      id: area.id,
      parkId: area.parkId,
      name: area.name,
      description: area.description,
      reservationsEnabled: area.reservationsEnabled
    });
  }

  updateArea(area: ParkArea) {
    return this.httpClient.put<ParkArea>(apiParkAreas, {
      id: area.id,
      name: area.name,
      description: area.description,
      reservationsEnabled: area.reservationsEnabled
    });
  }

  deleteArea(areaId: number) {
    return this.httpClient.delete(apiParkAreas,{body: {
      id: areaId
    }});
  }
}
