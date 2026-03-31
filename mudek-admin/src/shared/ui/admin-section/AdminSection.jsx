import styles from './AdminSection.module.css'

export function AdminSection({ title, description, toolbar, error, loading, children }) {
  return (
    <section className={styles.root}>
      <h2 className={styles.title}>{title}</h2>
      {description ? <p className={styles.desc}>{description}</p> : null}
      {toolbar ? <div className={styles.toolbar}>{toolbar}</div> : null}
      {error ? (
        <p className={styles.error} role="alert">
          {error}
        </p>
      ) : null}
      {loading ? <p className={styles.loading}>Yükleniyor…</p> : children}
    </section>
  )
}
