import { ClipboardCheck, Pencil, Trash2 } from 'lucide-react'
import { useCallback, useEffect, useMemo, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'

import {
  createExam,
  deleteExam,
  fetchExams,
  fetchExamById,
  updateExam,
} from '../../../shared/api/teacherApi'
import { appConfig } from '../../../shared/config/appConfig'
import { getTeacherToken } from '../../../shared/lib/authToken'
import { RefreshIconButton } from '../../../shared/ui/refresh-icon-button/RefreshIconButton.jsx'
import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import { parseMaybeNumber, parseRequiredNumber } from '../../../shared/lib/numberUtils.js'
import styles from './EvaluationExamsPage.module.css'

const examTypes = ['Vize', 'Final', 'Bütünleme', 'Quiz', 'Ödev', 'Proje', 'Lab', 'Diğer']

function toCamelBody({ examType, weightPercentage, orderIndex }) {
  return {
    examType,
    weightPercentage: Number(weightPercentage),
    orderIndex: Number(orderIndex),
  }
}

const emptyForm = {
  examType: 'Vize',
  weightPercentage: 0,
  orderIndex: 1,
}

function formatWeight(value) {
  const num = Number(value ?? 0)
  if (!Number.isFinite(num)) return '0'
  return Number.isInteger(num) ? String(num) : num.toFixed(2)
}

export function EvaluationExamsPage() {
  const { offeringId, evaluationId } = useParams()
  const navigate = useNavigate()

  const page = appConfig.pages.evaluations

  const [rows, setRows] = useState([])
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)
  const [search, setSearch] = useState('')
  const [dialogMode, setDialogMode] = useState('create')
  const [form, setForm] = useState(emptyForm)
  const [formError, setFormError] = useState('')
  const [saving, setSaving] = useState(false)
  const [editingId, setEditingId] = useState(null)
  const [editingLoading, setEditingLoading] = useState(false)

  const load = useCallback(() => {
    const token = getTeacherToken()
    if (!token || !evaluationId) return
    setLoading(true)
    setError('')
    fetchExams(token, evaluationId)
      .then((data) => setRows(Array.isArray(data) ? data : []))
      .catch((e) => setError(e instanceof Error ? e.message : 'Sınavlar alınamadı.'))
      .finally(() => setLoading(false))
  }, [evaluationId])

  useEffect(() => {
    load()
  }, [load])

  const openCreate = useCallback(() => {
    setDialogMode('create')
    setEditingId(null)
    setForm(emptyForm)
    setFormError('')
  }, [])

  const openEdit = useCallback((row) => {
    setDialogMode('edit')
    setEditingId(row.id)
    setForm({
      examType: row.examType ?? '',
      weightPercentage: Number(row.weightPercentage ?? 0),
      orderIndex: Number(row.orderIndex ?? 1),
    })
    setFormError('')
    setEditingLoading(true)

    const token = getTeacherToken()
    if (!token) {
      setFormError('Oturum bulunamadı.')
      setEditingLoading(false)
      return
    }

    fetchExamById(token, row.id)
      .then((data) => {
        if (!data) return
        setForm({
          examType: data.examType ?? '',
          weightPercentage: Number(data.weightPercentage ?? 0),
          orderIndex: Number(data.orderIndex ?? 1),
        })
      })
      .catch((e) => setFormError(e instanceof Error ? e.message : 'Sınav detayı alınamadı.'))
      .finally(() => setEditingLoading(false))
  }, [])

  const handleSubmit = async (e) => {
    e.preventDefault()
    const token = getTeacherToken()
    if (!token || !evaluationId) return

    setFormError('')
    setSaving(true)

    try {
      const examType = String(form.examType ?? '').trim()
      if (!examType) {
        setFormError('Exam type zorunludur.')
        return
      }

      const weightPercentage = parseMaybeNumber(form.weightPercentage) ?? 0
      const orderIndex = parseRequiredNumber(form.orderIndex)

      if (dialogMode === 'create') {
        await createExam(token, evaluationId, toCamelBody({ examType, weightPercentage, orderIndex }))
      } else {
        if (!editingId) return
        await updateExam(token, editingId, toCamelBody({ examType, weightPercentage, orderIndex }))
      }

      openCreate()
      load()
    } catch (err) {
      setFormError(err instanceof Error ? err.message : 'Kaydedilemedi.')
    } finally {
      setSaving(false)
    }
  }

  const handleDelete = useCallback(
    async (row) => {
      const token = getTeacherToken()
      if (!token) return
      const ok = window.confirm('Sınav silinsin mi?')
      if (!ok) return
      setSaving(true)
      setError('')
      try {
        await deleteExam(token, row.id)
        load()
      } catch (e) {
        setError(e instanceof Error ? e.message : 'Silme başarısız.')
      } finally {
        setSaving(false)
      }
    },
    [load],
  )

  const filteredRows = useMemo(() => {
    const q = search.trim().toLowerCase()
    if (!q) return rows
    return rows.filter((row) => {
      const type = String(row.examType ?? '').toLowerCase()
      const weight = String(row.weightPercentage ?? '')
      const order = String(row.orderIndex ?? '')
      return type.includes(q) || weight.includes(q) || order.includes(q)
    })
  }, [rows, search])

  const totalWeight = useMemo(
    () =>
      rows.reduce((sum, row) => {
        const value = Number(row.weightPercentage ?? 0)
        return Number.isFinite(value) ? sum + value : sum
      }, 0),
    [rows],
  )

  const progress = Math.min(100, Math.max(0, totalWeight))

  return (
    <PageSection title={`Sınavlar`} description={page.title} error={error}>
      <div className={styles.layout}>
        <aside className={styles.sidebar}>
          <section className={styles.card}>
            <h3 className={styles.cardTitle}>Sınav Konfigürasyonu</h3>
            <div className={styles.metricRow}>
              <span className={styles.metricLabel}>Toplam ağırlık</span>
              <strong className={styles.metricValue}>{totalWeight.toFixed(2)} / 100</strong>
            </div>
            <div className={styles.progress}>
              <div className={styles.progressFill} style={{ width: `${progress}%` }} />
            </div>
            <div className={styles.miniGrid}>
              <div className={styles.miniBox}>
                <span className={styles.miniLabel}>Sınav sayısı</span>
                <strong>{rows.length}</strong>
              </div>
              <div className={styles.miniBox}>
                <span className={styles.miniLabel}>Kalan</span>
                <strong>{Math.max(0, 100 - totalWeight).toFixed(2)}</strong>
              </div>
            </div>
          </section>

          <section className={styles.card}>
            <div className={styles.cardHead}>
              <h3 className={styles.cardTitle}>{dialogMode === 'create' ? 'Sınav Ekle' : 'Sınav Düzenle'}</h3>
              <button type="button" className={styles.ghostBtn} onClick={openCreate} disabled={saving}>
                Yeni
              </button>
            </div>
            <form id="exam-form" className={styles.form} onSubmit={handleSubmit}>
              {editingLoading ? <p className={styles.muted}>Sınav detayı yükleniyor…</p> : null}
              {formError ? (
                <p className={styles.error} role="alert">
                  {formError}
                </p>
              ) : null}

              <label className={styles.field}>
                <span>Sınav türü</span>
                <select
                  value={form.examType}
                  onChange={(e) => setForm((f) => ({ ...f, examType: e.target.value }))}
                >
                  {examTypes.map((t) => (
                    <option key={t} value={t}>
                      {t}
                    </option>
                  ))}
                </select>
              </label>

              <label className={styles.field}>
                <span>Ağırlık (%)</span>
                <input
                  type="number"
                  step="0.01"
                  value={form.weightPercentage}
                  onChange={(e) => setForm((f) => ({ ...f, weightPercentage: e.target.value }))}
                />
              </label>

              <label className={styles.field}>
                <span>Sıra</span>
                <input
                  type="number"
                  step="1"
                  value={form.orderIndex}
                  onChange={(e) => setForm((f) => ({ ...f, orderIndex: e.target.value }))}
                />
              </label>

              <button type="submit" className={styles.primaryBtn} disabled={saving || editingLoading}>
                {saving ? 'Kaydediliyor…' : dialogMode === 'create' ? 'Sınava Ekle' : 'Değişiklikleri Kaydet'}
              </button>
              {dialogMode === 'edit' ? (
                <button type="button" className={styles.secondaryBtn} onClick={openCreate} disabled={saving}>
                  Düzenlemeyi İptal Et
                </button>
              ) : null}
            </form>
          </section>
        </aside>

        <section className={styles.content}>
          <div className={styles.toolbar}>
            <input
              className={styles.search}
              placeholder="Sınav türü, ağırlık veya sıra ara…"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
            />
            <RefreshIconButton onClick={load} loading={loading} />
          </div>

          <div className={styles.listCard}>
            <div className={styles.listHead}>
              <h3>Sınav Listesi ({filteredRows.length})</h3>
            </div>
            <div className={styles.list}>
              {filteredRows.map((row, idx) => (
                <article key={row.id} className={styles.item}>
                  <div className={styles.itemLeft}>
                    <span className={styles.index}>{idx + 1}</span>
                    <div>
                      <h4 className={styles.itemTitle}>{row.examType ?? 'Sınav'}</h4>
                      <div className={styles.badges}>
                        <span className={styles.badge}>Ağırlık %{formatWeight(row.weightPercentage)}</span>
                        <span className={styles.badgeMuted}>Sıra {Number(row.orderIndex ?? 0)}</span>
                        <span className={styles.badgeMuted}>Soru {Number(row.questionCount ?? 0)}</span>
                      </div>
                    </div>
                  </div>
                  <div className={styles.actions}>
                    <button
                      type="button"
                      className={styles.actionBtn}
                      onClick={() =>
                        navigate(
                          `/evaluations/${offeringId}/evaluation/${evaluationId}/exams/${row.id}/questions`,
                        )
                      }
                    >
                      Sorular
                    </button>
                    <button
                      type="button"
                      className={styles.actionBtn}
                      onClick={() =>
                        navigate(
                          `/evaluations/${offeringId}/evaluation/${evaluationId}/exams/${row.id}/components`,
                        )
                      }
                    >
                      <ClipboardCheck size={14} aria-hidden />
                      Bileşenler
                    </button>
                    <button type="button" className={styles.actionBtn} onClick={() => openEdit(row)}>
                      <Pencil size={14} aria-hidden />
                      Düzenle
                    </button>
                    <button
                      type="button"
                      className={`${styles.actionBtn} ${styles.danger}`}
                      onClick={() => handleDelete(row)}
                      disabled={saving}
                    >
                      <Trash2 size={14} aria-hidden />
                      Sil
                    </button>
                  </div>
                </article>
              ))}
              {filteredRows.length === 0 ? (
                <div className={styles.empty}>
                  <p>Aramaya uygun sınav bulunamadı.</p>
                </div>
              ) : null}
            </div>
          </div>
        </section>
      </div>
    </PageSection>
  )
}

