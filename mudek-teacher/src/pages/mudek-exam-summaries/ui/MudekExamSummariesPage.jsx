import { createColumnHelper } from '@tanstack/react-table'
import { useMemo, useState } from 'react'
import { useParams } from 'react-router-dom'

import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import { DataTable } from '@shared/ui/data-table/DataTable.jsx'
import { useCourseEvaluationMudekData } from '../../course-evaluation/model/useCourseEvaluationMudekData'
import styles from './MudekExamSummariesPage.module.css'

const columnHelper = createColumnHelper()

export function MudekExamSummariesPage() {
  const { offeringId } = useParams()
  const d = useCourseEvaluationMudekData(offeringId)
  const [examFilter, setExamFilter] = useState('')

  const mudekExamSummaries = useMemo(() => {
    const list = d.mudekResults?.examSummaries ?? d.mudekResults?.ExamSummaries ?? []
    if (!Array.isArray(list)) return []
    return list.map((x) => ({
      id: x.id ?? x.Id,
      examId: x.examId ?? x.ExamId,
      participantCount: x.participantCount ?? x.ParticipantCount,
      includedStudentCount: x.includedStudentCount ?? x.IncludedStudentCount,
      perfectScoreCount: x.perfectScoreCount ?? x.PerfectScoreCount,
      maxTotalScore: x.maxTotalScore ?? x.MaxTotalScore,
      minTotalScore: x.minTotalScore ?? x.MinTotalScore,
      averageTotalScore: x.averageTotalScore ?? x.AverageTotalScore,
      updatedAt: x.updatedAt ?? x.UpdatedAt,
    }))
  }, [d.mudekResults])

  const sorted = useMemo(() => {
    return [...mudekExamSummaries].sort(
      (a, b) => Number(b.averageTotalScore ?? -Infinity) - Number(a.averageTotalScore ?? -Infinity),
    )
  }, [mudekExamSummaries])

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
      columnHelper.accessor('examId', {
        header: 'Sınav',
        cell: (info) => d.examById.get(String(info.getValue() ?? '')) ?? d.shortGuid(info.getValue()),
      }),
      columnHelper.accessor('participantCount', { header: 'Katılımcı' }),
      columnHelper.accessor('includedStudentCount', { header: 'Dahil öğrenci' }),
      columnHelper.accessor('perfectScoreCount', { header: 'Tam puan' }),
      columnHelper.accessor('maxTotalScore', { header: 'Max toplam' }),
      columnHelper.accessor('minTotalScore', { header: 'Min toplam' }),
      columnHelper.accessor('averageTotalScore', { header: 'Ortalama toplam' }),
      columnHelper.accessor('updatedAt', { header: 'Güncellendi', cell: (info) => d.formatDate(info.getValue()) }),
    ],
    [d],
  )

  return (
    <PageSection title={`${d.title} · MÜDEK · Sınav`} description="Sınav özetleri" error={d.error} loading={d.loading}>
      <div className={styles.panel}>
        <h3 className={styles.panelTitle}>MÜDEK · Sınav özetleri</h3>
        <DataTable
          columns={columns}
          data={ranked}
          globalFilter={examFilter}
          onGlobalFilterChange={setExamFilter}
          searchPlaceholder="Sınav ara…"
          isLoading={false}
          disablePagination
          getRowClassName={(row) => row?.rankClass ?? ''}
        />
      </div>
    </PageSection>
  )
}

