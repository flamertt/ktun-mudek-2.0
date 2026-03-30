import styles from './SectionPage.module.css'

export function SectionPage({ title, description }) {
  return (
    <section className={styles.root}>
      <h2 className={styles.title}>{title}</h2>
      <p className={styles.description}>{description}</p>
    </section>
  )
}

