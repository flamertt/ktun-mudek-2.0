import { Navigate, Route, Routes, useParams } from 'react-router-dom'

import { LoginPage } from './pages/login/ui/LoginPage.jsx'
import { StudentCoursesPage } from './pages/student-courses/ui/StudentCoursesPage.jsx'
import { StudentCourseSurveysPage } from './pages/student-course-surveys/ui/StudentCourseSurveysPage.jsx'
import { StudentSurveyFillPage } from './pages/student-survey-fill/ui/StudentSurveyFillPage.jsx'
import { appConfig } from './shared/config/appConfig'
import { AppShell } from './widgets/app-shell/ui/AppShell.jsx'

function RedirectLegacyCourseSurveysList() {
  const { offeringId } = useParams()
  return <Navigate to={`/surveys/${offeringId}`} replace />
}

function RedirectLegacyCourseSurveyFill() {
  const { offeringId, surveyId } = useParams()
  return <Navigate to={`/surveys/${offeringId}/${surveyId}`} replace />
}

export default function App() {
  const surveysPath = appConfig.routes.surveys

  return (
    <Routes>
      <Route path="/" element={<Navigate to="/login" replace />} />
      <Route path="/login" element={<LoginPage />} />
      <Route element={<AppShell />}>
        <Route path="/surveys/:offeringId/:surveyId" element={<StudentSurveyFillPage />} />
        <Route path="/surveys/:offeringId" element={<StudentCourseSurveysPage />} />
        <Route path={surveysPath} element={<StudentCoursesPage surveysOnly />} />
        <Route path="/courses/:offeringId/surveys/:surveyId" element={<RedirectLegacyCourseSurveyFill />} />
        <Route path="/courses/:offeringId/surveys" element={<RedirectLegacyCourseSurveysList />} />
        <Route path="/home" element={<Navigate to={surveysPath} replace />} />
        <Route path="/courses" element={<Navigate to={surveysPath} replace />} />
      </Route>
      <Route path="*" element={<Navigate to={surveysPath} replace />} />
    </Routes>
  )
}
