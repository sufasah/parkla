export namespace RouteUrl {


  export const login = () => {
    return ``;
  };

  export const register = () => {
    return `register`;
  };

  // USER ROUTES

  export const parkMap = () => {
    return `user/parkmap`;
  };

  export const reservations = () => {
    return `user/reservations`;
  };

  export const profile = () => {
    return `user/profile`;
  };

  export const parkArea = (parkid: number, areaid:number) => {
    return `user/park/${parkid}/area/${areaid}`;
  };

  export const parkAreas = (parkid: number) => {
    return `user/park/${parkid}/areas`;
  }

  export const loadMoney = () => {
    return `user/load-money`;
  }

  // MANAGER ROUTES

  export const mParkMap = () => {
    return `manager/parkmap`;
  };

  export const mDashboard = () => {
    return `manager/dashboard`;
  };

  export const mProfile = () => {
    return `manager/profile`;
  };

  export const mParkArea = (parkid: number, areaid:number) => {
    return `manager/park/${parkid}/area/${areaid}`;
  };

  export const mNewParkArea = (parkid: number) => {
    return `manager/park/${parkid}/area/add`;
  };

  export const mEditParkArea = (parkid: number, areaid:number) => {
    return `manager/park/${parkid}/area/${areaid}/edit`;
  };

  export const mEditParkAreaTemplate = (parkid: number, areaid:number) => {
    return `manager/park/${parkid}/area/${areaid}/edit/template`;
  };

  export const mParkAreaQR = (parkid: number, areaid:number) => {
    return `manager/park/${parkid}/area/${areaid}`;
  };

  export const mParkAreas = (parkid: number) => {
    return `manager/park/${parkid}/areas`;
  }

  export const mEditPark = (parkid: number) => {
    return `manager/park/${parkid}/edit`;
  }

  export const mNewPark = () => {
    return `manager/park/add`;
  }


}
