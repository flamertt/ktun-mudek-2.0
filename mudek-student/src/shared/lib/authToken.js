import { appConfig } from '../config/appConfig'

export function getStudentToken() {
  return localStorage.getItem(appConfig.storage.tokenKey)
}
