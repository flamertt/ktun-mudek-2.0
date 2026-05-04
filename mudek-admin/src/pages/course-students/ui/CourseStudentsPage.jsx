import { appConfig } from '../../../shared/config/appConfig'
import { UniversityManagedPlaceholderPage } from '../../../shared/ui/university-placeholder/UniversityManagedPlaceholderPage.jsx'

export function CourseStudentsPage() {
  const page = appConfig.pages.courseStudents
  return <UniversityManagedPlaceholderPage title={page.title} description={page.description} />
}
