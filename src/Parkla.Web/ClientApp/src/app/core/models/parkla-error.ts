export class ParklaError {
  innerError: any;
  message: string;

  constructor(error: any) {
    let isString = typeof error === "string";
    this.innerError = isString ? null : error;
    this.message = isString ? error : "Connection error. Request could not reach server endpoints";
  }
}
