import { createColumnHelper } from '@tanstack/react-table'
import { useCallback, useEffect, useMemo, useState } from 'react'

import { fetchUniversityProgramOutcomes, fetchUniversityPrograms } from '../../../shared/api/adminApi'
import { appConfig } from '../../../shared/config/appConfig'
import { getAdminToken } from '../../../shared/lib/authToken'
import { RefreshIconButton } from '../../../shared/ui/refresh-icon-button/RefreshIconButton.jsx'
import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import sectionStyles from '@shared/ui/page-section/PageSection.module.css'
import { DataTable } from '../../../shared/ui/data-table/DataTable.jsx'
import poStyles from './ProgramOutcomesPage.module.css'

const columnHelper = createColumnHelper()

function getUniversityProgramId(p) {
  return String(p?.programId ?? p?.ProgramId ?? '')
}

function programName(p) {
  return p?.programName ?? p?.ProgramName ?? '—'
}

export function ProgramOutcomesPage() {
  const page = appConfig.pages.programOutcomes
  const [programs, setPrograms] = useState([])
  const [programId, setProgramId] = useState('')
  const [outcomeRows, setOutcomeRows] = useState([])
  const [error, setError] = useState('')
  const [loadingPrograms, setLoadingPrograms] = useState(true)
  const [loadingOutcomes, setLoadingOutcomes] = useState(false)
  const [filterPrograms, setFilterPrograms] = useState('')
  const [filterOutcomes, setFilterOutcomes] = useState('')

  const loadPrograms = useCallback(async () => {
    const token = getAdminToken()
    if (!token) return
    setLoadingPrograms(true)
    setError('')
    try {
      const data = await fetchUniversityPrograms(token)
      const list = Array.isArray(data) ? data : []
      setPrograms(list)
      setProgramId((prev) => {
        if (!list.length) return ''
        const ids = list.map((p) => getUniversityProgramId(p)).filter(Boolean)
        if (prev && ids.includes(prev)) return prev
        return ids[0] ?? ''
      })
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Programlar yüklenemedi.')
    } finally {
      setLoadingPrograms(false)
    }
  }, [])

  const loadOutcomes = useCallback(async () => {
    const token = getAdminToken()
    if (!token || !programId) {
      setOutcomeRows([])
      setLoadingOutcomes(false)
      return
    }
    setLoadingOutcomes(true)
    setError('')
    try {
      const pid = Number(programId)
      const data = await fetchUniversityProgramOutcomes(token, pid)
      setOutcomeRows(Array.isArray(data) ? data : [])
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Program çıktıları alınamadı.')
    } finally {
      setLoadingOutcomes(false)
    }
  }, [programId])

  useEffect(() => {
    void loadPrograms()
  }, [loadPrograms])

  useEffect(() => {
    void loadOutcomes()
  }, [loadOutcomes])

  const programColumns = useMemo(
    () => [
      columnHelper.accessor((p) => getUniversityProgramId(p), { header: 'Program Id' }),
      columnHelper.accessor((p) => programName(p), { header: 'Program adı (üniversite)' }),
    ],
    [],
  )

  const outcomeColumns = useMemo(
    () => [
      columnHelper.accessor((r) => r?.programOutcomeId ?? r?.ProgramOutcomeId ?? '—', { header: 'PÇ Id' }),
      columnHelper.accessor((r) => r?.programOutcomeCode ?? r?.ProgramOutcomeCode ?? '—', { header: 'Kod' }),
      columnHelper.accessor((r) => r?.description ?? r?.Description ?? '—', {
        header: 'Açıklama',
        cell: (info) => {
          const v = String(info.getValue() ?? '')
          return (
            <span className={poStyles.cellMuted} title={v}>
              {v || '—'}
            </span>
          )
        },
      }),
    ],
    [],
  )

  const outcomeToolbarFilters = (
    <label>
      Program
      <select
        className={sectionStyles.select}
        value={programId}
        onChange={(e) => setProgramId(e.target.value)}
        disabled={!programs.length}
      >
        {programs.map((p) => {
          const id = getUniversityProgramId(p)
          return (
            <option key={id} value={id}>
              {programName(p)}
            </option>
          )
        })}
      </select>
    </label>
  )

  return (
    <PageSection
      title={page.title}
      description="Program ve program çıktıları üniversite API’sinden salt okunur gelir; düzenleme üniversite sisteminde yapılır."
      error={error}
    >
      <div className={poStyles.block}>
        <h3 className={poStyles.subTitle}>Programlar</h3>
        <DataTable
          columns={programColumns}
          data={programs}
          globalFilter={filterPrograms}
          onGlobalFilterChange={setFilterPrograms}
          searchPlaceholder="Program ara…"
          toolbarExtra={<RefreshIconButton onClick={() => void loadPrograms()} loading={loadingPrograms} />}
          isLoading={loadingPrograms}
        />
      </div>

      <div className={poStyles.block}>
        <h3 className={poStyles.subTitle}>Program çıktıları (PÇ)</h3>
        {!programs.length ? (
          <p className={sectionStyles.muted}>Program listesi boş.</p>
        ) : (
          <DataTable
            columns={outcomeColumns}
            data={outcomeRows}
            globalFilter={filterOutcomes}
            onGlobalFilterChange={setFilterOutcomes}
            searchPlaceholder="Kod veya açıklamada ara…"
            toolbarFilters={outcomeToolbarFilters}
            toolbarExtra={<RefreshIconButton onClick={() => void loadOutcomes()} loading={loadingOutcomes} />}
            isLoading={loadingOutcomes}
          />
        )}
      </div>
    </PageSection>
  )
}
