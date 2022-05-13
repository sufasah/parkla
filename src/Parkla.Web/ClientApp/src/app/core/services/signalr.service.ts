import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ReplaySubject, Subject, Subscription } from 'rxjs';
import { signalConnectionUrl, signalParkChanges, signalParkChangesRegister, signalParkChangesUnRegister } from '../constants/signalr';
import { Park } from '../models/park';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {

  private _connection: HubConnection;
  private _queueAddInformer = new Subject<void>();
  private _subscription?: Subscription;
  private _queue = <{name: string; args: any[]}[]>[];
  private _connected = false;

  constructor() {
    this._connection = new HubConnectionBuilder()
      .withUrl(signalConnectionUrl)
      .build();

    this._connection.on("connected", (message) => console.log(message));

    this._connection.onreconnected(() => {
      this.whenConnected();
    });

    this._connection.onclose(() => {
      this._connected = false;
      this._subscription?.unsubscribe();
      this.startConnection();
    });

    this.startConnection();
  }

  private startConnection() {
    this._connection.start().then(()=>{
      this.whenConnected();
    }).catch(() => {
      setTimeout(() => {
        this.startConnection();
      }, 10000);
    });
  }

  private whenConnected() {
    this._connected = true;
    this.handleQueue();
  }

  private handleQueue() {
    this._subscription = this._queueAddInformer.subscribe(() => {
      const params = this._queue.shift();
      if(params)
        this._connection.invoke(params.name, ...params.args);
    })

    const queueLen = this._queue.length;
    for(let i=0; i<queueLen; i++) {
      if(this._connected)
        this._queueAddInformer.next();
      else
        break;
    }
  }

  private addQueue(item: {name: string; args: any[]}) {
    this._queue.push(item);
    if(this._connected)
      this._queueAddInformer.next();
  }

  registerParkChanges(callback: (park: Park, isDelete: boolean) => void) {
    //this._connection.onreconnected() REGISTER AGAIN BECAUSE SERVER MAY CRASHES AND THE GROUP MAP RECORDS MAY BE LOST;
    //SO REGISTER WILL BE BASE METHOD TO HANDLE REGISTERINGS AND RECONNECTIONS
    //AND THIS REGISTERPARKCHANGES WILL ONLY PASS INVOKE ARGUMENTS INSTEAD OF HANDLING THESE REGISTERATIONS.
    this._connection.on(signalParkChanges, callback);
    this.addQueue({
      name: signalParkChangesRegister,
      args: []
    });

    return new Subscription(() => {
      this.addQueue({
        name: signalParkChangesUnRegister,
        args: []
      });
      this._connection.off(signalParkChanges, callback);
    });
  }

}
