import { Download, Upload } from 'lucide-react'
import { useCallback, useEffect, useState } from 'react'

import {
  fetchAllCourseOfferings,
  fetchCourseOfferingsActiveTerm,
  importEnrollmentsFromExcel,
} from '../../../shared/api/adminApi'
import { appConfig } from '../../../shared/config/appConfig'
import { getAdminToken } from '../../../shared/lib/authToken'
import formStyles from '../../../shared/ui/admin-form/AdminForm.module.css'
import { RefreshIconButton } from '../../../shared/ui/refresh-icon-button/RefreshIconButton.jsx'
import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import sectionStyles from '@shared/ui/page-section/PageSection.module.css'
import pageStyles from './EnrollmentBulkPage.module.css'

function downloadSeedTemplateCsv() {
  // Excel bazı yerel ayarlarda `,` yerine `;` ayırıcı bekleyebilir.
  // Bu sayede kullanıcı tek sütun yerine 3 ayrı sütun görür.
  const delimiter = ';'
  const csv = [
    `StudentNumber${delimiter}FullName${delimiter}Email`,
    `2023000001${delimiter}Ali Veli${delimiter}ali.veli@ogr.ktun.edu.tr`,
    `2023000002${delimiter}Ayse Yilmaz${delimiter}ayse.yilmaz@ogr.ktun.edu.tr`,
  ].join('\n')
  const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' })
  const url = URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = 'enrollment-import-seed-template.csv'
  document.body.appendChild(a)
  a.click()
  document.body.removeChild(a)
  URL.revokeObjectURL(url)
}

export function EnrollmentBulkPage() {
  const page = appConfig.pages.enrollmentBulk
  const [scope, setScope] = useState('active')
  const [offerings, setOfferings] = useState([])
  const [offeringId, setOfferingId] = useState('')
  const [file, setFile] = useState(null)
  const [error, setError] = useState('')
  const [success, setSuccess] = useState('')
  const [loadingList, setLoadingList] = useState(false)
  const [saving, setSaving] = useState(false)

  const loadOfferings = useCallback(() => {
    const token = getAdminToken()
    if (!token) return
    setLoadingList(true)
    setError('')
    const req = scope === 'active' ? fetchCourseOfferingsActiveTerm(token) : fetchAllCourseOfferings(token)
    req
      .then((data) => {
        const list = Array.isArray(data) ? data : []
        setOfferings(list)
        setOfferingId((prev) => {
          if (!list.length) return ''
          if (prev && list.some((o) => o.id === prev)) return prev
          return list[0].id
        })
      })
      .catch((e) => setError(e instanceof Error ? e.message : 'Açılış listesi alınamadı.'))
      .finally(() => setLoadingList(false))
  }, [scope])

  useEffect(() => {
    loadOfferings()
  }, [loadOfferings])

  const handleImport = async (e) => {
    e.preventDefault()
    const token = getAdminToken()
    if (!token || !offeringId) {
      setError('Ders açılışı seçin.')
      return
    }
    if (!file) {
      setError('Excel dosyası seçin.')
      return
    }
    setSaving(true)
    setError('')
    setSuccess('')
    try {
      await importEnrollmentsFromExcel(token, offeringId, file)
      setSuccess('Excel içe aktarma isteği gönderildi.')
      setFile(null)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'İçe aktarma başarısız.')
    } finally {
      setSaving(false)
    }
  }

  const selectedOffering = offerings.find((o) => String(o.id) === String(offeringId))

  return (
    <PageSection title={page.title} description={page.description} error={error}>
      <div className={pageStyles.stack}>
        <div className={pageStyles.panel}>
          <div className={pageStyles.toolbarRow}>
            <RefreshIconButton onClick={loadOfferings} loading={loadingList} disabled={saving} />
          </div>

          <div className={pageStyles.fieldsGrid}>
            <div className={pageStyles.fieldCard}>
              <div className={formStyles.field}>
                <label className={formStyles.label} htmlFor="eb-scope">
                  Liste kaynağı
                </label>
                <select
                  id="eb-scope"
                  className={formStyles.input}
                  value={scope}
                  onChange={(e) => setScope(e.target.value)}
                  disabled={saving}
                >
                  <option value="active">Aktif dönem açılışları</option>
                  <option value="all">Tüm açılışlar</option>
                </select>
              </div>
            </div>

            <div className={pageStyles.fieldCard}>
              <div className={formStyles.field}>
                <label className={formStyles.label} htmlFor="eb-offering">
                  Ders açılışı
                </label>
                <select
                  id="eb-offering"
                  className={formStyles.input}
                  value={offeringId}
                  onChange={(e) => setOfferingId(e.target.value)}
                  disabled={loadingList || saving}
                >
                  <option value="">{loadingList ? 'Yükleniyor…' : 'Seçin'}</option>
                  {offerings.map((o) => (
                    <option key={o.id} value={o.id}>
                      {o.courseCode} — {o.termName} {o.section ? `(${o.section})` : ''}
                    </option>
                  ))}
                </select>
              </div>
            </div>
          </div>

          {selectedOffering ? (
            <div className={pageStyles.selectedCard} aria-live="polite">
              <div className={pageStyles.selectedLabel}>Seçili ders açılışı</div>
              <p className={sectionStyles.muted} style={{ margin: 0 }}>
                <span className={pageStyles.selectedStrong}>{selectedOffering.courseName ?? selectedOffering.courseCode}</span> ·{' '}
                {selectedOffering.termName}
              </p>
            </div>
          ) : null}

          {success ? (
            <p className={pageStyles.success} role="status">
              {success}
            </p>
          ) : null}

          <form className={formStyles.form} onSubmit={handleImport}>
            <h3 className={pageStyles.subheading}>Excel ile içe aktarma</h3>
            <p className={sectionStyles.muted}>Şablon ve sütun sırası API dokümantasyonuna uymalıdır.</p>
            <p className={pageStyles.columnsHelp} aria-label="Beklenen sütunlar">
              <span className={sectionStyles.code}>StudentNumber</span>
              <span className={sectionStyles.code}>FullName</span>
              <span className={sectionStyles.code}>Email</span>
            </p>

            <div className={pageStyles.templateActions}>
              <button
                type="button"
                className={`${formStyles.btn} ${formStyles.btnGhost}`}
                onClick={downloadSeedTemplateCsv}
              >
                <Download size={18} aria-hidden />
                Örnek seed şablonu indir
              </button>
            </div>

            <div className={formStyles.field}>
              <span className={formStyles.label}>Dosya</span>
              <div className={pageStyles.fileRow}>
                <label
                  className={`${pageStyles.fileLabel} ${saving ? pageStyles.fileLabelDisabled : ''}`}
                  htmlFor="eb-file"
                >
                  <Upload size={16} aria-hidden />
                  <span>{file ? 'Dosyayı değiştir' : 'Dosya seç (.xlsx / .xls)'}</span>
                </label>
                <input
                  id="eb-file"
                  type="file"
                  accept=".xlsx,.xls"
                  className={pageStyles.fileInput}
                  disabled={saving}
                  onChange={(e) => setFile(e.target.files?.[0] ?? null)}
                />
                <span className={pageStyles.fileName}>{file ? file.name : 'Henüz dosya seçilmedi.'}</span>
              </div>
            </div>

            <button
              type="submit"
              className={`${formStyles.btn} ${formStyles.btnPrimary}`}
              disabled={saving || !offeringId || !file}
            >
              <Upload size={18} aria-hidden />
              {saving ? 'Yükleniyor…' : 'İçe aktar'}
            </button>
          </form>
        </div>
      </div>
    </PageSection>
  )
}
