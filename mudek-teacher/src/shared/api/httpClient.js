import { API_BASE_URL } from '@shared/config/env.js'

async function parseJsonSafely(response) {
  const text = await response.text()
  if (!text) return null

  try {
    return JSON.parse(text)
  } catch {
    return null
  }
}

export async function postJson(path, body) {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  })

  const data = await parseJsonSafely(response)

  if (!response.ok) {
    const message = data?.message ?? 'Giriş sırasında bir hata oluştu.'
    throw new Error(message)
  }

  return data
}

export async function postJsonWithAuth(path, body, token) {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(body),
  })

  const data = await parseJsonSafely(response)

  if (!response.ok) {
    const message = data?.message ?? 'İstek sırasında bir hata oluştu.'
    throw new Error(message)
  }

  return data
}
