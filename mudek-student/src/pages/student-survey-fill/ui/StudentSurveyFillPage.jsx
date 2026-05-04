import { ArrowLeft, CheckCircle2, ClipboardList, Send } from 'lucide-react'
import { useCallback, useEffect, useMemo, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'

import { fetchStudentCourses, fetchStudentSurveyDetail, submitStudentSurvey } from '../../../shared/api/studentApi'
import { getStudentToken } from '../../../shared/lib/authToken'
import { getStudentCourseOfferingId } from '../../../shared/lib/studentCourseMap'
import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import sectionStyles from '@shared/ui/page-section/PageSection.module.css'
import styles from './StudentSurveyFillPage.module.css'

function gid(q) {
  return q?.id ?? q?.Id ?? ''
}

function buildOptions(q) {
  const min = Number(q.scaleMin ?? q.ScaleMin ?? 0)
  const max = Number(q.scaleMax ?? q.ScaleMax ?? 5)
  const req = q.isRequired ?? q.IsRequired ?? false
  const set = new Set()
  if (!req) set.add(0)
  const low = req ? Math.max(min, 1) : min
  for (let v = low; v <= max; v++) set.add(v)
  return [...set].sort((a, b) => a - b)
}

function defaultValue(q) {
  const opts = buildOptions(q)
  return opts.length ? opts[0] : 0
}

export function StudentSurveyFillPage() {
  const { offeringId, surveyId } = useParams()
  const navigate = useNavigate()

  const [detail, setDetail] = useState(null)
  const [courseLabel, setCourseLabel] = useState('')
  const [values, setValues] = useState({})
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)
  const [submitting, setSubmitting] = useState(false)

  const load = useCallback(() => {
    const token = getStudentToken()
    if (!token || !surveyId || !offeringId) return
    setLoading(true)
    setError('')
    Promise.all([fetchStudentSurveyDetail(token, surveyId), fetchStudentCourses(token)])
      .then(([d, courses]) => {
        setDetail(d ?? null)
        const arr = Array.isArray(courses) ? courses : []
        const match = arr.find((c) => getStudentCourseOfferingId(c) === String(offeringId))
        const code = match?.courseCode ?? match?.CourseCode ?? ''
        const name = match?.courseName ?? match?.CourseName ?? ''
        setCourseLabel(code && name ? `${code} — ${name}` : (name || code || ''))

        const qs = d?.questions ?? d?.Questions ?? []
        const next = {}
        for (const q of Array.isArray(qs) ? qs : []) {
          const id = gid(q)
          if (id) next[id] = defaultValue(q)
        }
        setValues(next)
      })
      .catch((e) => setError(e instanceof Error ? e.message : 'Anket yüklenemedi.'))
      .finally(() => setLoading(false))
  }, [surveyId, offeringId])

  useEffect(() => {
    void Promise.resolve().then(load)
  }, [load])

  const questions = useMemo(() => {
    const qs = detail?.questions ?? detail?.Questions ?? []
    return Array.isArray(qs) ? [...qs].sort((a, b) => (a?.orderIndex ?? a?.OrderIndex ?? 0) - (b?.orderIndex ?? b?.OrderIndex ?? 0)) : []
  }, [detail])

  const title = detail?.title ?? detail?.Title ?? 'Anket'
  const desc = detail?.description ?? detail?.Description ?? ''
  const hasSubmitted = detail?.hasSubmitted ?? detail?.HasSubmitted ?? false

  const scaleMin = useMemo(() => {
    if (!questions.length) return 0
    return Math.min(...questions.map((q) => Number(q.scaleMin ?? q.ScaleMin ?? 0)))
  }, [questions])

  const scaleMax = useMemo(() => {
    if (!questions.length) return 5
    return Math.max(...questions.map((q) => Number(q.scaleMax ?? q.ScaleMax ?? 5)))
  }, [questions])

  const pageDescription = courseLabel
    ? `${courseLabel} · Ölçek: ${scaleMin} (düşük) — ${scaleMax} (yüksek). İsteğe bağlı sorularda «Atla» ile yanıt vermeden geçebilirsiniz.`
    : `Ölçek: ${scaleMin} (düşük) — ${scaleMax} (yüksek). İsteğe bağlı sorularda «Atla» ile yanıt vermeden geçebilirsiniz.`

  const handleSubmit = async (e) => {
    e.preventDefault()
    const token = getStudentToken()
    if (!token || !surveyId) return
    setSubmitting(true)
    setError('')
    try {
      const answers = questions.map((q) => {
        const id = gid(q)
        const raw = values[id]
        const num = raw === '' || raw === undefined ? 0 : Number(raw)
        return { questionId: id, valueNumeric: num }
      })
      await submitStudentSurvey(token, surveyId, { answers })
      navigate(`/surveys/${offeringId}`, { replace: true })
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Gönderilemedi.')
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <PageSection
      title={title}
      description={!loading && !hasSubmitted && questions.length ? pageDescription : undefined}
      error={error}
      loading={loading}
      fullWidthCopy
    >
      <div className={styles.root}>
        <div className={styles.toolbar}>
          <button type="button" className={styles.backBtn} onClick={() => navigate(`/surveys/${offeringId}`)}>
            <ArrowLeft size={16} aria-hidden />
            Anket listesi
          </button>
          {!loading && questions.length && !hasSubmitted ? (
            <span className={styles.toolbarMeta}>
              <ClipboardList size={15} aria-hidden />
              {questions.length} soru
            </span>
          ) : null}
        </div>

        {!loading && hasSubmitted ? (
          <div className={styles.doneCard}>
            <div className={styles.doneIcon} aria-hidden>
              <CheckCircle2 size={32} strokeWidth={2.25} />
            </div>
            <h2 className={styles.doneTitle}>Bu anketi zaten doldurdunuz</h2>
            <p className={styles.doneText}>
              Yanıtlarınız kayıtlıdır. Gerekirse ders öğretmeninizle iletişime geçebilirsiniz.
            </p>
            <button type="button" className={styles.backBtn} onClick={() => navigate(`/surveys/${offeringId}`)}>
              <ArrowLeft size={16} aria-hidden />
              Listeye dön
            </button>
          </div>
        ) : null}

        {!loading && detail && !hasSubmitted && desc ? <p className={styles.surveyIntro}>{desc}</p> : null}

        {!loading && detail && !hasSubmitted && !questions.length ? (
          <p className={sectionStyles.muted}>Bu ankette henüz soru tanımlanmamış.</p>
        ) : null}

        {!loading && detail && !hasSubmitted && questions.length ? (
          <form className={styles.form} onSubmit={(e) => void handleSubmit(e)}>
            <ol className={styles.qList}>
              {questions.map((q, idx) => {
                const id = gid(q)
                const opts = buildOptions(q)
                const txt = q?.text ?? q?.Text ?? ''
                const required = q.isRequired ?? q.IsRequired
                const smin = Number(q.scaleMin ?? q.ScaleMin ?? 0)
                const smax = Number(q.scaleMax ?? q.ScaleMax ?? 5)
                return (
                  <li key={id} className={styles.qCard}>
                    <div className={styles.qHeader}>
                      <span className={styles.qIndex} aria-hidden>
                        {idx + 1}
                      </span>
                      <div className={styles.qBody}>
                        <p className={styles.qText}>{txt}</p>
                        <p className={styles.qMeta}>
                          <span className={required ? styles.reqMark : styles.optMark}>
                            {required ? 'Zorunlu' : 'İsteğe bağlı'}
                          </span>
                          <span className={styles.qMetaSep} aria-hidden>
                            ·
                          </span>
                          <span>
                            Ölçek {smin}–{smax}
                          </span>
                        </p>
                      </div>
                    </div>
                    <div className={styles.likertWrap}>
                      <span className={styles.visuallyHidden} id={`scale-${id}-label`}>
                        Soru {idx + 1} yanıtı
                      </span>
                      <div
                        className={styles.likert}
                        role="radiogroup"
                        aria-labelledby={`scale-${id}-label`}
                      >
                        {opts.map((v) => (
                          <label
                            key={v}
                            className={`${styles.likertOpt} ${v === 0 ? styles.likertSkip : ''}`}
                          >
                            <input
                              type="radio"
                              name={`survey-q-${id}`}
                              value={v}
                              checked={Number(values[id]) === v}
                              onChange={() => setValues((prev) => ({ ...prev, [id]: v }))}
                            />
                            <span className={styles.likertFace}>{v === 0 ? 'Atla' : v}</span>
                          </label>
                        ))}
                      </div>
                    </div>
                  </li>
                )
              })}
            </ol>

            <div className={styles.submitBar}>
              <button type="submit" className={styles.submit} disabled={submitting}>
                <Send size={18} aria-hidden />
                {submitting ? 'Gönderiliyor…' : 'Anketi gönder'}
              </button>
            </div>
          </form>
        ) : null}
      </div>
    </PageSection>
  )
}
