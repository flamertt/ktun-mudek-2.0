import { createColumnHelper } from '@tanstack/react-table'
import { useMemo, useState } from 'react'
import { useParams } from 'react-router-dom'

import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import { DataTable } from '@shared/ui/data-table/DataTable.jsx'
import { useCourseEvaluationMudekData } from '../../course-evaluation/model/useCourseEvaluationMudekData'
import styles from './MudekItemCloAchievementsPage.module.css'

const columnHelper = createColumnHelper()

export function MudekItemCloAchievementsPage() {
  const { offeringId } = useParams()
  const d = useCourseEvaluationMudekData(offeringId)
  const [filter, setFilter] = useState('')

  const rows = useMemo(() => {
    const list = d.mudekResults?.itemCloAchievements ?? d.mudekResults?.ItemCloAchievements ?? []
    if (!Array.isArray(list)) return []
    return list.map((x) => ({
      id: x.id ?? x.Id,
      itemType: x.itemType ?? x.ItemType,
      examId: x.examId ?? x.ExamId,
      itemNumber: x.itemNumber ?? x.ItemNumber,
      courseLearningOutcomeId: x.courseLearningOutcomeId ?? x.CourseLearningOutcomeId,
      mappingWeight: x.mappingWeight ?? x.MappingWeight,
      achievementRate: x.achievementRate ?? x.AchievementRate,
      weightedAchievement: x.weightedAchievement ?? x.WeightedAchievement,
      includedStudentCount: x.includedStudentCount ?? x.IncludedStudentCount,
    }))
  }, [d.mudekResults])

  const sorted = useMemo(() => {
    return [...rows].sort(
      (a, b) => Number(b.weightedAchievement ?? -Infinity) - Number(a.weightedAchievement ?? -Infinity),
    )
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
      columnHelper.accessor('itemType', { header: 'Tür' }),
      columnHelper.accessor('examId', {
        header: 'Sınav',
        cell: (info) => d.examById.get(String(info.getValue() ?? '')) ?? d.shortGuid(info.getValue()),
      }),
      columnHelper.accessor('itemNumber', { header: 'No/Sıra' }),
      columnHelper.accessor('courseLearningOutcomeId', {
        header: 'CLO',
        cell: (info) => d.cloById.get(String(info.getValue() ?? '')) ?? d.shortGuid(info.getValue()),
      }),
      columnHelper.accessor('mappingWeight', { header: 'Eşleme ağırlığı' }),
      columnHelper.accessor('achievementRate', { header: 'Başarı oranı' }),
      columnHelper.accessor('weightedAchievement', { header: 'WeightedAchievement' }),
      columnHelper.accessor('includedStudentCount', { header: 'Dahil öğrenci' }),
    ],
    [d],
  )

  return (
    <PageSection
      title={`${d.title} · MÜDEK · DOC→CLO`}
      description="DOC katkı matrisi (Öğe → CLO)"
      error={d.error}
      loading={d.loading}
    >
      <div className={styles.panel}>
        <h3 className={styles.panelTitle}>MÜDEK · DOC katkı matrisi (Öğe → CLO)</h3>
        <DataTable
          columns={columns}
          data={ranked}
          globalFilter={filter}
          onGlobalFilterChange={setFilter}
          searchPlaceholder="CLO / tür ara…"
          isLoading={false}
          disablePagination
          getRowClassName={(row) => row?.rankClass ?? ''}
        />
      </div>
    </PageSection>
  )
}

