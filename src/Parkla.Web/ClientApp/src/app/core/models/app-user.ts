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
  city?: City;
  district?: District;
  birthdate?: string;
  gender?: Gender;
  address?: string;
}
