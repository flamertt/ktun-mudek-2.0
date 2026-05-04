import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import sectionStyles from '@shared/ui/page-section/PageSection.module.css'

const DEFAULT_TEXT =
  'Bu işlemler KTÜN üniversite bilgi sistemi üzerinden yönetilir. Bu panelde ilgili yerel API uçları bulunmamaktadır.'

/**
 * @param {{ title: string, description?: string }} props
 */
export function UniversityManagedPlaceholderPage({ title, description }) {
  return (
    <PageSection title={title} description={description ?? DEFAULT_TEXT}>
      <p className={sectionStyles.muted}>{DEFAULT_TEXT}</p>
    </PageSection>
  )
}
