import { createColumnHelper } from '@tanstack/react-table'
import { Eye } from 'lucide-react'
import { useCallback, useEffect, useMemo, useState } from 'react'

import { fetchCourseEvaluationById, fetchCourseEvaluations } from '../../../shared/api/adminApi'
import { appConfig } from '../../../shared/config/appConfig'
import { getAdminToken } from '../../../shared/lib/authToken'
import formStyles from '../../../shared/ui/admin-form/AdminForm.module.css'
import { RefreshIconButton } from '../../../shared/ui/refresh-icon-button/RefreshIconButton.jsx'
import { DataTable } from '../../../shared/ui/data-table/DataTable.jsx'
import { AppDialog } from '../../../shared/ui/dialog/AppDialog.jsx'
import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import sectionStyles from '@shared/ui/page-section/PageSection.module.css'
import pageStyles from './CourseEvaluationsPage.module.css'

const columnHelper = createColumnHelper()

function formatDate(iso) {
  if (!iso) return '—'
  try {
    return new Date(iso).toLocaleString('tr-TR')
  } catch {
    return String(iso)
  }
}

export function CourseEvaluationsPage() {
  const page = appConfig.pages.courseEvaluations
  const [rows, setRows] = useState([])
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)
  const [globalFilter, setGlobalFilter] = useState('')

  const [detail, setDetail] = useState(null)
  const [detailLoading, setDetailLoading] = useState(false)

  const load = useCallback(() => {
    const token = getAdminToken()
    if (!token) return
    setLoading(true)
    setError('')
    fetchCourseEvaluations(token)
      .then((data) => setRows(Array.isArray(data) ? data : []))
      .catch((e) => setError(e instanceof Error ? e.message : 'Liste alınamadı.'))
      .finally(() => setLoading(false))
  }, [])

  useEffect(() => {
    load()
  }, [load])

  const openDetail = useCallback(async (row) => {
    const token = getAdminToken()
    if (!token || !row?.id) return
    setDetailLoading(true)
    setDetail(null)
    try {
      const data = await fetchCourseEvaluationById(token, row.id)
      setDetail(data)
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Detay alınamadı.')
    } finally {
      setDetailLoading(false)
    }
  }, [])

  const columns = useMemo(
    () => [
      columnHelper.accessor('courseCode', { header: 'Ders kodu' }),
      columnHelper.accessor('courseName', {
        header: 'Ders',
        cell: (info) => {
          const v = info.getValue() ?? ''
          return v.length > 36 ? `${v.slice(0, 36)}…` : v
        },
      }),
      columnHelper.accessor('termName', { header: 'Dönem' }),
      columnHelper.accessor('teacherName', {
        header: 'Öğretmen',
        cell: (info) => info.getValue() ?? '—',
      }),
      columnHelper.accessor('lastCalculatedAt', {
        header: 'Son hesaplama',
        cell: (info) => formatDate(info.getValue()),
      }),
      columnHelper.accessor('isCalculationDirty', {
        header: 'Güncelleme gerekli',
        cell: (info) => (info.getValue() ? 'Evet' : 'Hayır'),
      }),
      columnHelper.display({
        id: 'actions',
        header: 'Detay',
        cell: ({ row }) => (
          <button
            type="button"
            className={formStyles.actionIcon}
            title="Salt okunur detay"
            onClick={() => openDetail(row.original)}
          >
            <Eye size={16} aria-hidden />
          </button>
        ),
      }),
    ],
    [openDetail],
  )

  return (
    <PageSection title={page.title} description={page.description} error={error}>
      <DataTable
        columns={columns}
        data={rows}
        globalFilter={globalFilter}
        onGlobalFilterChange={setGlobalFilter}
        searchPlaceholder="Ders veya dönemde ara…"
        toolbarExtra={<RefreshIconButton onClick={load} loading={loading} />}
        isLoading={loading}
      />

      <AppDialog
        open={Boolean(detail) || detailLoading}
        onClose={() => {
          setDetail(null)
          setDetailLoading(false)
        }}
        title="Ders değerlendirmesi (salt okunur)"
        size="lg"
        footer={
          <button
            type="button"
            className={`${formStyles.btn} ${formStyles.btnGhost}`}
            onClick={() => {
              setDetail(null)
              setDetailLoading(false)
            }}
          >
            Kapat
          </button>
        }
      >
        {detailLoading ? <p className={sectionStyles.muted}>Yükleniyor…</p> : null}
        {detail && !detailLoading ? (
          <div className={pageStyles.readOnlyDetail}>
            <p>
              <strong>{detail.courseCode}</strong> — {detail.courseName}
            </p>
            <p className={sectionStyles.muted}>
              {detail.termName} · Şube {detail.section ?? '—'} · {detail.teacherName ?? 'Öğretmen yok'}
            </p>
            <p className={sectionStyles.muted}>Öğrenci sayısı: {detail.studentCount ?? '—'}</p>
            <hr className={pageStyles.hr} />
            <section>
              <h4 className={pageStyles.detailHeading}>Öğrenci geri bildirimi</h4>
              <p className={pageStyles.readOnlyBlock}>{detail.studentFeedbackEvaluation?.trim() || '—'}</p>
            </section>
            <section>
              <h4 className={pageStyles.detailHeading}>Program çıktısı değerlendirmesi</h4>
              <p className={pageStyles.readOnlyBlock}>{detail.programOutcomeEvaluation?.trim() || '—'}</p>
            </section>
            <section>
              <h4 className={pageStyles.detailHeading}>Genel değerlendirme</h4>
              <p className={pageStyles.readOnlyBlock}>{detail.generalEvaluation?.trim() || '—'}</p>
            </section>
            <section>
              <h4 className={pageStyles.detailHeading}>İyileştirme önerileri</h4>
              <p className={pageStyles.readOnlyBlock}>{detail.improvementSuggestions?.trim() || '—'}</p>
            </section>
          </div>
        ) : null}
      </AppDialog>
    </PageSection>
  )
}
