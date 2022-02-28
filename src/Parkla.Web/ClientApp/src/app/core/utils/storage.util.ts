import { accessTokenKey, expiresKey, refreshTokenKey } from "../constants/storage.const";

export const setStorageTokens = (accessToken: string, refreshToken: string, expires: number | string) => {
  localStorage.setItem(accessTokenKey, accessToken);
  localStorage.setItem(refreshTokenKey, refreshToken);
  localStorage.setItem(expiresKey, expires.toString());
}
