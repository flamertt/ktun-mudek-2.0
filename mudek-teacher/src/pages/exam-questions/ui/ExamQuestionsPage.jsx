import { createColumnHelper } from '@tanstack/react-table'
import { Pencil, Plus, Trash2 } from 'lucide-react'
import { useCallback, useEffect, useMemo, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'

import {
  createQuestion,
  deleteQuestion,
  fetchQuestions,
  fetchQuestionById,
  updateQuestion,
} from '../../../shared/api/teacherApi'
import { appConfig } from '../../../shared/config/appConfig'
import { getTeacherToken } from '../../../shared/lib/authToken'
import { parseMaybeNumber, parseRequiredNumber } from '../../../shared/lib/numberUtils.js'
import { AppDialog } from '../../../shared/ui/dialog/AppDialog.jsx'
import formStyles from '../../../shared/ui/admin-form/AdminForm.module.css'
import { DataTable } from '../../../shared/ui/data-table/DataTable.jsx'
import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import sectionStyles from '@shared/ui/page-section/PageSection.module.css'
import { RefreshIconButton } from '../../../shared/ui/refresh-icon-button/RefreshIconButton.jsx'

const columnHelper = createColumnHelper()

const questionTypes = [
  'WrittenQuestion',
  'ShortAnswer',
  'MultipleChoice',
  'Essay',
  'Homework',
  'Project',
  'Lab',
  'Other',
]

const emptyForm = {
  questionNumber: 1,
  maxScore: 100,
  title: '',
  description: '',
  questionType: 'WrittenQuestion',
}

function trimOrNull(v) {
  const s = String(v ?? '').trim()
  return s ? s : null
}

export function ExamQuestionsPage() {
  const { offeringId, evaluationId, examId } = useParams()
  const navigate = useNavigate()

  const page = appConfig.pages.evaluations

  const [rows, setRows] = useState([])
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)
  const [globalFilter, setGlobalFilter] = useState('')

  const [dialogOpen, setDialogOpen] = useState(false)
  const [dialogMode, setDialogMode] = useState('create')
  const [form, setForm] = useState(emptyForm)
  const [formError, setFormError] = useState('')
  const [saving, setSaving] = useState(false)
  const [editingId, setEditingId] = useState(null)
  const [editingLoading, setEditingLoading] = useState(false)

  const load = useCallback(() => {
    const token = getTeacherToken()
    if (!token || !examId) return
    setLoading(true)
    setError('')
    fetchQuestions(token, examId)
      .then((data) => setRows(Array.isArray(data) ? data : []))
      .catch((e) => setError(e instanceof Error ? e.message : 'Sorular alınamadı.'))
      .finally(() => setLoading(false))
  }, [examId])

  useEffect(() => {
    load()
  }, [load])

  const openCreate = useCallback(() => {
    setDialogMode('create')
    setEditingId(null)
    setForm(emptyForm)
    setFormError('')
    setDialogOpen(true)
  }, [])

  const openEdit = useCallback((row) => {
    setDialogMode('edit')
    setEditingId(row.id)
    setForm({
      questionNumber: Number(row.questionNumber ?? 1),
      maxScore: Number(row.maxScore ?? 100),
      title: row.title ?? '',
      description: row.description ?? '',
      questionType: row.questionType ?? 'WrittenQuestion',
    })
    setFormError('')
    setDialogOpen(true)
    setEditingLoading(true)

    const token = getTeacherToken()
    if (!token) {
      setFormError('Oturum bulunamadı.')
      setEditingLoading(false)
      return
    }

    fetchQuestionById(token, row.id)
      .then((data) => {
        if (!data) return
        setForm({
          questionNumber: Number(data.questionNumber ?? 1),
          maxScore: Number(data.maxScore ?? 100),
          title: data.title ?? '',
          description: data.description ?? '',
          questionType: data.questionType ?? 'WrittenQuestion',
        })
      })
      .catch((e) => setFormError(e instanceof Error ? e.message : 'Soru detayı alınamadı.'))
      .finally(() => setEditingLoading(false))
  }, [])

  const handleSubmit = async (e) => {
    e.preventDefault()
    const token = getTeacherToken()
    if (!token || !examId) return

    setFormError('')

    try {
      const questionNumber = parseRequiredNumber(form.questionNumber)
      const maxScore = parseMaybeNumber(form.maxScore) ?? 0
      const questionType = String(form.questionType ?? '').trim()

      if (!questionType) {
        setFormError('Soru tipi zorunludur.')
        return
      }

      const body = {
        questionNumber,
        maxScore,
        title: trimOrNull(form.title),
        description: trimOrNull(form.description),
        questionType,
      }

      if (dialogMode === 'create') {
        await createQuestion(token, examId, body)
      } else {
        if (!editingId) return
        await updateQuestion(token, editingId, body)
      }

      setDialogOpen(false)
      load()
    } catch (err) {
      setFormError(err instanceof Error ? err.message : 'Kaydedilemedi.')
    } finally {
      setSaving(false)
    }
  }

  const handleDelete = async (row) => {
    const token = getTeacherToken()
    if (!token) return
    const ok = window.confirm('Soru silinsin mi?')
    if (!ok) return
    setSaving(true)
    setError('')
    try {
      await deleteQuestion(token, row.id)
      load()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Silme başarısız.')
    } finally {
      setSaving(false)
    }
  }

  const columns = useMemo(
    () => [
      columnHelper.accessor('questionNumber', { header: 'Soru no' }),
      columnHelper.accessor('maxScore', { header: 'Maks. puan', cell: (info) => String(info.getValue() ?? 0) }),
      columnHelper.accessor('title', { header: 'Başlık', cell: (info) => info.getValue() ?? '—' }),
      columnHelper.accessor('questionType', { header: 'Tip', cell: (info) => info.getValue() ?? '—' }),
      columnHelper.accessor('outcomeMappings', {
        header: 'CLO eşleme',
        cell: (info) => (Array.isArray(info.getValue()) ? info.getValue().length : 0),
      }),
      columnHelper.display({
        id: 'actions',
        header: 'İşlemler',
        cell: ({ row }) => (
          <div className={formStyles.rowActionGroup}>
            <button
              type="button"
              className={formStyles.rowActionText}
              onClick={() =>
                navigate(
                  `/evaluations/${offeringId}/evaluation/${evaluationId}/questions/${row.original.id}/clos`,
                )
              }
            >
              DÖÇ eşlemesi
            </button>
            <button
              type="button"
              className={formStyles.rowActionText}
              onClick={() =>
                navigate(
                  `/evaluations/${offeringId}/evaluation/${evaluationId}/questions/${row.original.id}/answers`,
                )
              }
            >
              Öğrenci cevapları
            </button>
            <button type="button" className={formStyles.rowActionText} onClick={() => openEdit(row.original)}>
              <Pencil size={14} aria-hidden />
              Düzenle
            </button>
            <button
              type="button"
              className={`${formStyles.rowActionText} ${formStyles.rowActionTextDanger}`}
              onClick={() => handleDelete(row.original)}
              disabled={saving}
            >
              <Trash2 size={14} aria-hidden />
              Sil
            </button>
          </div>
        ),
      }),
    ],
    [evaluationId, handleDelete, navigate, offeringId, openEdit, saving],
  )

  return (
    <PageSection title="Soru yönetimi" description={page.title} error={error}>
      <DataTable
        columns={columns}
        data={rows}
        globalFilter={globalFilter}
        onGlobalFilterChange={setGlobalFilter}
        searchPlaceholder="Soru no veya başlık ara…"
        toolbarExtra={
          <>
            <RefreshIconButton onClick={load} loading={loading} title="Soruları yenile" />
            <button type="button" className={`${formStyles.btn} ${formStyles.btnPrimary}`} onClick={openCreate}>
              <Plus size={18} aria-hidden />
              Yeni soru
            </button>
          </>
        }
        isLoading={loading}
      />

      <AppDialog
        open={dialogOpen}
        onClose={() => {
          if (!saving) setDialogOpen(false)
        }}
        title={dialogMode === 'create' ? 'Yeni soru' : 'Soru düzenle'}
        size="md"
        footer={
          <>
            <button
              type="button"
              className={`${formStyles.btn} ${formStyles.btnGhost}`}
              onClick={() => setDialogOpen(false)}
              disabled={saving || editingLoading}
            >
              Vazgeç
            </button>
            <button
              type="submit"
              form="question-form"
              className={`${formStyles.btn} ${formStyles.btnPrimary}`}
              disabled={saving || editingLoading}
            >
              {saving ? 'Kaydediliyor…' : 'Kaydet'}
            </button>
          </>
        }
      >
        <form id="question-form" className={formStyles.form} onSubmit={handleSubmit}>
          {editingLoading ? (
            <p className={sectionStyles.muted} style={{ marginTop: 0, marginBottom: '0.75rem' }}>
              Soru detayı yükleniyor…
            </p>
          ) : null}
          {formError ? (
            <p className={sectionStyles.error} role="alert">
              {formError}
            </p>
          ) : null}

          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="questionNumber">
              Soru no
            </label>
            <input
              id="questionNumber"
              type="number"
              step="1"
              className={formStyles.input}
              value={form.questionNumber}
              onChange={(e) => setForm((f) => ({ ...f, questionNumber: e.target.value }))}
            />
          </div>

          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="maxScore">
              Maks. puan
            </label>
            <input
              id="maxScore"
              type="number"
              step="0.01"
              className={formStyles.input}
              value={form.maxScore}
              onChange={(e) => setForm((f) => ({ ...f, maxScore: e.target.value }))}
            />
          </div>

          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="questionType">
              Soru tipi
            </label>
            <select
              id="questionType"
              className={formStyles.select}
              value={form.questionType}
              onChange={(e) => setForm((f) => ({ ...f, questionType: e.target.value }))}
            >
              {questionTypes.map((t) => (
                <option key={t} value={t}>
                  {t}
                </option>
              ))}
            </select>
          </div>

          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="title">
              Başlık (opsiyonel)
            </label>
            <input
              id="title"
              className={formStyles.input}
              value={form.title}
              onChange={(e) => setForm((f) => ({ ...f, title: e.target.value }))}
            />
          </div>

          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="description">
              Açıklama (opsiyonel)
            </label>
            <textarea
              id="description"
              className={formStyles.textarea}
              value={form.description}
              onChange={(e) => setForm((f) => ({ ...f, description: e.target.value }))}
            />
          </div>
        </form>
      </AppDialog>
    </PageSection>
  )
}

