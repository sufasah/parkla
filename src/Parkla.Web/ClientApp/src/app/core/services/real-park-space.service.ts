import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { apiParkSpaces, apiRealParkSpaces } from '../constants/http';
import { ParkSpaceReal } from '../models/park-space-real';

@Injectable({
  providedIn: 'root'
})
export class RealParkSpaceService {

  constructor(private httpClient: HttpClient) { }

  getPage(parkId: number, nextRecord: number, pageSize: number, search: string | null = null) {
    return this.httpClient.get<ParkSpaceReal[]>(apiRealParkSpaces, !!search ? {
      params: {
        nextRecord,
        parkId,
        pageSize,
        s: search
      },
      observe: "response"
    } : {
      params: {
        nextRecord,
        parkId,
        pageSize
      },
      observe: "response"
    })
  }

  addRealSpace(space: ParkSpaceReal) {
    return this.httpClient.post<ParkSpaceReal>(apiRealParkSpaces, {...space});
  }

  deleteRealSpace(id: number) {
    return this.httpClient.delete(apiRealParkSpaces, {
      body: {
        id
      }
    });
  }
}
