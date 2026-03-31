import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'

import {
  fetchCourses,
  fetchPrograms,
  fetchStudents,
  fetchTeachers,
} from '../../../shared/api/adminApi'
import { appConfig } from '../../../shared/config/appConfig'
import { getAdminToken } from '../../../shared/lib/authToken'
import { AdminSection } from '../../../shared/ui/admin-section/AdminSection.jsx'
import sectionStyles from '../../../shared/ui/admin-section/AdminSection.module.css'
import styles from './UserManagementPage.module.css'

export function UserManagementPage() {
  const page = appConfig.pages.userManagement
  const [counts, setCounts] = useState({ programs: '—', courses: '—', teachers: '—', students: '—' })
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const token = getAdminToken()
    if (!token) return
    let cancelled = false
    setLoading(true)
    Promise.all([
      fetchPrograms(token),
      fetchCourses(token),
      fetchTeachers(token),
      fetchStudents(token),
    ])
      .then(([programs, courses, teachers, students]) => {
        if (cancelled) return
        setCounts({
          programs: String(Array.isArray(programs) ? programs.length : 0),
          courses: String(Array.isArray(courses) ? courses.length : 0),
          teachers: String(Array.isArray(teachers) ? teachers.length : 0),
          students: String(Array.isArray(students) ? students.length : 0),
        })
      })
      .catch((e) => {
        if (!cancelled) setError(e instanceof Error ? e.message : 'Veri alınamadı.')
      })
      .finally(() => {
        if (!cancelled) setLoading(false)
      })
    return () => {
      cancelled = true
    }
  }, [])

  return (
    <AdminSection title={page.title} description={page.description} error={error} loading={loading}>
      <p className={sectionStyles.muted}>
        Backend’de merkezi “tüm kullanıcılar” listesi yok;{' '}
        <code className={sectionStyles.code}>GET /api/Admin/teachers</code> ve{' '}
        <code className={sectionStyles.code}>GET /api/Admin/students</code> ile rol bazlı yönetim yapılır.
      </p>

      <div className={styles.grid}>
        <div className={styles.card}>
          <p className={styles.cardLabel}>Program</p>
          <p className={styles.cardValue}>{loading ? '…' : counts.programs}</p>
        </div>
        <div className={styles.card}>
          <p className={styles.cardLabel}>Ders (katalog)</p>
          <p className={styles.cardValue}>{loading ? '…' : counts.courses}</p>
        </div>
        <div className={styles.card}>
          <p className={styles.cardLabel}>Öğretmen</p>
          <p className={styles.cardValue}>{loading ? '…' : counts.teachers}</p>
          <Link className={styles.link} to={appConfig.routes.teacherManagement}>
            Öğretmen listesi →
          </Link>
        </div>
        <div className={styles.card}>
          <p className={styles.cardLabel}>Öğrenci</p>
          <p className={styles.cardValue}>{loading ? '…' : counts.students}</p>
          <Link className={styles.link} to={appConfig.routes.studentManagement}>
            Öğrenci listesi →
          </Link>
        </div>
      </div>
    </AdminSection>
  )
}
