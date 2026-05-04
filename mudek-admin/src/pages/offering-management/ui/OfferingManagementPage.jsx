import { appConfig } from '../../../shared/config/appConfig'
import { UniversityManagedPlaceholderPage } from '../../../shared/ui/university-placeholder/UniversityManagedPlaceholderPage.jsx'

export function OfferingManagementPage() {
  const page = appConfig.pages.offeringManagement
  return <UniversityManagedPlaceholderPage title={page.title} description={page.description} />
}
