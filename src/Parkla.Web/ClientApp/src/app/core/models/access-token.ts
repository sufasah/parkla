import { Gender } from "../enums/Gender";

export interface AccessToken {
  sub?: string;
  exp?: number;
  nbf?: number;
  iat?: number;
  sid?: string;
  preferred_username: string;
  name: string;
  family_name: string;
  email: string;
  email_verified: boolean;
  gender: Gender;
  birthdate: Date;
  phone_number: string;
  address: TokenAddress;
}

export interface TokenAddress {
  locality: string;
  region: string;
  street_address: string;
}
