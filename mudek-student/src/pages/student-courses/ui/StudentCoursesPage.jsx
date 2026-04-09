import { ClipboardList } from 'lucide-react'
import { useCallback, useEffect, useMemo, useState } from 'react'
import { useNavigate } from 'react-router-dom'

import { fetchStudentCourses } from '../../../shared/api/studentApi'
import { appConfig } from '../../../shared/config/appConfig'
import { getStudentToken } from '../../../shared/lib/authToken'
import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import sectionStyles from '@shared/ui/page-section/PageSection.module.css'
import styles from './StudentCoursesPage.module.css'

/**
 * @param {{ surveysOnly?: boolean }} props
 */
export function StudentCoursesPage({ surveysOnly = false }) {
  const page = appConfig.pages.surveys
  const navigate = useNavigate()

  const [rows, setRows] = useState([])
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)
  const [query, setQuery] = useState('')

  const load = useCallback(() => {
    const token = getStudentToken()
    if (!token) return
    setLoading(true)
    setError('')
    fetchStudentCourses(token)
      .then((data) => setRows(Array.isArray(data) ? data : []))
      .catch((e) => setError(e instanceof Error ? e.message : 'Dersler alınamadı.'))
      .finally(() => setLoading(false))
  }, [])

  useEffect(() => {
    void Promise.resolve().then(load)
  }, [load])

  const normalizedRows = useMemo(() => {
    return (rows ?? [])
      .map((r) => {
        const id = r?.courseOfferingId ?? r?.CourseOfferingId ?? r?.id ?? r?.Id ?? ''
        const courseCode = r?.courseCode ?? r?.CourseCode ?? ''
        const courseName = r?.courseName ?? r?.CourseName ?? ''
        const termName = r?.termName ?? r?.TermName ?? ''
        const section = r?.section ?? r?.Section ?? ''
        const teacherName = r?.teacherName ?? r?.TeacherName ?? ''
        const activeSurveyCount = r?.activeSurveyCount ?? r?.ActiveSurveyCount ?? 0
        return {
          ...r,
          id,
          courseCode,
          courseName,
          termName,
          section,
          teacherName,
          activeSurveyCount,
        }
      })
      .filter((r) => Boolean(r.id))
  }, [rows])

  const filtered = useMemo(() => {
    let list = normalizedRows
    if (surveysOnly) {
      list = list.filter((r) => (r.activeSurveyCount ?? 0) > 0)
    }
    const q = query.trim().toLowerCase()
    if (!q) return list
    return list.filter((r) => {
      const hay = `${r.courseCode ?? ''} ${r.courseName ?? ''} ${r.termName ?? ''} ${r.section ?? ''} ${r.teacherName ?? ''}`.toLowerCase()
      return hay.includes(q)
    })
  }, [normalizedRows, query, surveysOnly])

  return (
    <PageSection title={page.title} description={page.description} error={error}>
      <div className={styles.root}>
        <div className={styles.toolbar}>
          <div className={styles.search}>
            <label className={styles.searchLabel} htmlFor="st-course-search">
              Ders ara
              <input
                id="st-course-search"
                className={styles.searchInput}
                type="search"
                value={query}
                onChange={(e) => setQuery(e.target.value)}
                placeholder="Ders kodu, adı, dönem…"
              />
            </label>
          </div>
          <span className={sectionStyles.muted} style={{ fontSize: '0.85rem' }}>
            {filtered.length} ders
          </span>
        </div>

        {loading ? <p className={sectionStyles.muted}>Yükleniyor…</p> : null}

        {!loading && filtered.length ? (
          <div className={styles.grid}>
            {filtered.map((r) => (
              <div key={r.id} className={styles.card}>
                <div className={styles.cardHeaderRow}>
                  <div className={styles.codeBadge}>{r.courseCode}</div>
                </div>
                <div className={styles.cardTitle}>{r.courseName}</div>
                <p className={styles.cardSub}>
                  {r.termName} · Şube {r.section}
                  {r.teacherName ? ` · ${r.teacherName}` : ''}
                </p>
                <div className={styles.cardBottom}>
                  <div className={styles.metric}>
                    <div className={styles.metricValue}>{r.activeSurveyCount ?? 0}</div>
                    <div className={styles.metricLabel}>Aktif anket</div>
                  </div>
                  <button
                    type="button"
                    className={styles.openBtn}
                    onClick={() => navigate(`/surveys/${r.id}`)}
                    title="Anketler"
                  >
                    <ClipboardList size={16} aria-hidden />
                    Anketler
                  </button>
                </div>
              </div>
            ))}
          </div>
        ) : null}

        {!loading && !filtered.length ? (
          <div className={styles.empty}>
            {surveysOnly ? 'Aktif anketi olan ders bulunamadı.' : 'Uygun ders bulunamadı.'}
          </div>
        ) : null}
      </div>
    </PageSection>
  )
}
