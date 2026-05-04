import { appConfig } from '../../../shared/config/appConfig'
import { UniversityManagedPlaceholderPage } from '../../../shared/ui/university-placeholder/UniversityManagedPlaceholderPage.jsx'

export function StudentManagementPage() {
  const page = appConfig.pages.studentManagement
  return <UniversityManagedPlaceholderPage title={page.title} description={page.description} />
}
