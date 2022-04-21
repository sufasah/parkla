import { accessTokenKey, expiresKey, refreshTokenKey } from "../constants/storage";

export const setStorageTokens = (accessToken: string, refreshToken: string, expires: number | string) => {
  localStorage.setItem(accessTokenKey, accessToken);
  localStorage.setItem(refreshTokenKey, refreshToken);
  localStorage.setItem(expiresKey, expires.toString());
}

export const clearStorageTokens = () => {
  localStorage.removeItem(accessTokenKey);
  localStorage.removeItem(refreshTokenKey);
  localStorage.removeItem(expiresKey);
}
