import { Users } from 'lucide-react'
import { useCallback, useEffect, useMemo, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'

import { fetchMyCourseDetail } from '../../../shared/api/teacherApi'
import { appConfig } from '../../../shared/config/appConfig'
import { getTeacherToken } from '../../../shared/lib/authToken'
import { RefreshIconButton } from '../../../shared/ui/refresh-icon-button/RefreshIconButton.jsx'
import formStyles from '../../../shared/ui/admin-form/AdminForm.module.css'
import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import sectionStyles from '@shared/ui/page-section/PageSection.module.css'

export function CourseDetailPage() {
  const { offeringId } = useParams()
  const navigate = useNavigate()

  const page = appConfig.pages.courses

  const [detail, setDetail] = useState(null)
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  const load = useCallback(() => {
    const token = getTeacherToken()
    if (!token) return
    if (!offeringId) return
    setLoading(true)
    setError('')
    fetchMyCourseDetail(token, offeringId)
      .then((data) => setDetail(data ?? null))
      .catch((e) => setError(e instanceof Error ? e.message : 'Ders detayı alınamadı.'))
      .finally(() => setLoading(false))
  }, [offeringId])

  useEffect(() => {
    void Promise.resolve().then(load)
  }, [load])

  const actionButtons = useMemo(() => {
    if (!detail?.id) return []
    const id = detail.id
    return [
      {
        key: 'students',
        label: 'Öğrenciler',
        icon: Users,
        onClick: () => navigate(`/courses/${id}/students`),
      },
    ]
  }, [detail?.id, navigate])

  return (
    <PageSection
      title={detail?.courseCode ? `${detail.courseCode} - ${detail.courseName}` : page.title}
      description={detail ? `${detail.termName} · Şube ${detail.section ?? 'A'}` : page.description}
      error={error}
      loading={loading}
    >
      <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
        {detail ? (
          <div style={{ display: 'flex', flexDirection: 'column', gap: '0.25rem' }}>
            <p className={sectionStyles.muted}>
              Program: {detail.programName ?? '—'} · Öğretmen: {detail.teacherName ?? '—'}
            </p>
            <p className={sectionStyles.muted}>
              Kayıtlı öğrenci: {detail.enrolledCount ?? '—'} · Değerlendirme: {detail.hasEvaluation ? 'Var' : 'Yok'}
            </p>
          </div>
        ) : null}

        <div className={formStyles.actions}>
          {actionButtons.map((b) => (
            <button
              key={b.key}
              type="button"
              className={`${formStyles.btn} ${formStyles.btnGhost}`}
              onClick={b.onClick}
            >
              <b.icon size={16} aria-hidden />
              {b.label}
            </button>
          ))}
        </div>

        <div style={{ display: 'flex', justifyContent: 'flex-end' }}>
          <RefreshIconButton onClick={load} loading={loading} title="Yenile" />
        </div>
      </div>
    </PageSection>
  )
}

