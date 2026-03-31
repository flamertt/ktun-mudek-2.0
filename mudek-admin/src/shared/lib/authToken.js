import { appConfig } from '../config/appConfig'

export function getAdminToken() {
  return localStorage.getItem(appConfig.storage.tokenKey)
}
