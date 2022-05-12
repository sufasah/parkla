import { Gender } from "@app/core/enums/Gender";
import { AppUser } from "@app/core/models/app-user";

export const mockUser: AppUser = {
  id: 1,
  wallet: 250,
  username: "username",
  name: "name",
  surname: "surname",
  email: "deneme@hotmail.com",
  phone: "5555555555",
  gender: Gender.MALE,
  birthdate: null,
  city: null,
  address: null,
  district: null
}
