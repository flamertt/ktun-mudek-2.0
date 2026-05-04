/**
 * BitirmeApi kök URL’i (sonda / yok).
 * - Geliştirme: boş bırakılırsa istekler aynı origin’e gider; Vite `/api` → BitirmeApi’ye proxy’ler (CORS gerekmez).
 * - Build / prod: `VITE_API_BASE_URL` ile tam URL verin (örn. https://api.example.com).
 */
const DEFAULT_API_BASE = 'http://localhost:5010'

function normalizeBase(raw) {
  if (raw == null) return ''
  const s = String(raw).trim()
  if (!s) return ''
  return s.replace(/\/+$/, '')
}

const fromEnv = normalizeBase(import.meta.env.VITE_API_BASE_URL)

export const API_BASE_URL =
  fromEnv ||
  (import.meta.env.DEV ? '' : DEFAULT_API_BASE)

