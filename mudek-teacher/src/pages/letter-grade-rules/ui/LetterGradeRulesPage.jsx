import { createColumnHelper } from '@tanstack/react-table'
import { Plus, Pencil, Trash2 } from 'lucide-react'
import { useCallback, useEffect, useMemo, useState } from 'react'
import { useParams } from 'react-router-dom'

import {
  addLetterRule,
  deleteLetterRule,
  fetchLetterGradeRules,
  updateLetterRule,
} from '../../../shared/api/teacherApi'
import { appConfig } from '../../../shared/config/appConfig'
import { getTeacherToken } from '../../../shared/lib/authToken'
import { parseMaybeNumber, parseRequiredNumber } from '../../../shared/lib/numberUtils.js'
import { AppDialog } from '../../../shared/ui/dialog/AppDialog.jsx'
import { DataTable } from '../../../shared/ui/data-table/DataTable.jsx'
import formStyles from '../../../shared/ui/admin-form/AdminForm.module.css'
import { RefreshIconButton } from '../../../shared/ui/refresh-icon-button/RefreshIconButton.jsx'
import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import sectionStyles from '@shared/ui/page-section/PageSection.module.css'

const columnHelper = createColumnHelper()

const emptyForm = {
  letterGrade: '',
  minScore: 0,
  maxScore: 0,
  isPassing: true,
  minimumFinalScore: '',
  description: '',
}

function trimOrNull(v) {
  const s = String(v ?? '').trim()
  return s ? s : null
}

export function LetterGradeRulesPage() {
  const { evaluationId } = useParams()

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

  const load = useCallback(() => {
    const token = getTeacherToken()
    if (!token || !evaluationId) return
    setLoading(true)
    setError('')
    fetchLetterGradeRules(token, evaluationId)
      .then((data) => setRows(Array.isArray(data) ? data : []))
      .catch((e) => setError(e instanceof Error ? e.message : 'Harf notu kuralları alınamadı.'))
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
    setDialogOpen(true)
  }, [])

  const openEdit = useCallback((row) => {
    setDialogMode('edit')
    setEditingId(row.id)
    setForm({
      letterGrade: row.letterGrade ?? '',
      minScore: Number(row.minScore ?? 0),
      maxScore: Number(row.maxScore ?? 0),
      isPassing: Boolean(row.isPassing),
      minimumFinalScore: row.minimumFinalScore != null ? String(row.minimumFinalScore) : '',
      description: row.description ?? '',
    })
    setFormError('')
    setDialogOpen(true)
  }, [])

  const handleSubmit = async (e) => {
    e.preventDefault()
    const token = getTeacherToken()
    if (!token || !evaluationId) return

    const letterGrade = String(form.letterGrade ?? '').trim()
    if (!letterGrade) {
      setFormError('Harf notu zorunlu.')
      return
    }

    const minScore = parseRequiredNumber(form.minScore)
    const maxScore = parseRequiredNumber(form.maxScore)
    const minimumFinalScore = parseMaybeNumber(form.minimumFinalScore)

    const body = {
      letterGrade,
      minScore,
      maxScore,
      isPassing: Boolean(form.isPassing),
      minimumFinalScore,
      description: trimOrNull(form.description),
    }

    setFormError('')
    setSaving(true)
    try {
      if (dialogMode === 'create') {
        await addLetterRule(token, evaluationId, body)
      } else {
        if (!editingId) return
        await updateLetterRule(token, editingId, body)
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
    const ok = window.confirm('Kural silinsin mi?')
    if (!ok) return
    setSaving(true)
    setError('')
    try {
      await deleteLetterRule(token, row.id)
      load()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Silinemedi.')
    } finally {
      setSaving(false)
    }
  }

  const columns = useMemo(
    () => [
      columnHelper.accessor('letterGrade', { header: 'Harf' }),
      columnHelper.accessor('minScore', { header: 'Min', cell: (info) => String(info.getValue() ?? 0) }),
      columnHelper.accessor('maxScore', { header: 'Max', cell: (info) => String(info.getValue() ?? 0) }),
      columnHelper.accessor('isPassing', { header: 'Geçiyor', cell: (info) => (info.getValue() ? 'Evet' : 'Hayır') }),
      columnHelper.accessor('minimumFinalScore', { header: 'Dönem sonu min', cell: (info) => info.getValue() ?? '—' }),
      columnHelper.accessor('description', { header: 'Açıklama', cell: (info) => info.getValue() ?? '—' }),
      columnHelper.display({
        id: 'actions',
        header: 'İşlemler',
        cell: ({ row }) => (
          <div className={formStyles.actions}>
            <button type="button" className={formStyles.actionIcon} title="Düzenle" onClick={() => openEdit(row.original)}>
              <Pencil size={16} aria-hidden />
            </button>
            <button
              type="button"
              className={`${formStyles.actionIcon} ${formStyles.actionDanger}`}
              title="Sil"
              onClick={() => handleDelete(row.original)}
              disabled={saving}
            >
              <Trash2 size={16} aria-hidden />
            </button>
          </div>
        ),
      }),
    ],
    [handleDelete, openEdit, saving],
  )

  return (
    <PageSection title="Harf notu kuralları" description={page.title} error={error}>
      <DataTable
        columns={columns}
        data={rows}
        globalFilter={globalFilter}
        onGlobalFilterChange={setGlobalFilter}
        searchPlaceholder="Harf veya açıklama ara…"
        toolbarExtra={
          <>
            <RefreshIconButton onClick={load} loading={loading} title="Kuralları yenile" />
            <button type="button" className={`${formStyles.btn} ${formStyles.btnPrimary}`} onClick={openCreate}>
              <Plus size={18} aria-hidden />
              Yeni kural
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
        title={dialogMode === 'create' ? 'Yeni kural' : 'Kuralı düzenle'}
        size="md"
        footer={
          <>
            <button
              type="button"
              className={`${formStyles.btn} ${formStyles.btnGhost}`}
              onClick={() => setDialogOpen(false)}
              disabled={saving}
            >
              Vazgeç
            </button>
            <button type="submit" form="letter-rule-form" className={`${formStyles.btn} ${formStyles.btnPrimary}`} disabled={saving}>
              {saving ? 'Kaydediliyor…' : 'Kaydet'}
            </button>
          </>
        }
      >
        <form id="letter-rule-form" className={formStyles.form} onSubmit={handleSubmit}>
          {formError ? (
            <p className={sectionStyles.error} role="alert">
              {formError}
            </p>
          ) : null}

          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="letterGrade">
              Harf
            </label>
            <input
              id="letterGrade"
              className={formStyles.input}
              value={form.letterGrade}
              onChange={(e) => setForm((f) => ({ ...f, letterGrade: e.target.value }))}
              required
            />
          </div>

          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="minScore">
              Min skor
            </label>
            <input
              id="minScore"
              type="number"
              step="0.01"
              className={formStyles.input}
              value={form.minScore}
              onChange={(e) => setForm((f) => ({ ...f, minScore: e.target.value }))}
              required
            />
          </div>

          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="maxScore">
              Max skor
            </label>
            <input
              id="maxScore"
              type="number"
              step="0.01"
              className={formStyles.input}
              value={form.maxScore}
              onChange={(e) => setForm((f) => ({ ...f, maxScore: e.target.value }))}
              required
            />
          </div>

          <div className={formStyles.checkRow}>
            <input
              id="isPassing"
              type="checkbox"
              className={formStyles.check}
              checked={Boolean(form.isPassing)}
              onChange={(e) => setForm((f) => ({ ...f, isPassing: e.target.checked }))}
            />
            <label htmlFor="isPassing">Geçiyor</label>
          </div>

          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="minimumFinalScore">
              Minimum final skoru (opsiyonel)
            </label>
            <input
              id="minimumFinalScore"
              type="number"
              step="0.01"
              className={formStyles.input}
              value={form.minimumFinalScore}
              onChange={(e) => setForm((f) => ({ ...f, minimumFinalScore: e.target.value }))}
            />
          </div>

          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="description">
              Açıklama
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

