import { API_BASE_URL } from '@shared/config/env.js'

export { API_BASE_URL }

function normalizeBearerToken(token) {
  if (token == null) return ''
  const s = String(token).trim()
  if (!s) return ''
  return s.replace(/^Bearer\s+/i, '').trim()
}

function buildUrl(path, query) {
  const base = path.startsWith('http') ? path : `${API_BASE_URL}${path}`
  if (!query || typeof query !== 'object') return base

  const sp = new URLSearchParams()
  for (const [key, value] of Object.entries(query)) {
    if (value != null && value !== '') sp.set(key, String(value))
  }

  const q = sp.toString()
  return q ? `${base}?${q}` : base
}

async function parseJsonSafely(response) {
  const text = await response.text()
  if (!text) return null

  try {
    return JSON.parse(text)
  } catch {
    return null
  }
}

function errorMessageFromResponse(data, response) {
  if (data && typeof data === 'object') {
    if (data.message) return data.message
    if (data.title) return data.title
    if (Array.isArray(data.errors) && data.errors[0]) return String(data.errors[0])
  }

  if (typeof data === 'string') return data
  return `İstek başarısız (${response.status})`
}

/**
 * @param {string} method
 * @param {string} path
 * @param {{ token?: string, body?: unknown, query?: Record<string, string | number | boolean> }} [options]
 */
export async function requestJson(method, path, options = {}) {
  const { token, body, query } = options
  const url = buildUrl(path, query)
  const headers = {}

  const hasBody = body != null && method !== 'GET' && method !== 'DELETE'
  if (hasBody) headers['Content-Type'] = 'application/json'
  const bearer = normalizeBearerToken(token)
  if (bearer) headers.Authorization = `Bearer ${bearer}`

  const init = { method, headers }
  if (hasBody) init.body = JSON.stringify(body)

  const response = await fetch(url, init)

  if (response.status === 204 || response.status === 205) {
    if (!response.ok) {
      throw new Error(`İstek başarısız (${response.status})`)
    }
    return null
  }

  const data = await parseJsonSafely(response)

  if (!response.ok) {
    throw new Error(errorMessageFromResponse(data, response))
  }
  return data
}

export const getJson = (path, options) => requestJson('GET', path, options)
export const postJson = (path, body) => requestJson('POST', path, { body })
export const postJsonWithAuth = (path, body, token) =>
  requestJson('POST', path, { body, token })
export const putJsonWithAuth = (path, body, token) =>
  requestJson('PUT', path, { body, token })
export const deleteJsonWithAuth = (path, token) => requestJson('DELETE', path, { token })
export const patchJsonWithAuth = (path, token) => requestJson('PATCH', path, { token })
