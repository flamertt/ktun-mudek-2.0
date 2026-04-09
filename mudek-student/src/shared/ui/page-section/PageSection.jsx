import styles from './PageSection.module.css'

/**
 * Sayfa başlığı (PageSection.headCard): `root.css` içindeki `--page-card-*` ve `--table-header-bg`
 * token’larıyla hizalanır. Ek kart kutuları için global `pageSurfaceCard` sınıfını veya aynı CSS
 * değişkenlerini kullanın.
 * `fullWidthCopy`: başlık altı açıklama satırını dar sütun yerine içerik genişliğinde gösterir.
 */
export function PageSection({ title, description, error, loading, children, fullWidthCopy = false }) {
  return (
    <section className={styles.root}>
      <header className={styles.headCard}>
        <div className={styles.headAccent} aria-hidden />
        <div className={`${styles.headMain} ${fullWidthCopy ? styles.headMainWide : ''}`.trim()}>
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

