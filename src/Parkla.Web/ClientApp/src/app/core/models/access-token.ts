export interface AccessToken {
  iss?: string;
  sub?: string;
  aud?: string;
  exp?: number;
  nbf?: number;
  iat?: number;
  jti?: string;
  sid?: string;
  preferred_username?: string;
  name?: string;
  email?: string;
  email_verified?: boolean;
  gender?: string;
  phone_number_verified?: boolean;
}
