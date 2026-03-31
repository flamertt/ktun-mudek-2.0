import { UsersRound } from 'lucide-react'
import { useCallback, useEffect, useMemo, useState } from 'react'
import { useNavigate } from 'react-router-dom'

import { fetchMyCourses } from '../../../shared/api/teacherApi'
import { appConfig } from '../../../shared/config/appConfig'
import { getTeacherToken } from '../../../shared/lib/authToken'
import { RefreshIconButton } from '../../../shared/ui/refresh-icon-button/RefreshIconButton.jsx'
import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import sectionStyles from '@shared/ui/page-section/PageSection.module.css'
import styles from './MyCoursesPage.module.css'

export function MyCoursesPage() {
  const page = appConfig.pages.courses
  const navigate = useNavigate()

  const [rows, setRows] = useState([])
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)
  const [query, setQuery] = useState('')

  const load = useCallback(() => {
    const token = getTeacherToken()
    if (!token) return
    setLoading(true)
    setError('')
    fetchMyCourses(token)
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
        const id = r?.id ?? r?.Id ?? r?.offeringId ?? r?.OfferingId ?? ''
        const courseCode = r?.courseCode ?? r?.CourseCode ?? ''
        const courseName = r?.courseName ?? r?.CourseName ?? ''
        const termName = r?.termName ?? r?.TermName ?? ''
        const section = r?.section ?? r?.Section ?? ''
        const teacherName = r?.teacherName ?? r?.TeacherName ?? ''
        const enrolledCount = r?.enrolledCount ?? r?.EnrolledCount ?? 0
        const isActive = r?.isActive ?? r?.IsActive ?? false

        return {
          ...r,
          id,
          courseCode,
          courseName,
          termName,
          section,
          teacherName,
          enrolledCount,
          isActive,
        }
      })
      .filter((r) => Boolean(r.id))
  }, [rows])

  const filtered = useMemo(() => {
    const q = query.trim().toLowerCase()
    if (!q) return normalizedRows
    return normalizedRows.filter((r) => {
      const hay = `${r.courseCode ?? ''} ${r.courseName ?? ''} ${r.termName ?? ''} ${r.section ?? ''} ${r.teacherName ?? ''}`.toLowerCase()
      return hay.includes(q)
    })
  }, [normalizedRows, query])

  return (
    <PageSection title={page.title} description={page.description} error={error}>
      <div className={styles.root}>
        <div className={styles.toolbar}>
          <div className={styles.search}>
            <label className={styles.searchLabel} htmlFor="my-course-search">
              Ders ara
              <input
                id="my-course-search"
                className={styles.searchInput}
                type="search"
                value={query}
                onChange={(e) => setQuery(e.target.value)}
                placeholder="Ders kodu, ders adı, dönem…"
              />
            </label>
          </div>

          <div style={{ display: 'flex', gap: '0.75rem', alignItems: 'center', flexWrap: 'wrap' }}>
            <RefreshIconButton onClick={load} loading={loading} title="Yenile" />
            <span className={sectionStyles.muted} style={{ fontSize: '0.85rem' }}>
              {filtered.length} ders
            </span>
          </div>
        </div>

        {loading ? <p className={sectionStyles.muted}>Dersler yükleniyor…</p> : null}

        {!loading && filtered.length ? (
          <div className={styles.grid}>
            {filtered.map((r) => (
              <div key={r.id} className={styles.card}>
                <div className={styles.cardHeaderRow}>
                  <div className={styles.codeBadge}>{r.courseCode}</div>
                </div>
                <div className={styles.cardTitle}>{r.courseName}</div>
                <p className={styles.cardSub}>
                  {r.termName} · Şube {r.section} {r.teacherName ? `· ${r.teacherName}` : ''}
                </p>

                <div className={styles.cardBottom}>
                  <div className={styles.metric}>
                    <div className={styles.metricValue}>{r.enrolledCount ?? 0}</div>
                    <div className={styles.metricLabel}>Kayıtlı öğrenci</div>
                  </div>

                  <button
                    type="button"
                    className={styles.openBtn}
                    onClick={() => navigate(`/courses/${r.id}`)}
                    title="Ders detay"
                  >
                    <UsersRound size={16} aria-hidden />
                    Dersi aç
                  </button>
                </div>
              </div>
            ))}
          </div>
        ) : null}

        {!loading && !filtered.length ? <div className={styles.empty}>Uygun ders bulunamadı.</div> : null}
      </div>
    </PageSection>
  )
}

