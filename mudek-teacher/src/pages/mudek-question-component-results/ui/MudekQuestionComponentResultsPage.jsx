import { createColumnHelper } from '@tanstack/react-table'
import { useMemo, useState } from 'react'
import { useParams } from 'react-router-dom'

import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import { DataTable } from '@shared/ui/data-table/DataTable.jsx'
import { useCourseEvaluationMudekData } from '../../course-evaluation/model/useCourseEvaluationMudekData'
import styles from './MudekQuestionComponentResultsPage.module.css'

const columnHelper = createColumnHelper()

export function MudekQuestionComponentResultsPage() {
  const { offeringId } = useParams()
  const d = useCourseEvaluationMudekData(offeringId)
  const [filter, setFilter] = useState('')

  const rows = useMemo(() => {
    const list = d.mudekResults?.questionAndComponentResults ?? d.mudekResults?.QuestionAndComponentResults ?? []
    if (!Array.isArray(list)) return []
    return list.map((x) => ({
      id: x.id ?? x.Id,
      examId: x.examId ?? x.ExamId,
      examQuestionId: x.examQuestionId ?? x.ExamQuestionId,
      assessmentComponentId: x.assessmentComponentId ?? x.AssessmentComponentId,
      questionNumber: x.questionNumber ?? x.QuestionNumber,
      maxScore: x.maxScore ?? x.MaxScore,
      averageScore: x.averageScore ?? x.AverageScore,
      achievementRate: x.achievementRate ?? x.AchievementRate,
      includedStudentCount: x.includedStudentCount ?? x.IncludedStudentCount,
      updatedAt: x.updatedAt ?? x.UpdatedAt,
    }))
  }, [d.mudekResults])

  const sorted = useMemo(() => {
    return [...rows].sort((a, b) => Number(b.achievementRate ?? -Infinity) - Number(a.achievementRate ?? -Infinity))
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
      columnHelper.accessor('questionNumber', { header: 'No / Sıra' }),
      columnHelper.accessor('examQuestionId', { header: 'SoruId', cell: (info) => info.getValue() ?? '—' }),
      columnHelper.accessor('assessmentComponentId', {
        header: 'BileşenId',
        cell: (info) => info.getValue() ?? '—',
      }),
      columnHelper.accessor('maxScore', { header: 'Maks. puan' }),
      columnHelper.accessor('averageScore', { header: 'Ortalama' }),
      columnHelper.accessor('achievementRate', { header: 'Başarı oranı' }),
      columnHelper.accessor('includedStudentCount', { header: 'Dahil öğrenci' }),
      columnHelper.accessor('updatedAt', { header: 'Güncellendi', cell: (info) => d.formatDate(info.getValue()) }),
    ],
    [d],
  )

  return (
    <PageSection
      title={`${d.title} · MÜDEK · Soru/Bileşen`}
      description="Soru veya ölçme bileşeni bazında başarı oranı"
      error={d.error}
      loading={d.loading}
    >
      <div className={styles.panel}>
        <h3 className={styles.panelTitle}>MÜDEK · Soru / bileşen başarıları</h3>
        <DataTable
          columns={columns}
          data={ranked}
          globalFilter={filter}
          onGlobalFilterChange={setFilter}
          searchPlaceholder="No / id ara…"
          isLoading={false}
          disablePagination
          getRowClassName={(row) => row?.rankClass ?? ''}
        />
      </div>
    </PageSection>
  )
}

