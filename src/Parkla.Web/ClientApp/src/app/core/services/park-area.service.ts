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
    parkId: number,
    search: string | null = null,
    orderBy: string | null = null,
    asc: boolean | null = null,
  ) {
    const params = <any>{
      nextRecord,
      pageSize,
      parkId
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

  updateArea(area: ParkArea, templateMode: boolean = false) {
    let data:any = {
      id: area.id,
      name: area.name,
      description: area.description,
      reservationsEnabled: area.reservationsEnabled,
      pricings: area.pricings
    };

    if(templateMode) {
      data = {
        id: area.id,
        templateImage: area.templateImage,
        spaces: area.spaces.map(space => ({...space, realSpaceId: space.realSpace?.id}))
      };
    }
    return this.httpClient.put<ParkArea>(apiParkAreas, data, {
      params: {
        templateMode
      }
    });
  }

  deleteArea(areaId: number) {
    return this.httpClient.delete(apiParkAreas,{body: {
      id: areaId
    }});
  }
}
