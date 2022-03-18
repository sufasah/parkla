export namespace RouteUrl {
  export const login = () => {
    return ``;
  };

  export const register = () => {
    return `register`;
  };

  export const parkMap = () => {
    return `parkmap`;
  };

  export const reservations = () => {
    return `reservations`;
  };

  export const profile = () => {
    return `profile`;
  };

  export const parkArea = (parkid: number, areaid:number) => {
    return `park/${parkid}/area/${areaid}`;
  };

  export const parkAreas = (parkid: number) => {
    return `park/${parkid}/areas`;
  }

  export const loadMoney = () => {
    return `load-money`;
  }
}
