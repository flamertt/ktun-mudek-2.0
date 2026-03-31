import { GraduationCap } from 'lucide-react'
import { useMemo } from 'react'
import { useNavigate, useParams } from 'react-router-dom'

import sectionStyles from '@shared/ui/page-section/PageSection.module.css'
import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import styles from './CourseEvaluationPage.module.css'
import { useCourseEvaluationMudekData } from '../model/useCourseEvaluationMudekData'

export function CourseEvaluationPage() {
  const { offeringId } = useParams()
  const navigate = useNavigate()
  const d = useCourseEvaluationMudekData(offeringId)

  const nav = useMemo(
    () => [
      {
        title: 'Öğrenci sonuçları',
        desc: 'Başarı setleri + en iyi/en kötü/ortalama öğrenci + tablo.',
        to: `/evaluations/${offeringId}/mudek/students`,
      },
      {
        title: 'Sınav özetleri',
        desc: 'Katılımcı ve toplam puan istatistikleri.',
        to: `/evaluations/${offeringId}/mudek/exams`,
      },
      {
        title: 'Soru / bileşen başarıları',
        desc: 'Soru veya bileşen bazında başarı oranı.',
        to: `/evaluations/${offeringId}/mudek/question-components`,
      },
      {
        title: 'DOC → CLO katkı matrisi',
        desc: 'WeightedAchievement (eşleme × başarı).',
        to: `/evaluations/${offeringId}/mudek/item-clo`,
      },
      {
        title: 'CLO sonuçları',
        desc: 'CLO seviyesinde test + anket bileşenleri.',
        to: `/evaluations/${offeringId}/mudek/clo`,
      },
      {
        title: 'Program çıktısı sonuçları',
        desc: 'PÇ seviyesinde nihai başarı skoru.',
        to: `/evaluations/${offeringId}/mudek/program-outcomes`,
      },
    ],
    [offeringId],
  )

  return (
    <PageSection title={d.title} description="Değerlendirme (dashboard)" error={d.error} loading={d.loading}>
      <div className={styles.layout}>
        <p className={styles.headerLine}>
          {d.courseDetail?.termName ?? d.courseDetail?.TermName ? `${d.courseDetail?.termName ?? d.courseDetail?.TermName} · ` : ''}
          {d.courseDetail?.section ?? d.courseDetail?.Section ? `Şube ${d.courseDetail?.section ?? d.courseDetail?.Section} · ` : ''}
          {d.students?.length ? `${d.students.length} kayıtlı öğrenci` : '—'}
        </p>

        <div className={styles.pageGrid}>
          <div className={styles.mainCol}>
            <div className={styles.panel}>
              <h3 className={styles.panelTitle}>MÜDEK tabloları</h3>
              <p className={styles.panelHint}>
                Tablolar ayrı sayfalarda. İlgili karta basarak detay sayfaya gidebilirsin.
              </p>

              <div className={styles.quickGrid}>
                {nav.map((t) => (
                  <div key={t.to} className={styles.quickCard}>
                    <h4 className={styles.quickTitle}>{t.title}</h4>
                    <p className={styles.quickDesc}>{t.desc}</p>
                    <button type="button" className={styles.goBtn} onClick={() => navigate(t.to)}>
                      Tabloya git
                    </button>
                  </div>
                ))}
              </div>
            </div>

            {d.mudekLoading && !d.mudekResults ? (
              <p className={sectionStyles.muted} style={{ marginTop: '0.25rem' }}>
                MÜDEK sonuçları yükleniyor…
              </p>
            ) : null}
          </div>

          <div className={styles.sideCol}>
            <div className={styles.panel}>
              <h3 className={styles.panelTitle}>Durum</h3>
              <p className={styles.summaryLine}>
                Son hesaplama:{' '}
                {d.mudekMeta?.lastCalculatedAt ? d.formatDate(d.mudekMeta.lastCalculatedAt) : '—'}
              </p>
            </div>

            {d.evaluation ? (
              <>
                <div className={styles.panel}>
                  <h3 className={styles.panelTitle}>Hızlı erişim</h3>
                  <div className={styles.navButtons}>
                    <button
                      type="button"
                      className={styles.navButton}
                      onClick={() => navigate(`/evaluations/${offeringId}/evaluation/${d.evaluationId}/exams`)}
                    >
                      <GraduationCap size={16} aria-hidden />
                      Sınavlar
                    </button>

                    <button
                      type="button"
                      className={styles.navButton}
                      onClick={() =>
                        navigate(`/evaluations/${offeringId}/evaluation/${d.evaluationId}/letter-grade-rules`)
                      }
                    >
                      Harf notu kuralları
                    </button>
                  </div>
                </div>

                <div className={styles.panel}>
                  <h3 className={styles.panelTitle}>MÜDEK Snapshot</h3>
                  {d.mudekError ? (
                    <p className={sectionStyles.error} role="alert">
                      {d.mudekError}
                    </p>
                  ) : null}
                  <p className={styles.panelHint}>
                    Son hesaplanmış MÜDEK değerlendirme sonuçları bu ders açılışı için aşağıda listelenir.
                  </p>
                  <p className={styles.summaryLine}>
                    Snapshot son hesaplama:{' '}
                    {d.mudekMeta?.lastCalculatedAt ? d.formatDate(d.mudekMeta.lastCalculatedAt) : '—'}
                  </p>
                </div>
              </>
            ) : null}
          </div>
        </div>
      </div>
    </PageSection>
  )
}

