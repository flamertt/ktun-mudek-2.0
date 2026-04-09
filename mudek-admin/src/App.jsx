import { Navigate, Route, Routes } from 'react-router-dom'

import { CloManagementPage } from './pages/clo-management/ui/CloManagementPage.jsx'
import { CloPoMappingPage } from './pages/clo-po-mapping/ui/CloPoMappingPage.jsx'
import { CourseEvaluationsPage } from './pages/course-evaluations/ui/CourseEvaluationsPage.jsx'
import { CourseManagementPage } from './pages/course-management/ui/CourseManagementPage.jsx'
import { CourseStudentsPage } from './pages/course-students/ui/CourseStudentsPage.jsx'
import { EnrollmentBulkPage } from './pages/enrollment-bulk/ui/EnrollmentBulkPage.jsx'
import { FacultyManagementPage } from './pages/faculty-management/ui/FacultyManagementPage.jsx'
import { HomePage } from './pages/home/ui/HomePage.jsx'
import { LoginPage } from './pages/login/ui/LoginPage.jsx'
import { OfferingManagementPage } from './pages/offering-management/ui/OfferingManagementPage.jsx'
import { ProgramLetterGradeRulesPage } from './pages/program-letter-grade-rules/ui/ProgramLetterGradeRulesPage.jsx'
import { ProgramOutcomesPage } from './pages/program-outcomes/ui/ProgramOutcomesPage.jsx'
import { StudentManagementPage } from './pages/student-management/ui/StudentManagementPage.jsx'
import { SurveyManagementPage } from './pages/survey-management/ui/SurveyManagementPage.jsx'
import { TeacherManagementPage } from './pages/teacher-management/ui/TeacherManagementPage.jsx'
import { UserManagementPage } from './pages/user-management/ui/UserManagementPage.jsx'
import { appConfig } from './shared/config/appConfig'
import { AppShell } from './widgets/app-shell/ui/AppShell.jsx'

const PAGE_BY_KEY = {
  home: HomePage,
  userManagement: UserManagementPage,
  teacherManagement: TeacherManagementPage,
  facultyManagement: FacultyManagementPage,
  programOutcomes: ProgramOutcomesPage,
  courseManagement: CourseManagementPage,
  cloManagement: CloManagementPage,
  cloPoMapping: CloPoMappingPage,
  letterGradeRules: ProgramLetterGradeRulesPage,
  offeringManagement: OfferingManagementPage,
  studentManagement: StudentManagementPage,
  courseStudents: CourseStudentsPage,
  enrollmentBulk: EnrollmentBulkPage,
  courseEvaluations: CourseEvaluationsPage,
  surveyManagement: SurveyManagementPage,
}

export default function App() {
  const navItems = appConfig.navSections.flatMap((section) => section.items)
  const secondaryItems = navItems.filter((item) => item.key !== 'home')

  return (
    <Routes>
      <Route path="/" element={<Navigate to="/login" replace />} />
      <Route path="/login" element={<LoginPage />} />
      <Route element={<AppShell />}>
        <Route path={appConfig.routes.home} element={<HomePage />} />
        {secondaryItems.map((item) => {
          const Page = PAGE_BY_KEY[item.key]
          if (!Page) return null
          return <Route key={item.key} path={item.path} element={<Page />} />
        })}
      </Route>
      <Route path="*" element={<Navigate to={appConfig.routes.home} replace />} />
    </Routes>
  )
}
