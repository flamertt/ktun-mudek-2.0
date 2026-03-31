import { appConfig } from '../config/appConfig'

export function getTeacherToken() {
  return localStorage.getItem(appConfig.storage.tokenKey)
}

