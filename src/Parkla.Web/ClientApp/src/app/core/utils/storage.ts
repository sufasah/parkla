import { accessTokenKey, expiresKey, refreshTokenKey } from "../constants/storage";
import { TokenResponse } from "../server-models/token";

export const setStorageTokens = (accessToken: string, refreshToken: string, expires: number | string) => {
  localStorage.setItem(accessTokenKey, accessToken);
  localStorage.setItem(refreshTokenKey, refreshToken);
  localStorage.setItem(expiresKey, expires.toString());
}

export const getStorageTokens = () => {
  return <TokenResponse>{
    accessToken: localStorage.getItem(accessTokenKey),
    refreshToken: localStorage.getItem(refreshTokenKey),
    expires: Number(localStorage.getItem(expiresKey)),
  }
}

export const clearStorageTokens = () => {
  localStorage.removeItem(accessTokenKey);
  localStorage.removeItem(refreshTokenKey);
  localStorage.removeItem(expiresKey);
}
