import { EventEmitter, Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, HubConnectionState, IStreamSubscriber } from '@microsoft/signalr';
import { BehaviorSubject, Subject, Subscription } from 'rxjs';
import { signalAllParks, signalAllParksReservedSpaceCount, signalConnectionUrl, signalParkAreaChanges, signalParkAreaChangesRegister, signalParkAreaChangesUnRegister, signalParkAreasReservedSpaceCount, signalParkChanges, signalParkChangesRegister, signalParkChangesUnRegister, signalParkSpaceChanges, signalParkSpaceChangesRegister, signalParkSpaceChangesUnRegister, signalReservationChanges, signalReservationChangesRegister, signalReservationChangesUnRegister } from '../constants/signalr';
import { Park } from '../models/park';
import { ParkArea } from '../models/park-area';
import { ParkSpace } from '../models/park-space';
import { Reservation } from '../models/reservation';

interface QueueItem {
  name: string;
  args: any[]
};

@Injectable({
  providedIn: 'root'
})
export class SignalrService {

  private readonly _connection: HubConnection;
  private readonly _queueAddInformer = new Subject<void>();
  private readonly _registerations = new Map<string,QueueItem>();
  private readonly _queue = <QueueItem[]>[];
  private _subscription?: Subscription;
  private _startProimse!: Promise<void>;

  connectedEvent = new BehaviorSubject<boolean | null>(null)

  get isConnected() {
    return this._connection.state == HubConnectionState.Connected;
  }

  constructor() {
    this._connection = new HubConnectionBuilder()
      .withUrl(signalConnectionUrl)
      .build();

    this._connection.on("connected", (message) => console.log(message));

    this._connection.onreconnected(() => {
      this.whenConnected();
    });

    this._connection.onclose(() => {
      this._subscription?.unsubscribe();
      this.connectedEvent.next(false);
      this.startConnection();
    });

    this.startConnection();
  }

  private startConnection() {
    this._startProimse = this._connection.start();
    this._startProimse.then(()=>{
      this.whenConnected();
    })
    .catch(() => {
      setTimeout(() => {
        this.startConnection();
      }, 10000);
    });
  }

  private whenConnected() {
    this._registerations.forEach(params => {
      this._connection.invoke(params.name, ...params.args);
    });
    this.handleQueue();
    this.connectedEvent.next(true);
  }

  private handleQueue() {
    this._subscription = this._queueAddInformer.subscribe(() => {
      const params = this._queue.shift();
      if(params)
        this._connection.invoke(params.name, ...params.args);
    })

    const queueLen = this._queue.length;
    for(let i=0; i<queueLen; i++) {
      if(this.isConnected)
        this._queueAddInformer.next();
      else
        break;
    }
  }

  private addQueue(item: QueueItem) {
    this._queue.push(item);
    if(this.isConnected)
      this._queueAddInformer.next();
  }

  private register(
    name:string, // after register invoke, server will send messages about registeration to this name and this callback
    callback: (...args: any[]) => void,
    registerInvokeItem: QueueItem,
    unregisterInvokeItem: QueueItem
  ) {
    if(!this._registerations.has(registerInvokeItem.name)) {
      this._connection.on(name, callback);
      this.addQueue(registerInvokeItem);
      this._registerations.set(registerInvokeItem.name, registerInvokeItem);
    }

    return new Subscription(() => {
      if(this._registerations.has(registerInvokeItem.name)) {
        this._connection.off(name, callback);
        this.addQueue(unregisterInvokeItem);
        this._registerations.delete(registerInvokeItem.name);
      }
  });
  }

  registerParkChanges(callback: (park: Park, isDelete: boolean) => void) {
    return this.register(
      signalParkChanges,
      callback,
      {name: signalParkChangesRegister, args: []},
      {name: signalParkChangesUnRegister, args: []}
    );
  }

  registerParkSpaceChanges(areaId: number, callback: (parkSpace: ParkSpace, isDelete: boolean) => void) {
    return this.register(
      signalParkSpaceChanges,
      callback,
      {name: signalParkSpaceChangesRegister, args: [areaId]},
      {name: signalParkSpaceChangesUnRegister, args: [areaId]}
    );
  }

  registerReservationChanges(areaId: number, callback: (reservation: Reservation, isDelete: boolean) => void) {
    return this.register(
      signalReservationChanges,
      callback,
      {name: signalReservationChangesRegister, args: [areaId]},
      {name: signalReservationChangesUnRegister, args: [areaId]}
    );
  }

  registerParkAreaChanges(parkId: string, callback: (area: ParkArea, isDelete: boolean) => void) {
    return this.register(
      signalParkAreaChanges,
      callback,
      {name: signalParkAreaChangesRegister, args: [parkId]},
      {name: signalParkAreaChangesUnRegister, args: [parkId]}
    );
  }

  GetAllParksAsStream(callbacks: IStreamSubscriber<any>) {
    return this._connection.stream(signalAllParks).subscribe(callbacks);
  }

  getAllParksReservationCountAsStream(callbacks: IStreamSubscriber<any>) {
    return this._connection.stream(signalAllParksReservedSpaceCount).subscribe(callbacks);
  }

  getParkAreasReservedSpaceCountAsStream(areaIds: number[], callbacks: IStreamSubscriber<any>) {
    return this._connection.stream(signalParkAreasReservedSpaceCount, areaIds).subscribe(callbacks);
  }

}
