import { Gender } from "../enums/Gender";

export interface AppUser {
  username: string;
  email: string;
  name: string;
  surname: string;
  birthdate?: Date;
  gender?: Gender;
  phone: string;
  city: string;
  district: string;
  address?: string;
  zip?: string;//delete
}
