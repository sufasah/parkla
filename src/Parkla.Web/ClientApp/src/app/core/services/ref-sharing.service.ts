import { ElementRef, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class RefSharingService {

  private _data:{[key:string]:any} = {};

  constructor() {}

  setData(key:string, data:any){
    this._data[key] = data;
  }

  getData<T>(key:string): T {
    return this._data[key];
  }

  removeData(key:string) {
    delete this._data[key];
  }
}
