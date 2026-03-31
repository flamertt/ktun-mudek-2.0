import styles from './PageSection.module.css'

/**
 * Başlık + açıklama (kart üst alanı). Tablo filtreleri DataTable araç çubuğunda tutulur.
 * Admin / öğretmen / öğrenci kabuklarında ortak kullanım.
 */
export function PageSection({ title, description, error, loading, children }) {
  return (
    <section className={styles.root}>
      <header className={styles.headCard}>
        <div className={styles.headAccent} aria-hidden />
        <div className={styles.headMain}>
          <div className={styles.headText}>
            <h2 className={styles.title}>{title}</h2>
            {description ? <p className={styles.desc}>{description}</p> : null}
          </div>
        </div>
      </header>

      {error ? (
        <p className={styles.error} role="alert">
          {error}
        </p>
      ) : null}
      {loading ? <p className={styles.loading}>Yükleniyor…</p> : children}
    </section>
  )
}

