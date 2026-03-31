export function parseMaybeNumber(value) {
  if (value == null) return null
  const s = String(value).trim()
  if (!s) return null
  const n = Number(s)
  return Number.isFinite(n) ? n : null
}

export function parseRequiredNumber(value) {
  const n = parseMaybeNumber(value)
  if (n == null) throw new Error('Geçerli sayı giriniz.')
  return n
}

