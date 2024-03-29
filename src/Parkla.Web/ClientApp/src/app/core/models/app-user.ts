import { Gender } from "../enums/Gender";
import { City } from "./city";
import { District } from "./district";

export interface AppUser {
  id: number;
  wallet: number;
  username: string;
  email: string;
  name: string;
  surname: string;
  phone: string;
  cityId: number | null;
  city: City | null;
  districtId: number | null;
  district: District | null;
  birthdate: string | null;
  gender: Gender | null;
  address: string | null;
  xmin: number;
}
