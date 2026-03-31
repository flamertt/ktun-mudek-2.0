import { createColumnHelper } from '@tanstack/react-table'
import { useMemo, useState } from 'react'
import { useParams } from 'react-router-dom'

import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import { DataTable } from '@shared/ui/data-table/DataTable.jsx'
import { useCourseEvaluationMudekData } from '../../course-evaluation/model/useCourseEvaluationMudekData'
import styles from './MudekCloResultsPage.module.css'

const columnHelper = createColumnHelper()

export function MudekCloResultsPage() {
  const { offeringId } = useParams()
  const d = useCourseEvaluationMudekData(offeringId)
  const [filter, setFilter] = useState('')

  const rows = useMemo(() => {
    const list = d.mudekResults?.cloResults ?? d.mudekResults?.CloResults ?? []
    if (!Array.isArray(list)) return []
    return list.map((x) => ({
      id: x.id ?? x.Id,
      courseLearningOutcomeId: x.courseLearningOutcomeId ?? x.CourseLearningOutcomeId,
      resultType: x.resultType ?? x.ResultType,
      examId: x.examId ?? x.ExamId,
      achievementScore: x.achievementScore ?? x.AchievementScore,
      combinedAchievementScore: x.combinedAchievementScore ?? x.CombinedAchievementScore,
      surveyScore: x.surveyScore ?? x.SurveyScore,
      surveyDifference: x.surveyDifference ?? x.SurveyDifference,
      updatedAt: x.updatedAt ?? x.UpdatedAt,
    }))
  }, [d.mudekResults])

  const sorted = useMemo(() => {
    return [...rows].sort((a, b) => {
      const as = Number(a.combinedAchievementScore ?? a.achievementScore ?? -Infinity)
      const bs = Number(b.combinedAchievementScore ?? b.achievementScore ?? -Infinity)
      return bs - as
    })
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
      columnHelper.accessor('courseLearningOutcomeId', {
        header: 'CLO',
        cell: (info) => d.cloById.get(String(info.getValue() ?? '')) ?? d.shortGuid(info.getValue()),
      }),
      columnHelper.accessor('resultType', { header: 'ResultType' }),
      columnHelper.accessor('achievementScore', { header: 'AchievementScore' }),
      columnHelper.accessor('combinedAchievementScore', { header: 'CombinedAchievementScore' }),
      columnHelper.accessor('surveyScore', { header: 'SurveyScore' }),
      columnHelper.accessor('surveyDifference', { header: 'SurveyDiff' }),
      columnHelper.accessor('updatedAt', { header: 'Güncellendi', cell: (info) => d.formatDate(info.getValue()) }),
    ],
    [d],
  )

  return (
    <PageSection title={`${d.title} · MÜDEK · CLO`} description="CLO sonuçları" error={d.error} loading={d.loading}>
      <div className={styles.panel}>
        <h3 className={styles.panelTitle}>MÜDEK · CLO sonuçları</h3>
        <DataTable
          columns={columns}
          data={ranked}
          globalFilter={filter}
          onGlobalFilterChange={setFilter}
          searchPlaceholder="CLO / ResultType ara…"
          isLoading={false}
          disablePagination
          getRowClassName={(row) => row?.rankClass ?? ''}
        />
      </div>
    </PageSection>
  )
}

