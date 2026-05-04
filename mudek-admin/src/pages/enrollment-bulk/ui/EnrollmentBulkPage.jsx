import { appConfig } from '../../../shared/config/appConfig'
import { UniversityManagedPlaceholderPage } from '../../../shared/ui/university-placeholder/UniversityManagedPlaceholderPage.jsx'

export function EnrollmentBulkPage() {
  const page = appConfig.pages.enrollmentBulk
  return <UniversityManagedPlaceholderPage title={page.title} description={page.description} />
}
