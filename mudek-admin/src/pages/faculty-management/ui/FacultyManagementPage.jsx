import { createColumnHelper } from '@tanstack/react-table'
import { useCallback, useEffect, useMemo, useState } from 'react'

import {
  fetchDbActiveAcademicTerm,
  fetchUniversityAcademicTerms,
  syncUniversityActiveAcademicTerm,
} from '../../../shared/api/adminApi'
import { appConfig } from '../../../shared/config/appConfig'
import { getAdminToken } from '../../../shared/lib/authToken'
import formStyles from '../../../shared/ui/admin-form/AdminForm.module.css'
import { RefreshIconButton } from '../../../shared/ui/refresh-icon-button/RefreshIconButton.jsx'
import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import sectionStyles from '@shared/ui/page-section/PageSection.module.css'
import { DataTable } from '../../../shared/ui/data-table/DataTable.jsx'

const columnHelper = createColumnHelper()

function termId(t) {
  return t?.academicTermId ?? t?.AcademicTermId ?? t?.id ?? t?.Id ?? ''
}

/** Üniversite JSON: { id, ad } — BitirmeApi aynı şemayı iletir. */
function termName(t) {
  return (
    t?.academicTermName ??
    t?.AcademicTermName ??
    t?.ad ??
    t?.Ad ??
    t?.name ??
    t?.Name ??
    '—'
  )
}

export function FacultyManagementPage() {
  const page = appConfig.pages.facultyManagement
  const [terms, setTerms] = useState([])
  const [dbActive, setDbActive] = useState(null)
  const [error, setError] = useState('')
  const [actionError, setActionError] = useState('')
  const [loading, setLoading] = useState(true)
  const [syncing, setSyncing] = useState(false)
  const [globalFilter, setGlobalFilter] = useState('')

  const load = useCallback(async () => {
    const token = getAdminToken()
    if (!token) return
    setLoading(true)
    setError('')
    setActionError('')
    try {
      const [list, active] = await Promise.all([
        fetchUniversityAcademicTerms(token),
        fetchDbActiveAcademicTerm(token).catch(() => null),
      ])
      setTerms(Array.isArray(list) ? list : [])
      setDbActive(active)
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Dönemler yüklenemedi.')
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => {
    void load()
  }, [load])

  const handleSync = useCallback(async () => {
    const token = getAdminToken()
    if (!token) return
    setSyncing(true)
    setActionError('')
    try {
      await syncUniversityActiveAcademicTerm(token)
      await load()
    } catch (e) {
      setActionError(e instanceof Error ? e.message : 'Senkronizasyon başarısız.')
    } finally {
      setSyncing(false)
    }
  }, [load])

  const columns = useMemo(
    () => [
      columnHelper.accessor((row) => termId(row), { header: 'Dönem Id' }),
      columnHelper.accessor((row) => termName(row), { header: 'Ad (üniversite)' }),
    ],
    [],
  )

  const dbActiveLabel =
    dbActive?.ad ??
    dbActive?.Ad ??
    dbActive?.name ??
    dbActive?.Name ??
    (dbActive?.id != null || dbActive?.Id != null ? `Kayıt #${dbActive?.id ?? dbActive?.Id}` : null)

  return (
    <PageSection
      title={page.title}
      description="Akademik dönem listesi üniversite API’sinden salt okunur gelir. Öğrenci paneli için aktif dönemi veritabanına yazmak üzere senkronize edin."
      error={error}
    >
      {actionError ? (
        <p className={sectionStyles.error} role="alert">
          {actionError}
        </p>
      ) : null}

      <p className={sectionStyles.muted}>
        DB aktif dönem:{' '}
        {dbActiveLabel ? (
          <strong>{dbActiveLabel}</strong>
        ) : (
          <span>Henüz yok — aşağıdaki senkron ile oluşturulur.</span>
        )}
      </p>

      <div style={{ display: 'flex', gap: '0.75rem', flexWrap: 'wrap', alignItems: 'center', marginBottom: '1rem' }}>
        <RefreshIconButton onClick={() => void load()} loading={loading} title="Yenile" />
        <button
          type="button"
          className={`${formStyles.btn} ${formStyles.btnPrimary}`}
          onClick={() => void handleSync()}
          disabled={syncing || loading}
        >
          {syncing ? 'Senkronize ediliyor…' : 'Üniversite aktif dönemini DB’ye senkronize et'}
        </button>
      </div>

      <DataTable
        columns={columns}
        data={terms}
        globalFilter={globalFilter}
        onGlobalFilterChange={setGlobalFilter}
        searchPlaceholder="Dönem adı veya Id ara…"
        isLoading={loading}
      />
    </PageSection>
  )
}
