export interface AccessToken {
  iss?: string;
  sub?: string;
  aud?: string;
  exp?: number;
  nbf?: number;
  iat?: number;
  jti?: string;
  sid?: string;
  name?: string;
  preferred_username?: string;
  email?: string;
  email_verified?: boolean;
  gender?: string;
  birthdate?: string;
  phone_number?: string;
  phone_number_verified?: boolean;
  address?: string;
  roles?: [string];
  groups?: [string];
}
