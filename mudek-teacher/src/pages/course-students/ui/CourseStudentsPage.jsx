import { createColumnHelper } from '@tanstack/react-table'
import { useCallback, useEffect, useMemo, useState } from 'react'
import { useParams } from 'react-router-dom'

import { fetchCourseStudents } from '../../../shared/api/teacherApi'
import { appConfig } from '../../../shared/config/appConfig'
import { getTeacherToken } from '../../../shared/lib/authToken'
import { RefreshIconButton } from '../../../shared/ui/refresh-icon-button/RefreshIconButton.jsx'
import { DataTable } from '../../../shared/ui/data-table/DataTable.jsx'
import sectionStyles from '@shared/ui/page-section/PageSection.module.css'
import formStyles from '../../../shared/ui/admin-form/AdminForm.module.css'
import { PageSection } from '@shared/ui/page-section/PageSection.jsx'

const columnHelper = createColumnHelper()

export function CourseStudentsPage() {
  const { offeringId } = useParams()
  const page = appConfig.pages.courses

  const [rows, setRows] = useState([])
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)
  const [globalFilter, setGlobalFilter] = useState('')

  const load = useCallback(() => {
    const token = getTeacherToken()
    if (!token || !offeringId) return

    setLoading(true)
    setError('')
    fetchCourseStudents(token, offeringId)
      .then((data) => setRows(Array.isArray(data) ? data : []))
      .catch((e) => setError(e instanceof Error ? e.message : 'Öğrenciler alınamadı.'))
      .finally(() => setLoading(false))
  }, [offeringId])

  useEffect(() => {
    void Promise.resolve().then(load)
  }, [load])

  const columns = useMemo(
    () => [
      columnHelper.accessor('studentFullName', { header: 'Öğrenci' }),
      columnHelper.accessor('studentNumber', { header: 'Numara' }),
      columnHelper.accessor('status', { header: 'Durum' }),
      columnHelper.accessor('enrolledAt', {
        header: 'Kayıt',
        cell: (info) => {
          const v = info.getValue()
          if (!v) return '—'
          try {
            return new Date(v).toLocaleString('tr-TR')
          } catch {
            return String(v)
          }
        },
      }),
    ],
    [],
  )

  return (
    <PageSection
      title={`${page.title} · Öğrenciler`}
      description={'Kayıtlı öğrenci listesi'}
      error={error}
    >
      <DataTable
        columns={columns}
        data={rows}
        globalFilter={globalFilter}
        onGlobalFilterChange={setGlobalFilter}
        searchPlaceholder="Öğrenci ara…"
        toolbarExtra={
          <RefreshIconButton onClick={load} loading={loading} title="Öğrencileri yenile" />
        }
        isLoading={loading}
        pageSize={10}
      />

      {rows.length ? (
        <p className={sectionStyles.muted} style={{ marginTop: '0.75rem', fontSize: '0.8rem' }}>
          Toplam: {rows.length}
        </p>
      ) : null}

      <div className={formStyles.actions} />
    </PageSection>
  )
}

