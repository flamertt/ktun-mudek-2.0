import { appConfig } from '../../../shared/config/appConfig'
import { UniversityManagedPlaceholderPage } from '../../../shared/ui/university-placeholder/UniversityManagedPlaceholderPage.jsx'

export function CourseManagementPage() {
  const page = appConfig.pages.courseManagement
  return <UniversityManagedPlaceholderPage title={page.title} description={page.description} />
}
