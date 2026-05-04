import { ArrowLeft, PenLine } from 'lucide-react'
import { useCallback, useEffect, useMemo, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'

import { fetchStudentCourses, fetchStudentSurveysForOffering } from '../../../shared/api/studentApi'
import { getStudentToken } from '../../../shared/lib/authToken'
import { getStudentCourseOfferingId } from '../../../shared/lib/studentCourseMap'
import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import sectionStyles from '@shared/ui/page-section/PageSection.module.css'
import styles from './StudentCourseSurveysPage.module.css'

function sid(x) {
  return x?.id ?? x?.Id ?? ''
}

export function StudentCourseSurveysPage() {
  const { offeringId } = useParams()
  const navigate = useNavigate()

  const [courseLabel, setCourseLabel] = useState('')
  const [surveys, setSurveys] = useState([])
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  const load = useCallback(() => {
    const token = getStudentToken()
    if (!token || !offeringId) return
    setLoading(true)
    setError('')
    Promise.all([fetchStudentCourses(token), fetchStudentSurveysForOffering(token, offeringId)])
      .then(([courses, list]) => {
        const arr = Array.isArray(courses) ? courses : []
        const match = arr.find((c) => getStudentCourseOfferingId(c) === String(offeringId))
        const code = match?.courseCode ?? match?.CourseCode ?? ''
        const name = match?.courseName ?? match?.CourseName ?? ''
        setCourseLabel(code && name ? `${code} — ${name}` : (name || code || 'Ders'))
        setSurveys(Array.isArray(list) ? list : [])
      })
      .catch((e) => setError(e instanceof Error ? e.message : 'Anketler alınamadı.'))
      .finally(() => setLoading(false))
  }, [offeringId])

  useEffect(() => {
    void Promise.resolve().then(load)
  }, [load])

  const rows = useMemo(() => {
    return (surveys ?? []).map((s) => ({
      id: sid(s),
      title: s?.title ?? s?.Title ?? '—',
      desc: s?.description ?? s?.Description ?? '',
      qCount: s?.questionCount ?? s?.QuestionCount ?? 0,
      done: s?.hasSubmitted ?? s?.HasSubmitted ?? false,
    }))
  }, [surveys])

  return (
    <PageSection
      title={courseLabel || 'Ders anketleri'}
      description="Aktif anketlerden birini seçerek Likert ölçeği ile yanıtlayın."
      error={error}
      loading={loading}
    >
      <div className={styles.root}>
        <div className={styles.toolbar}>
          <button type="button" className={styles.backBtn} onClick={() => navigate('/surveys')}>
            <ArrowLeft size={16} aria-hidden />
            Tüm dersler
          </button>
          {!loading && rows.length ? (
            <span className={sectionStyles.muted} style={{ fontSize: '0.85rem' }}>
              {rows.length} anket
            </span>
          ) : null}
        </div>

        {rows.length ? (
          <div className={styles.grid}>
            {rows.map((r) => (
              <article key={r.id} className={styles.card}>
                <div className={styles.cardHead}>
                  <span className={r.done ? styles.badgeDone : styles.badgeOpen}>
                    {r.done ? 'Tamamlandı' : 'Bekliyor'}
                  </span>
                </div>
                <h3 className={styles.cardTitle}>{r.title}</h3>
                {r.desc ? <p className={styles.cardDesc}>{r.desc}</p> : null}
                <div className={styles.meta}>
                  <span>{r.qCount} soru</span>
                </div>
                <div className={styles.cardBottom}>
                  <button
                    type="button"
                    className={styles.go}
                    disabled={r.done}
                    onClick={() => navigate(`/surveys/${offeringId}/${r.id}`)}
                  >
                    <PenLine size={17} aria-hidden />
                    {r.done ? 'Gönderildi' : 'Anketi doldur'}
                  </button>
                </div>
              </article>
            ))}
          </div>
        ) : null}

        {!loading && !rows.length ? (
          <div className={styles.empty}>Bu ders için şu an listelenecek aktif anket bulunmuyor.</div>
        ) : null}
      </div>
    </PageSection>
  )
}
