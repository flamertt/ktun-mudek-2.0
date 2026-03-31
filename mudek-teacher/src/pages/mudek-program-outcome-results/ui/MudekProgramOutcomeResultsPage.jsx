import { createColumnHelper } from '@tanstack/react-table'
import { useMemo, useState } from 'react'
import { useParams } from 'react-router-dom'

import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import { DataTable } from '@shared/ui/data-table/DataTable.jsx'
import { useCourseEvaluationMudekData } from '../../course-evaluation/model/useCourseEvaluationMudekData'
import styles from './MudekProgramOutcomeResultsPage.module.css'

const columnHelper = createColumnHelper()

export function MudekProgramOutcomeResultsPage() {
  const { offeringId } = useParams()
  const d = useCourseEvaluationMudekData(offeringId)
  const [filter, setFilter] = useState('')

  const rows = useMemo(() => {
    const list = d.mudekResults?.programOutcomeResults ?? d.mudekResults?.ProgramOutcomeResults ?? []
    if (!Array.isArray(list)) return []
    return list.map((x) => ({
      id: x.id ?? x.Id,
      programOutcomeId: x.programOutcomeId ?? x.ProgramOutcomeId,
      achievementScore: x.achievementScore ?? x.AchievementScore,
      updatedAt: x.updatedAt ?? x.UpdatedAt,
    }))
  }, [d.mudekResults])

  const sorted = useMemo(() => {
    return [...rows].sort((a, b) => Number(b.achievementScore ?? -Infinity) - Number(a.achievementScore ?? -Infinity))
  }, [rows])

  const ranked = useMemo(() => {
    if (!sorted.length) return []
    const mid = Math.floor(sorted.length / 2)
    return sorted.map((r, i) => ({
      ...r,
      rankClass: i === 0 ? 'rowBest' : i === sorted.length - 1 ? 'rowWorst' : i === mid ? 'rowAvg' : '',
    }))
  }, [sorted])

  const columns = useMemo(
    () => [
      columnHelper.accessor('programOutcomeId', {
        header: 'PÇ',
        cell: (info) => `PÇ · ${d.shortGuid(info.getValue())}`,
      }),
      columnHelper.accessor('achievementScore', { header: 'AchievementScore' }),
      columnHelper.accessor('updatedAt', { header: 'Güncellendi', cell: (info) => d.formatDate(info.getValue()) }),
    ],
    [d],
  )

  return (
    <PageSection
      title={`${d.title} · MÜDEK · Program çıktısı`}
      description="Program çıktısı sonuçları"
      error={d.error}
      loading={d.loading}
    >
      <div className={styles.panel}>
        <h3 className={styles.panelTitle}>MÜDEK · Program çıktısı sonuçları</h3>
        <DataTable
          columns={columns}
          data={ranked}
          globalFilter={filter}
          onGlobalFilterChange={setFilter}
          searchPlaceholder="ProgramOutcomeId ara…"
          isLoading={false}
          disablePagination
          getRowClassName={(row) => row?.rankClass ?? ''}
        />
      </div>
    </PageSection>
  )
}

