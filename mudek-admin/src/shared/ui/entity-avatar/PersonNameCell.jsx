import { EntityAvatar } from './EntityAvatar.jsx'
import styles from './PersonNameCell.module.css'

/**
 * @param {{ variant: 'teacher' | 'student', name: string, seedKey?: string | number }} props
 * `seedKey` (satır id’si vb.) verilirse aynı isimli kayıtlar farklı avatar alır.
 */
export function PersonNameCell({ variant, name, seedKey }) {
  const display = name?.trim() || '—'
  const seed =
    seedKey != null && seedKey !== '' ? String(seedKey) : display === '—' ? '' : display
  return (
    <div className={styles.cell}>
      <EntityAvatar variant={variant} size="sm" seed={seed} />
      <span className={styles.name}>{display}</span>
    </div>
  )
}

