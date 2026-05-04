/**
 * GET /api/Teacher/academic-terms — üniversite şeması { id, ad } veya camel eşlenikleri.
 */
export function academicTermRowId(t) {
  if (t == null || typeof t !== 'object') return ''
  const v = t.academicTermId ?? t.AcademicTermId ?? t.id ?? t.Id
  return v != null && v !== '' ? String(v) : ''
}

export function academicTermRowLabel(t) {
  if (t == null || typeof t !== 'object') return '—'
  return (
    t.academicTermName ??
    t.AcademicTermName ??
    t.ad ??
    t.Ad ??
    t.name ??
    t.Name ??
    '—'
  )
}
