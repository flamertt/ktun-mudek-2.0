import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import { appConfig } from '../../../shared/config/appConfig'
import styles from './HomePage.module.css'

export function HomePage() {
  return (
    <PageSection title={appConfig.home.title} description={appConfig.home.subtitle} loading={false}>
      <div className={styles.body}>
        <div className={styles.stats}>
          {appConfig.home.stats.map((stat) => (
            <article key={stat.label} className={styles.card}>
              <p className={styles.statLabel}>{stat.label}</p>
              <p className={styles.statValue}>{stat.value}</p>
            </article>
          ))}
        </div>

        <article className={styles.list}>
          <h3 className={styles.listTitle}>{appConfig.ui.homeFocusTitle}</h3>
          {appConfig.home.highlights.map((item) => (
            <p key={item} className={styles.listItem}>
              {item}
            </p>
          ))}
        </article>
      </div>
    </PageSection>
  )
}
