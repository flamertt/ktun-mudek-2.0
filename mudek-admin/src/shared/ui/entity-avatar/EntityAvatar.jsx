import { GraduationCap, School } from 'lucide-react'
import { useState } from 'react'

import { pickUserAvatarUrl } from '../../config/branding.js'
import styles from './EntityAvatar.module.css'

/**
 * Tablo satırı avatarı: `branding.avatars.userPool` içinden seed’e göre URL; yüklenemezse Lucide ikon.
 * @param {{ variant: 'teacher' | 'student', size?: 'sm' | 'md', seed?: string }} props
 */
export function EntityAvatar({ variant, size = 'sm', seed = '' }) {
  const [failed, setFailed] = useState(false)
  const url = pickUserAvatarUrl(String(seed), variant)
  const Icon = variant === 'teacher' ? School : GraduationCap
  const cls = `${styles.root} ${styles[variant]} ${size === 'md' ? styles.md : styles.sm}`

  if (url && !failed) {
    return (
      <span className={cls}>
        <img
          className={styles.photo}
          src={url}
          alt=""
          loading="lazy"
          decoding="async"
          referrerPolicy="no-referrer"
          onError={() => setFailed(true)}
        />
      </span>
    )
  }

  return (
    <span className={cls} aria-hidden>
      <Icon className={styles.icon} strokeWidth={2} />
    </span>
  )
}

