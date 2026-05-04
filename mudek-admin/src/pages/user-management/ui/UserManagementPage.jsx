import { appConfig } from '../../../shared/config/appConfig'
import { UniversityManagedPlaceholderPage } from '../../../shared/ui/university-placeholder/UniversityManagedPlaceholderPage.jsx'

export function UserManagementPage() {
  const page = appConfig.pages.userManagement
  return <UniversityManagedPlaceholderPage title={page.title} description={page.description} />
}
