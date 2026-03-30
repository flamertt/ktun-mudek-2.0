import { postJson, postJsonWithAuth } from './httpClient'

const STUDENT_LOGIN_ENDPOINT = '/api/StudentAuth/login'
const STUDENT_LOGOUT_ENDPOINT = '/api/StudentAuth/logout'

export function loginAsStudent({ email, password }) {
  return postJson(STUDENT_LOGIN_ENDPOINT, { email, password })
}

export async function logoutCurrentUser(token) {
  if (!token) return null

  try {
    return await postJsonWithAuth(STUDENT_LOGOUT_ENDPOINT, {}, token)
  } catch {
    return null
  }
}
