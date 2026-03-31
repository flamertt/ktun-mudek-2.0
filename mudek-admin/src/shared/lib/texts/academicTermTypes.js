export const TERM_TYPE_OPTIONS = [
  { value: 'Guz', label: 'Güz' },
  { value: 'Bahar', label: 'Bahar' },
  { value: 'Yaz', label: 'Yaz' },
]

export const DEFAULT_TERM_TYPE = 'Guz'

export function getTermTypeLabel(value) {
  return TERM_TYPE_OPTIONS.find((t) => t.value === value)?.label ?? value
}

