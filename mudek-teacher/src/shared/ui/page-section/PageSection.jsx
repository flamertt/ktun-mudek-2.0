import styles from './PageSection.module.css'

/**
 * Sayfa başlığı (PageSection.headCard): `root.css` içindeki `--page-card-*` ve `--table-header-bg`
 * token’larıyla hizalanır. Ek kart kutuları için global `pageSurfaceCard` sınıfını veya aynı CSS
 * değişkenlerini kullanın.
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

