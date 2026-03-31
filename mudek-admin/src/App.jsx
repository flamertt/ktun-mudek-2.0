import { Navigate, Route, Routes } from 'react-router-dom'

import { CourseManagementPage } from './pages/course-management/ui/CourseManagementPage.jsx'
import { CourseStudentsPage } from './pages/course-students/ui/CourseStudentsPage.jsx'
import { FacultyManagementPage } from './pages/faculty-management/ui/FacultyManagementPage.jsx'
import { HomePage } from './pages/home/ui/HomePage.jsx'
import { LoginPage } from './pages/login/ui/LoginPage.jsx'
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
  studentManagement: StudentManagementPage,
  courseStudents: CourseStudentsPage,
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
