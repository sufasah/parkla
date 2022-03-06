type Gender = "Male" | "Female";

export class AppUser {
  username: string;
  email: string;
  name: string;
  surname: string;
  gender: Gender | null;
  phone: string;
  address: string | null;
  city: string;
  district: string;
  zip: string | null;
  birthDate: Date | null = null;

  constructor(
    username: string,
    email: string,
    name: string,
    surname: string,
    phone: string,
    city: string,
    district: string,
    gender: Gender | null = null,
    address: string | null = null,
    zip: string | null = null,
    birthDate: Date | null = null) {

    this.username = username;
    this.email = email;
    this.name = name;
    this.surname = surname;
    this.gender = gender;
    this.phone = phone;
    this.address = address;
    this.city = city;
    this.district = district;
    this.zip = zip;
    this. birthDate = birthDate;
  }
}
