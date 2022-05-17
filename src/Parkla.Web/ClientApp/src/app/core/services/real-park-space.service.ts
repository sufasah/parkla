import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { apiParkSpaces } from '../constants/http';
import { ParkSpaceReal } from '../models/park-space-real';

@Injectable({
  providedIn: 'root'
})
export class RealParkSpaceService {

  constructor(private httpClient: HttpClient) { }

  getPage(nextRecord: number, pageSize: number, search: string | null = null) {
    return this.httpClient.get<ParkSpaceReal[]>(apiParkSpaces, !!search ? {
      params: {
        nextRecord,
        pageSize,
        s: search
      },
      observe: "response"
    } : {
      params: {
        nextRecord,
        pageSize
      },
      observe: "response"
    })
  }
}
