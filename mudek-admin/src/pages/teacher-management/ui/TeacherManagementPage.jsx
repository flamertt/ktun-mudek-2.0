import { appConfig } from '../../../shared/config/appConfig'
import { UniversityManagedPlaceholderPage } from '../../../shared/ui/university-placeholder/UniversityManagedPlaceholderPage.jsx'

export function TeacherManagementPage() {
  const page = appConfig.pages.teacherManagement
  return <UniversityManagedPlaceholderPage title={page.title} description={page.description} />
}
