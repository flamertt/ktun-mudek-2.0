import { Navigate, Route, Routes } from 'react-router-dom'

import { HomePage } from './pages/home/ui/HomePage.jsx'
import { LoginPage } from './pages/login/ui/LoginPage.jsx'
import { SectionPage } from './pages/section/ui/SectionPage.jsx'
import { appConfig } from './shared/config/appConfig'
import { AppShell } from './widgets/app-shell/ui/AppShell.jsx'

export default function App() {
  const navItems = appConfig.navSections.flatMap((section) => section.items)
  const secondaryItems = navItems.filter((item) => item.key !== 'home')

  return (
    <Routes>
      <Route path="/" element={<Navigate to="/login" replace />} />
      <Route path="/login" element={<LoginPage />} />
      <Route element={<AppShell />}>
        <Route path={appConfig.routes.home} element={<HomePage />} />
        {secondaryItems.map((item) => (
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
        ))}
      </Route>
      <Route path="*" element={<Navigate to={appConfig.routes.home} replace />} />
    </Routes>
  )
}

