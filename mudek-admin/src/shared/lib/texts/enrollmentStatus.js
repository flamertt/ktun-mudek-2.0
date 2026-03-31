export const ENROLLMENT_STATUS_OPTIONS = [
  { value: 'Enrolled', label: 'Kayıtlı' },
  { value: 'Passed', label: 'Geçti' },
  { value: 'Failed', label: 'Kaldı' },
  { value: 'Withdrawn', label: 'Çekildi' },
  { value: 'Repeat', label: 'Tekrar' },
]

export const DEFAULT_ENROLLMENT_STATUS = 'Enrolled'

export function getEnrollmentStatusLabel(code) {
  return ENROLLMENT_STATUS_OPTIONS.find((o) => o.value === code)?.label ?? code
}

