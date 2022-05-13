export class ParklaError {
  innerError: any;
  message: string;
  isServerError: boolean;

  constructor(error: any) {
    let isString = typeof error === "string";
    this.innerError = isString ? null : error;
    this.message = isString ? error : "Could not reached to the server. Check your network configurations. If it is not a client side problem please wait until server become available";
    this.isServerError = isString;
  }
}
