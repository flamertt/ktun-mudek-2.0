import { ArrowLeft, BarChart3, ClipboardList } from 'lucide-react'
import { useCallback, useEffect, useMemo, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'

import { fetchTeacherSurveyDetail, fetchTeacherSurveyResults } from '../../../shared/api/teacherApi'
import { appConfig } from '../../../shared/config/appConfig'
import { getTeacherToken } from '../../../shared/lib/authToken'
import formStyles from '../../../shared/ui/admin-form/AdminForm.module.css'
import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import sectionStyles from '@shared/ui/page-section/PageSection.module.css'
import styles from '../../survey-detail/ui/TeacherSurveyDetailPage.module.css'

function gid(x) {
  return x?.id ?? x?.Id ?? ''
}

function gstr(x, a, b) {
  return x?.[a] ?? x?.[b] ?? ''
}

export function TeacherSurveyResultsPage() {
  const { offeringId, surveyId } = useParams()
  const navigate = useNavigate()

  const [detail, setDetail] = useState(null)
  const [results, setResults] = useState(null)
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  const load = useCallback(() => {
    const token = getTeacherToken()
    if (!token || !surveyId) return
    setLoading(true)
    setError('')
    Promise.all([fetchTeacherSurveyDetail(token, surveyId), fetchTeacherSurveyResults(token, surveyId)])
      .then(([d, r]) => {
        setDetail(d ?? null)
        setResults(r ?? null)
      })
      .catch((e) => setError(e instanceof Error ? e.message : 'Sonuçlar alınamadı.'))
      .finally(() => setLoading(false))
  }, [surveyId])

  useEffect(() => {
    void Promise.resolve().then(load)
  }, [load])

  const pageTitle = useMemo(() => {
    const t = detail?.title ?? detail?.Title
    return t ? `${t} — sonuçlar` : 'Anket sonuçları'
  }, [detail])

  const base = appConfig.routes.surveyCreate
  const backList = offeringId ? `${base}/${offeringId}` : '/courses'
  const backEdit = offeringId && surveyId ? `${base}/${offeringId}/${surveyId}` : backList

  return (
    <PageSection title={pageTitle} description="Özet yanıt istatistikleri ve DÖÇ karşılaştırması." error={error} loading={loading}>
      <div className={styles.root}>
        <div className={styles.toolbar}>
          <button
            type="button"
            className={`${formStyles.btn} ${formStyles.btnGhost}`}
            onClick={() => navigate(backList)}
          >
            <ArrowLeft size={16} aria-hidden />
            Anket listesi
          </button>
          <button type="button" className={`${formStyles.btn} ${formStyles.btnGhost}`} onClick={() => navigate(backEdit)}>
            <ClipboardList size={16} aria-hidden />
            Anketi düzenle
          </button>
          <button
            type="button"
            className={`${formStyles.btn} ${formStyles.btnGhost}`}
            onClick={() => void load()}
            disabled={loading}
          >
            Yenile
          </button>
        </div>

        {!loading && results ? (
          <div className={styles.panel}>
            <div className={styles.panelHead}>
              <div className={styles.panelHeadMain}>
                <span className={styles.panelIcon} aria-hidden>
                  <BarChart3 size={20} strokeWidth={2.25} />
                </span>
                <div>
                  <h3 className={styles.panelTitle}>Özet sonuçlar</h3>
                  <p className={styles.panelSub}>Soru bazlı yanıt sayıları, ortalama ve yüzde dağılımı.</p>
                </div>
              </div>
            </div>
            <div className={styles.statsRow}>
              <div className={styles.stat}>
                <span className={styles.statValue}>
                  {results.enrolledStudentCount ?? results.EnrolledStudentCount ?? '—'}
                </span>
                <span className={styles.statLabel}>Kayıtlı</span>
              </div>
              <div className={styles.stat}>
                <span className={styles.statValue}>
                  {results.totalSubmissions ?? results.TotalSubmissions ?? '—'}
                </span>
                <span className={styles.statLabel}>Gönderim</span>
              </div>
              <div className={styles.stat}>
                <span className={styles.statValue}>
                  {results.evaluatedSubmissions ?? results.EvaluatedSubmissions ?? '—'}
                </span>
                <span className={styles.statLabel}>Değerlendirilen</span>
              </div>
            </div>
            <div className={styles.tableWrap}>
              <table className={styles.table}>
                <thead>
                  <tr>
                    <th>Soru</th>
                    <th>Yanıt</th>
                    <th>Ort.</th>
                    <th>%</th>
                  </tr>
                </thead>
                <tbody>
                  {(results.questions ?? results.Questions ?? []).map((row) => (
                    <tr key={String(row.questionId ?? row.QuestionId ?? gid(row))}>
                      <td>{gstr(row, 'text', 'Text')}</td>
                      <td>{row.responseCount ?? row.ResponseCount ?? 0}</td>
                      <td>
                        {row.averageScore != null || row.AverageScore != null
                          ? Number(row.averageScore ?? row.AverageScore).toFixed(2)
                          : '—'}
                      </td>
                      <td>
                        {row.scorePercentage != null || row.ScorePercentage != null
                          ? `${Number(row.scorePercentage ?? row.ScorePercentage).toFixed(1)}%`
                          : '—'}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
            {(results.cloResults ?? results.CloResults ?? []).length ? (
              <>
                <h4 className={styles.subTitle}>DÖÇ karşılaştırması</h4>
                <div className={styles.tableWrap}>
                  <table className={styles.table}>
                    <thead>
                      <tr>
                        <th>DÖÇ</th>
                        <th>Anket</th>
                        <th>MÜDEK</th>
                        <th>Yorum</th>
                      </tr>
                    </thead>
                    <tbody>
                      {(results.cloResults ?? results.CloResults ?? []).map((row) => (
                        <tr key={row.cloId ?? row.CloId}>
                          <td>{row.cloCode ?? row.CloCode}</td>
                          <td>
                            {row.surveyScore != null || row.SurveyScore != null
                              ? Number(row.surveyScore ?? row.SurveyScore).toFixed(1)
                              : '—'}
                          </td>
                          <td>
                            {row.mudekScore != null || row.MudekScore != null
                              ? Number(row.mudekScore ?? row.MudekScore).toFixed(1)
                              : '—'}
                          </td>
                          <td>{row.evaluation ?? row.Evaluation ?? '—'}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </>
            ) : null}
          </div>
        ) : null}

        {!loading && !results && !error ? (
          <p className={sectionStyles.muted}>Sonuç verisi yok.</p>
        ) : null}
      </div>
    </PageSection>
  )
}
