import { Navigate, Route, Routes } from 'react-router-dom'

import { HomePage } from './pages/home/ui/HomePage.jsx'
import { LoginPage } from './pages/login/ui/LoginPage.jsx'
import { appConfig } from './shared/config/appConfig'
import { AppShell } from './widgets/app-shell/ui/AppShell.jsx'
import { SectionPage } from './pages/section/ui/SectionPage.jsx'
import { MyCoursesPage } from './pages/my-courses/ui/MyCoursesPage.jsx'
import { CourseDetailPage } from './pages/course-detail/ui/CourseDetailPage.jsx'
import { CourseStudentsPage } from './pages/course-students/ui/CourseStudentsPage.jsx'
import { CourseEvaluationPage } from './pages/course-evaluation/ui/CourseEvaluationPage.jsx'
import { EvaluationsLandingPage } from './pages/evaluations/ui/EvaluationsLandingPage.jsx'
import { EvaluationExamsPage } from './pages/evaluation-exams/ui/EvaluationExamsPage.jsx'
import { ExamQuestionsPage } from './pages/exam-questions/ui/ExamQuestionsPage.jsx'
import { QuestionClosPage } from './pages/question-clos/ui/QuestionClosPage.jsx'
import { QuestionAnswersPage } from './pages/question-answers/ui/QuestionAnswersPage.jsx'
import { ExamComponentsPage } from './pages/exam-components/ui/ExamComponentsPage.jsx'
import { ComponentClosPage } from './pages/component-clos/ui/ComponentClosPage.jsx'
import { ComponentScoresPage } from './pages/component-scores/ui/ComponentScoresPage.jsx'
import { LetterGradeRulesPage } from './pages/letter-grade-rules/ui/LetterGradeRulesPage.jsx'
import { MudekStudentResultsPage } from './pages/mudek-student-results/ui/MudekStudentResultsPage.jsx'
import { MudekExamSummariesPage } from './pages/mudek-exam-summaries/ui/MudekExamSummariesPage.jsx'
import { MudekQuestionComponentResultsPage } from './pages/mudek-question-component-results/ui/MudekQuestionComponentResultsPage.jsx'
import { MudekItemCloAchievementsPage } from './pages/mudek-item-clo-achievements/ui/MudekItemCloAchievementsPage.jsx'
import { MudekCloResultsPage } from './pages/mudek-clo-results/ui/MudekCloResultsPage.jsx'
import { MudekProgramOutcomeResultsPage } from './pages/mudek-program-outcome-results/ui/MudekProgramOutcomeResultsPage.jsx'

export default function App() {
  const navItems = appConfig.navSections.flatMap((section) => section.items)
  const secondaryItems = navItems.filter((item) => item.key !== 'home')

  /** @type {Record<string, import('react').ComponentType> } */
  const PAGE_BY_KEY = {
    courses: MyCoursesPage,
    evaluations: EvaluationsLandingPage,
  }

  return (
    <Routes>
      <Route path="/" element={<Navigate to="/login" replace />} />
      <Route path="/login" element={<LoginPage />} />
      <Route element={<AppShell />}>
        <Route path={appConfig.routes.home} element={<HomePage />} />
        {secondaryItems.map((item) => {
          const Page = PAGE_BY_KEY[item.key]
          if (Page) return <Route key={item.key} path={item.path} element={<Page />} />
          return (
            <Route
              key={item.key}
              path={item.path}
              element={
                <SectionPage
                  title={appConfig.pages[item.key]?.title ?? item.label}
                  description={appConfig.pages[item.key]?.description ?? ''}
                />
              }
            />
          )
        })}

        <Route path="/courses/:offeringId" element={<CourseDetailPage />} />
        <Route path="/courses/:offeringId/students" element={<CourseStudentsPage />} />
        <Route path="/evaluations/:offeringId" element={<CourseEvaluationPage />} />
        <Route path="/evaluations/:offeringId/mudek/students" element={<MudekStudentResultsPage />} />
        <Route path="/evaluations/:offeringId/mudek/exams" element={<MudekExamSummariesPage />} />
        <Route
          path="/evaluations/:offeringId/mudek/question-components"
          element={<MudekQuestionComponentResultsPage />}
        />
        <Route path="/evaluations/:offeringId/mudek/item-clo" element={<MudekItemCloAchievementsPage />} />
        <Route path="/evaluations/:offeringId/mudek/clo" element={<MudekCloResultsPage />} />
        <Route
          path="/evaluations/:offeringId/mudek/program-outcomes"
          element={<MudekProgramOutcomeResultsPage />}
        />

        <Route
          path="/evaluations/:offeringId/evaluation/:evaluationId/exams"
          element={<EvaluationExamsPage />}
        />
        <Route
          path="/evaluations/:offeringId/evaluation/:evaluationId/exams/:examId/questions"
          element={<ExamQuestionsPage />}
        />
        <Route
          path="/evaluations/:offeringId/evaluation/:evaluationId/questions/:questionId/clos"
          element={<QuestionClosPage />}
        />
        <Route
          path="/evaluations/:offeringId/evaluation/:evaluationId/questions/:questionId/answers"
          element={<QuestionAnswersPage />}
        />
        <Route
          path="/evaluations/:offeringId/evaluation/:evaluationId/exams/:examId/components"
          element={<ExamComponentsPage />}
        />
        <Route
          path="/evaluations/:offeringId/evaluation/:evaluationId/components/:componentId/clos"
          element={<ComponentClosPage />}
        />
        <Route
          path="/evaluations/:offeringId/evaluation/:evaluationId/components/:componentId/scores"
          element={<ComponentScoresPage />}
        />
        <Route
          path="/evaluations/:offeringId/evaluation/:evaluationId/letter-grade-rules"
          element={<LetterGradeRulesPage />}
        />
      </Route>
      <Route path="*" element={<Navigate to={appConfig.routes.home} replace />} />
    </Routes>
  )
}

