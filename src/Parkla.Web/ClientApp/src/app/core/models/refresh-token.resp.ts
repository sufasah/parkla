export interface RefreshTokenResp {
  accessToken: string;
  refreshToken: string;
  tokenType: string;
  expires: number;
}
