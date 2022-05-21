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
    parkId: string,
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
      pricings: area.pricings,
      xmin: area.xmin
    });
  }

  updateArea(area: ParkArea, templateMode: boolean = false) {
    let data:any = {
      id: area.id,
      parkId: area.parkId,
      name: area.name,
      description: area.description,
      reservationsEnabled: area.reservationsEnabled,
      pricings: area.pricings,
      xmin: area.xmin
    };

    if(templateMode) {
      data = {
        id: area.id,
        parkId: area.parkId,
        templateImage: area.templateImage,
        spaces: area.spaces.map(space => ({...space, realSpaceId: space.realSpace?.id, realSpace: null})),
        xmin: area.xmin
      };
    }
    return this.httpClient.put<ParkArea>(apiParkAreas, data, {
      params: {
        templateMode
      }
    });
  }

  deleteArea(area: ParkArea) {
    return this.httpClient.delete(apiParkAreas,{body: {
      id: area.id,
      parkId: area.parkId,
      xmin: area.xmin
    }});
  }
}
