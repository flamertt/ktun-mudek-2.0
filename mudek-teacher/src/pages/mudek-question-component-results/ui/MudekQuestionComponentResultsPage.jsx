import { createColumnHelper } from '@tanstack/react-table'
import { useMemo, useState } from 'react'
import { useParams } from 'react-router-dom'

import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import { DataTable } from '@shared/ui/data-table/DataTable.jsx'
import {
  formatMudekDecimal,
  formatMudekRateAsPercent,
} from '../../course-evaluation/lib/mudekDisplayLabels'
import { useCourseEvaluationMudekData } from '../../course-evaluation/model/useCourseEvaluationMudekData'
import { EvaluationMudekBackToolbar } from '../../course-evaluation/ui/EvaluationMudekBackToolbar.jsx'
import styles from './MudekQuestionComponentResultsPage.module.css'

const columnHelper = createColumnHelper()

export function MudekQuestionComponentResultsPage() {
  const { offeringId } = useParams()
  const d = useCourseEvaluationMudekData(offeringId)
  const [filter, setFilter] = useState('')

  const rows = useMemo(() => {
    const list = d.mudekResults?.questionAndComponentResults ?? d.mudekResults?.QuestionAndComponentResults ?? []
    if (!Array.isArray(list)) return []
    return list.map((x) => {
      const examQuestionId = x.examQuestionId ?? x.ExamQuestionId
      const assessmentComponentId = x.assessmentComponentId ?? x.AssessmentComponentId
      const questionNumber = x.questionNumber ?? x.QuestionNumber
      const raw = String(x.itemCaption ?? x.ItemCaption ?? '').trim()
      const itemCaption =
        raw ||
        (examQuestionId ? `Soru ${questionNumber} (metin tanımsız)` : null) ||
        (assessmentComponentId ? `Bileşen ${questionNumber} (ad tanımsız)` : null) ||
        '—'
      return {
        id: x.id ?? x.Id,
        examId: x.examId ?? x.ExamId,
        examQuestionId,
        assessmentComponentId,
        itemCaption,
        questionNumber,
        maxScore: x.maxScore ?? x.MaxScore,
        averageScore: x.averageScore ?? x.AverageScore,
        achievementRate: x.achievementRate ?? x.AchievementRate,
        includedStudentCount: x.includedStudentCount ?? x.IncludedStudentCount,
        updatedAt: x.updatedAt ?? x.UpdatedAt,
      }
    })
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
      columnHelper.accessor('itemCaption', {
        header: 'Soru / bileşen',
        cell: (info) => info.getValue(),
      }),
      columnHelper.accessor('maxScore', {
        header: 'Maks. puan',
        cell: (info) => formatMudekDecimal(info.getValue(), 2),
      }),
      columnHelper.accessor('averageScore', {
        header: 'Ortalama',
        cell: (info) => formatMudekDecimal(info.getValue(), 2),
      }),
      columnHelper.accessor('achievementRate', {
        header: 'Başarı oranı',
        cell: (info) => formatMudekRateAsPercent(info.getValue(), 2),
      }),
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
      <EvaluationMudekBackToolbar />
      <div className={styles.panel}>
        <h3 className={styles.panelTitle}>MÜDEK · Soru / bileşen başarıları</h3>
        <DataTable
          columns={columns}
          data={ranked}
          globalFilter={filter}
          onGlobalFilterChange={setFilter}
          searchPlaceholder="Soru, bileşen veya sıra no ara…"
          isLoading={false}
          disablePagination
          getRowClassName={(row) => row?.rankClass ?? ''}
        />
      </div>
    </PageSection>
  )
}

