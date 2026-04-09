import { ClipboardList } from 'lucide-react'
import { useCallback, useEffect, useMemo, useState } from 'react'
import { useNavigate } from 'react-router-dom'

import { fetchMyCourses } from '../../../shared/api/teacherApi'
import { appConfig } from '../../../shared/config/appConfig'
import { getTeacherToken } from '../../../shared/lib/authToken'
import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import sectionStyles from '@shared/ui/page-section/PageSection.module.css'
import courseStyles from '../../my-courses/ui/MyCoursesPage.module.css'
import surveyStyles from './SurveyLandingPage.module.css'

/**
 * Kenar çubuğundaki "Anket Oluştur" girişi: ders seçerek ilgili dersin anket yönetimine gider.
 */
export function SurveyLandingPage() {
  const page = appConfig.pages.surveyCreate
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
        return { ...r, id, courseCode, courseName, termName, section }
      })
      .filter((r) => Boolean(r.id))
  }, [rows])

  const filtered = useMemo(() => {
    const q = query.trim().toLowerCase()
    if (!q) return normalizedRows
    return normalizedRows.filter((r) => {
      const hay = `${r.courseCode ?? ''} ${r.courseName ?? ''} ${r.termName ?? ''} ${r.section ?? ''}`.toLowerCase()
      return hay.includes(q)
    })
  }, [normalizedRows, query])

  return (
    <PageSection
      title={page.title}
      description="Önce bir ders seçin; o dersin anketlerini oluşturup düzenleyebilirsiniz."
      error={error}
    >
      <div className={courseStyles.root}>
        <div className={surveyStyles.hero}>
          <span className={surveyStyles.heroIcon} aria-hidden>
            <ClipboardList size={22} strokeWidth={2.25} />
          </span>
          <div>
            <p className={surveyStyles.heroTitle}>Anket yönetimi</p>
            <p className={surveyStyles.heroText}>
              Önce aşağıdan bir ders seçin. O dersin anketlerini oluşturabilir, Likert sorularını düzenleyebilir ve
              sonuçları görüntüleyebilirsiniz.
            </p>
          </div>
        </div>

        <div className={courseStyles.toolbar}>
          <div className={courseStyles.search}>
            <label className={courseStyles.searchLabel} htmlFor="survey-course-search">
              Ders ara
              <input
                id="survey-course-search"
                className={courseStyles.searchInput}
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

        {loading ? <p className={sectionStyles.muted}>Dersler yükleniyor…</p> : null}

        {!loading && filtered.length ? (
          <div className={courseStyles.grid}>
            {filtered.map((r) => (
              <div key={r.id} className={courseStyles.card}>
                <div className={courseStyles.cardHeaderRow}>
                  <div className={courseStyles.codeBadge}>{r.courseCode}</div>
                </div>
                <div className={courseStyles.cardTitle}>{r.courseName}</div>
                <p className={courseStyles.cardSub}>
                  {r.termName} · Şube {r.section}
                </p>
                <div className={courseStyles.cardBottom}>
                  <button
                    type="button"
                    className={courseStyles.openBtn}
                    onClick={() => navigate(`${appConfig.routes.surveyCreate}/${r.id}`)}
                    title="Anketleri yönet"
                  >
                    <ClipboardList size={16} aria-hidden />
                    Anketleri yönet
                  </button>
                </div>
              </div>
            ))}
          </div>
        ) : null}

        {!loading && !filtered.length ? (
          <div className={courseStyles.empty}>Aramanızla eşleşen ders bulunamadı.</div>
        ) : null}
      </div>
    </PageSection>
  )
}
