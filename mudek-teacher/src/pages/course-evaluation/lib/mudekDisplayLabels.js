/** Sınavdaki öğe türü (API sabitleri). */
export function mudekItemTypeTr(v) {
  switch (String(v ?? '')) {
    case 'WrittenQuestion':
      return 'Yazılı soru'
    case 'AssessmentComponent':
      return 'Ölçme bileşeni'
    default:
      return v ? String(v) : '—'
  }
}

/** CLO sonuç satırı türü. */
export function mudekCloResultTypeTr(v) {
  switch (String(v ?? '')) {
    case 'Midterm':
      return 'Vize'
    case 'Final':
      return 'Final'
    case 'Makeup':
      return 'Bütünleme'
    case 'Combined':
      return 'Birleşik'
    default:
      return v ? String(v) : '—'
  }
}

/** Öğrenci satırında hangi sınavın %60 ağırlıkta kullanıldığı. */
export function mudekUsedExamTypeTr(v) {
  switch (String(v ?? '')) {
    case 'None':
      return '—'
    case 'Final':
      return 'Final'
    case 'Makeup':
      return 'Bütünleme'
    default:
      return v ? String(v) : '—'
  }
}

export function formatMudekDecimal(v, digits = 4) {
  if (v == null) return '—'
  const n = Number(v)
  if (!Number.isFinite(n)) return '—'
  return n.toFixed(digits)
}

/** Oran 0–1 aralığındaysa yüzde gösterir. */
export function formatMudekRateAsPercent(v, digits = 2) {
  if (v == null) return '—'
  const n = Number(v)
  if (!Number.isFinite(n)) return '—'
  return `${(n * 100).toFixed(digits)}%`
}
