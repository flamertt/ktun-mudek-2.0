import { postJson, postJsonWithAuth } from './httpClient'

const ADMIN_LOGIN_ENDPOINT = '/api/AdminAuth/login'
const ADMIN_LOGOUT_ENDPOINT = '/api/AdminAuth/logout'

export function loginAsAdmin({ email, password }) {
  return postJson(ADMIN_LOGIN_ENDPOINT, { email, password })
}

export async function logoutCurrentUser(token) {
  if (!token) return null

  try {
    return await postJsonWithAuth(ADMIN_LOGOUT_ENDPOINT, {}, token)
  } catch {
    return null
  }
}
