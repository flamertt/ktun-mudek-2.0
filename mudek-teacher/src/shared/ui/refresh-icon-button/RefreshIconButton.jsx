import { RefreshCw } from 'lucide-react'

import formStyles from '../admin-form/AdminForm.module.css'
import styles from './RefreshIconButton.module.css'

/**
 * Liste / özet verilerini tekrar yüklemek için araç çubuğu veya filtre satırına yerleştirin.
 *
 * @param {{
 *   onClick: () => void,
 *   disabled?: boolean,
 *   loading?: boolean,
 *   title?: string
 * }} props
 */
export function RefreshIconButton({ onClick, disabled, loading, title = 'Yenile' }) {
  return (
    <button
      type="button"
      className={`${formStyles.btn} ${formStyles.btnGhost}`}
      onClick={onClick}
      disabled={disabled || loading}
      title={title}
      aria-label={title}
    >
      <RefreshCw size={18} aria-hidden className={loading ? styles.spin : undefined} />
    </button>
  )
}

