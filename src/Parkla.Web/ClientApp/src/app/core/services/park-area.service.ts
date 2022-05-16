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

  getAreasPage(
    nextRecord: number,
    pageSize: number,
    search: string | null = null,
    orderBy: string | null = null,
    asc: boolean | null = null,
  ) {
    const params = <any>{
      nextRecord,
      pageSize,
    };

    if(search) params.s = search;
    if(orderBy) params.orderBy = orderBy;
    if(asc) params.asc = asc;

    return this.httpClient.get<ParkArea[]>(apiParkAreas, {
      params,
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
      reservationsEnabled: area.reservationsEnabled,
      pricings: area.pricings
    });
  }

  updateArea(area: ParkArea) {
    return this.httpClient.put<ParkArea>(apiParkAreas, {
      id: area.id,
      name: area.name,
      description: area.description,
      reservationsEnabled: area.reservationsEnabled,
      pricings: area.pricings
    });
  }

  deleteArea(areaId: number) {
    return this.httpClient.delete(apiParkAreas,{body: {
      id: areaId
    }});
  }
}
