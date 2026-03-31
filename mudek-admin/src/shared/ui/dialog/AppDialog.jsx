import { useEffect, useRef } from 'react'

import styles from './AppDialog.module.css'

/**
 * @param {{
 *   open: boolean,
 *   title: string,
 *   onClose: () => void,
 *   children: import('react').ReactNode,
 *   footer?: import('react').ReactNode,
 *   size?: 'sm' | 'md' | 'lg',
 * }} props
 */
export function AppDialog({ open, title, onClose, children, footer, size = 'md' }) {
  const ref = useRef(null)

  useEffect(() => {
    const el = ref.current
    if (!el) return
    if (open) {
      if (!el.open) el.showModal()
    } else if (el.open) {
      el.close()
    }
  }, [open])

  return (
    <dialog
      ref={ref}
      className={`${styles.dialog} ${styles[`size_${size}`]}`}
      onClose={onClose}
      onClick={(e) => {
        if (e.target === ref.current) onClose()
      }}
    >
      <div className={styles.panel} onClick={(e) => e.stopPropagation()}>
        <header className={styles.header}>
          <h2 className={styles.title}>{title}</h2>
          <button type="button" className={styles.closeBtn} onClick={onClose} aria-label="Kapat">
            ×
          </button>
        </header>
        <div className={styles.body}>{children}</div>
        {footer ? <footer className={styles.footer}>{footer}</footer> : null}
      </div>
    </dialog>
  )
}
