import { createColumnHelper } from '@tanstack/react-table'
import { Pencil, Plus, Trash2 } from 'lucide-react'
import { useCallback, useEffect, useMemo, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'

import {
  createComponent,
  deleteComponent,
  fetchComponent,
  fetchComponents,
  updateComponent,
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
  name: '',
  componentType: 'Quiz',
  maxScore: 100,
  weightPercentage: 0,
  orderIndex: 1,
  description: '',
  isActive: true,
}

function trimOrNull(v) {
  const s = String(v ?? '').trim()
  return s ? s : null
}

export function ExamComponentsPage() {
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
    fetchComponents(token, examId)
      .then((data) => setRows(Array.isArray(data) ? data : []))
      .catch((e) => setError(e instanceof Error ? e.message : 'Bileşenler alınamadı.'))
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
      name: row.name ?? '',
      componentType: row.componentType ?? 'Quiz',
      maxScore: Number(row.maxScore ?? 0),
      weightPercentage: Number(row.weightPercentage ?? 0),
      orderIndex: Number(row.orderIndex ?? 1),
      description: row.description ?? '',
      isActive: row.isActive !== false,
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

    fetchComponent(token, row.id)
      .then((data) => {
        if (!data) return
        setForm({
          name: data.name ?? '',
          componentType: data.componentType ?? 'Quiz',
          maxScore: Number(data.maxScore ?? 0),
          weightPercentage: Number(data.weightPercentage ?? 0),
          orderIndex: Number(data.orderIndex ?? 1),
          description: data.description ?? '',
          isActive: data.isActive !== false,
        })
      })
      .catch((e) => setFormError(e instanceof Error ? e.message : 'Bileşen detayı alınamadı.'))
      .finally(() => setEditingLoading(false))
  }, [])

  const handleSubmit = async (e) => {
    e.preventDefault()
    const token = getTeacherToken()
    if (!token || !examId) return

    setFormError('')
    setSaving(true)
    try {
      const name = String(form.name ?? '').trim()
      const componentType = String(form.componentType ?? '').trim()
      if (!name || !componentType) {
        setFormError('Ad ve bileşen tipi zorunludur.')
        return
      }

      const maxScore = parseMaybeNumber(form.maxScore) ?? 0
      const weightPercentage = parseMaybeNumber(form.weightPercentage) ?? 0
      const orderIndex = parseRequiredNumber(form.orderIndex)

      const body = {
        name,
        componentType,
        maxScore,
        weightPercentage,
        orderIndex,
        description: trimOrNull(form.description),
        isActive: Boolean(form.isActive),
      }

      if (dialogMode === 'create') {
        // controller dto.ExamId route'dan set eder
        const { isActive: _ignored, ...createBody } = body
        await createComponent(token, examId, createBody)
      } else {
        if (!editingId) return
        await updateComponent(token, editingId, body)
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
    const ok = window.confirm('Bileşen silinsin mi?')
    if (!ok) return
    setSaving(true)
    setError('')
    try {
      await deleteComponent(token, row.id)
      load()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Silme başarısız.')
    } finally {
      setSaving(false)
    }
  }

  const columns = useMemo(
    () => [
      columnHelper.accessor('orderIndex', { header: 'Sıra' }),
      columnHelper.accessor('name', { header: 'Ad', cell: (info) => info.getValue() ?? '—' }),
      columnHelper.accessor('componentType', { header: 'Tip', cell: (info) => info.getValue() ?? '—' }),
      columnHelper.accessor('maxScore', { header: 'Maks. puan', cell: (info) => String(info.getValue() ?? 0) }),
      columnHelper.accessor('weightPercentage', { header: 'Ağırlık (%)', cell: (info) => String(info.getValue() ?? 0) }),
      columnHelper.accessor('isActive', {
        header: 'Aktif',
        cell: (info) => (info.getValue() ? 'Evet' : 'Hayır'),
      }),
      columnHelper.display({
        id: 'actions',
        header: 'İşlemler',
        cell: ({ row }) => (
          <div className={formStyles.actions}>
            <button
              type="button"
              className={formStyles.actionIcon}
              title="CLO eşleştir"
              onClick={() =>
                navigate(
                  `/evaluations/${offeringId}/evaluation/${evaluationId}/components/${row.original.id}/clos`,
                )
              }
            >
              C
            </button>
            <button
              type="button"
              className={formStyles.actionIcon}
              title="Notlar"
              onClick={() =>
                navigate(
                  `/evaluations/${offeringId}/evaluation/${evaluationId}/components/${row.original.id}/scores`,
                )
              }
            >
              N
            </button>
            <button
              type="button"
              className={formStyles.actionIcon}
              title="Düzenle"
              onClick={() => openEdit(row.original)}
            >
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
    [handleDelete, navigate, offeringId, openEdit, saving],
  )

  return (
    <PageSection title="Bileşenler" description={page.title} error={error}>
      <DataTable
        columns={columns}
        data={rows}
        globalFilter={globalFilter}
        onGlobalFilterChange={setGlobalFilter}
        searchPlaceholder="Ad veya tip ara…"
        toolbarExtra={
          <>
            <RefreshIconButton onClick={load} loading={loading} title="Bileşenleri yenile" />
            <button type="button" className={`${formStyles.btn} ${formStyles.btnPrimary}`} onClick={openCreate}>
              <Plus size={18} aria-hidden />
              Yeni bileşen
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
        title={dialogMode === 'create' ? 'Yeni bileşen' : 'Bileşeni düzenle'}
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
              form="component-form"
              className={`${formStyles.btn} ${formStyles.btnPrimary}`}
              disabled={saving || editingLoading}
            >
              {saving ? 'Kaydediliyor…' : 'Kaydet'}
            </button>
          </>
        }
      >
        <form id="component-form" className={formStyles.form} onSubmit={handleSubmit}>
          {editingLoading ? (
            <p className={sectionStyles.muted} style={{ marginTop: 0, marginBottom: '0.75rem' }}>
              Bileşen detayı yükleniyor…
            </p>
          ) : null}
          {formError ? (
            <p className={sectionStyles.error} role="alert">
              {formError}
            </p>
          ) : null}

          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="componentName">
              Ad
            </label>
            <input
              id="componentName"
              className={formStyles.input}
              value={form.name}
              onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))}
              required
            />
          </div>

          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="componentType">
              Bileşen tipi
            </label>
            <input
              id="componentType"
              className={formStyles.input}
              value={form.componentType}
              onChange={(e) => setForm((f) => ({ ...f, componentType: e.target.value }))}
              required
            />
          </div>

          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="componentMaxScore">
              Maks. puan
            </label>
            <input
              id="componentMaxScore"
              type="number"
              step="0.01"
              className={formStyles.input}
              value={form.maxScore}
              onChange={(e) => setForm((f) => ({ ...f, maxScore: e.target.value }))}
            />
          </div>

          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="weightPercentage">
              Ağırlık (%)
            </label>
            <input
              id="weightPercentage"
              type="number"
              step="0.01"
              className={formStyles.input}
              value={form.weightPercentage}
              onChange={(e) => setForm((f) => ({ ...f, weightPercentage: e.target.value }))}
            />
          </div>

          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="orderIndex">
              Sıra
            </label>
            <input
              id="orderIndex"
              type="number"
              step="1"
              className={formStyles.input}
              value={form.orderIndex}
              onChange={(e) => setForm((f) => ({ ...f, orderIndex: e.target.value }))}
            />
          </div>

          {dialogMode === 'edit' ? (
            <div className={formStyles.checkRow}>
              <input
                id="isActive"
                type="checkbox"
                className={formStyles.check}
                checked={Boolean(form.isActive)}
                onChange={(e) => setForm((f) => ({ ...f, isActive: e.target.checked }))}
              />
              <label htmlFor="isActive">Aktif</label>
            </div>
          ) : null}

          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="componentDescription">
              Açıklama (opsiyonel)
            </label>
            <textarea
              id="componentDescription"
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

