import { useCallback, useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { ArrowRight, BookOpen, GraduationCap, LayoutGrid, UserRound, Users } from 'lucide-react'

import {
  fetchCourses,
  fetchPrograms,
  fetchStudents,
  fetchTeachers,
} from '../../../shared/api/adminApi'
import { appConfig } from '../../../shared/config/appConfig'
import { getAdminToken } from '../../../shared/lib/authToken'
import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import { RefreshIconButton } from '../../../shared/ui/refresh-icon-button/RefreshIconButton.jsx'
import styles from './UserManagementPage.module.css'

const summaryCards = [
  { key: 'programs', label: 'Program', icon: LayoutGrid },
  { key: 'courses', label: 'Ders (katalog)', icon: BookOpen },
  { key: 'teachers', label: 'Öğretmen', icon: GraduationCap },
  { key: 'students', label: 'Öğrenci', icon: UserRound },
]

export function UserManagementPage() {
  const page = appConfig.pages.userManagement
  const [counts, setCounts] = useState({ programs: '—', courses: '—', teachers: '—', students: '—' })
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(true)

  const loadSummary = useCallback(() => {
    const token = getAdminToken()
    if (!token) return
    setLoading(true)
    setError('')
    Promise.all([
      fetchPrograms(token),
      fetchCourses(token),
      fetchTeachers(token),
      fetchStudents(token),
    ])
      .then(([programs, courses, teachers, students]) => {
        setCounts({
          programs: String(Array.isArray(programs) ? programs.length : 0),
          courses: String(Array.isArray(courses) ? courses.length : 0),
          teachers: String(Array.isArray(teachers) ? teachers.length : 0),
          students: String(Array.isArray(students) ? students.length : 0),
        })
      })
      .catch((e) => {
        setError(e instanceof Error ? e.message : 'Veri alınamadı.')
      })
      .finally(() => {
        setLoading(false)
      })
  }, [])

  useEffect(() => {
    loadSummary()
  }, [loadSummary])

  return (
    <PageSection title={page.title} description={page.description} error={error} loading={loading}>
      <div className={styles.layout}>
        <div className={styles.toolbarRow}>
          <RefreshIconButton onClick={loadSummary} loading={loading} />
        </div>
        <div className={styles.summary}>
          {summaryCards.map(({ key, label, icon: Icon }) => (
            <div key={key} className={styles.summaryCard}>
              <div className={styles.summaryIcon} aria-hidden>
                <Icon strokeWidth={1.75} size={20} />
              </div>
              <div>
                <p className={styles.summaryLabel}>{label}</p>
                <p className={styles.summaryValue}>{loading ? '…' : counts[key]}</p>
              </div>
            </div>
          ))}
        </div>

        <h2 className={styles.sectionTitle}>Kayıt yönetimi</h2>
        <p className={styles.sectionLead}>
          Öğretmen ve öğrenci listeleri ayrı ekranlarda tutulur; güncel kayıtlara buradan geçebilirsiniz.
        </p>

        <div className={styles.actions}>
          <Link className={styles.actionCard} to={appConfig.routes.teacherManagement} state={{ openCreate: true }}>
            <div className={styles.actionIconWrap}>
              <GraduationCap className={styles.actionIcon} strokeWidth={1.75} size={26} aria-hidden />
            </div>
            <div className={styles.actionText}>
              <span className={styles.actionTitle}>Öğretmen yönetimi</span>
              <span className={styles.actionDesc}>Öğretmen kayıtları ve program atamaları</span>
            </div>
            <ArrowRight className={styles.actionArrow} strokeWidth={2} />
          </Link>

          <Link className={styles.actionCard} to={appConfig.routes.studentManagement} state={{ openCreate: true }}>
            <div className={styles.actionIconWrap}>
              <Users className={styles.actionIcon} strokeWidth={1.75} size={26} aria-hidden />
            </div>
            <div className={styles.actionText}>
              <span className={styles.actionTitle}>Öğrenci yönetimi</span>
              <span className={styles.actionDesc}>Öğrenci kayıtları ve program bilgileri</span>
            </div>
            <ArrowRight className={styles.actionArrow} strokeWidth={2} />
          </Link>
        </div>
      </div>
    </PageSection>
  )
}
