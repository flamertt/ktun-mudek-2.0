import { appConfig } from '../../../shared/config/appConfig'
import { AdminSection } from '../../../shared/ui/admin-section/AdminSection.jsx'
import sectionStyles from '../../../shared/ui/admin-section/AdminSection.module.css'

export function SurveyManagementPage() {
  const page = appConfig.pages.surveyManagement

  return (
    <AdminSection title={page.title} description={page.description} loading={false}>
      <p className={sectionStyles.muted}>
        Bu sürümde anket uçları <code className={sectionStyles.code}>API_Endpoints.md</code> içinde listelenmemiştir.
        İleride eklendiğinde bu sayfa bağlanabilir.
      </p>
    </AdminSection>
  )
}
