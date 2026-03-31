import { ClipboardList } from 'lucide-react'

import { appConfig } from '../../../shared/config/appConfig'
import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import styles from './SurveyManagementPage.module.css'

export function SurveyManagementPage() {
  const page = appConfig.pages.surveyManagement

  return (
    <PageSection title={page.title} description={page.description} loading={false}>
      <div className={styles.panel}>
        <div className={styles.iconWrap} aria-hidden>
          <ClipboardList strokeWidth={1.5} size={28} />
        </div>
        <p className={styles.text}>Anket oluşturma ve sonuç takibi bu bölümde yer alacak.</p>
      </div>
    </PageSection>
  )
}
