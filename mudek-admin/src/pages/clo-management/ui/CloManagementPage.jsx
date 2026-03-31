import { createColumnHelper } from '@tanstack/react-table'
import { Pencil, Plus, Trash2 } from 'lucide-react'
import { useCallback, useEffect, useMemo, useState } from 'react'

import {
  createClo,
  deleteClo,
  fetchCourses,
  fetchCourseClos,
  fetchPrograms,
  updateClo,
} from '../../../shared/api/adminApi'
import { appConfig } from '../../../shared/config/appConfig'
import { getAdminToken } from '../../../shared/lib/authToken'
import formStyles from '../../../shared/ui/admin-form/AdminForm.module.css'
import { DataTable } from '../../../shared/ui/data-table/DataTable.jsx'
import { AppDialog } from '../../../shared/ui/dialog/AppDialog.jsx'
import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import sectionStyles from '@shared/ui/page-section/PageSection.module.css'
import { RefreshIconButton } from '../../../shared/ui/refresh-icon-button/RefreshIconButton.jsx'
import pageStyles from './CloManagementPage.module.css'

const columnHelper = createColumnHelper()

const emptyForm = { code: '', description: '', orderIndex: 1 }

export function CloManagementPage() {
  const page = appConfig.pages.cloManagement
  const [programs, setPrograms] = useState([])
  const [programId, setProgramId] = useState('')
  const [courses, setCourses] = useState([])
  const [courseId, setCourseId] = useState('')
  const [catalogError, setCatalogError] = useState('')
  const [loadingCatalog, setLoadingCatalog] = useState(true)
  const [rows, setRows] = useState([])
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)
  const [globalFilter, setGlobalFilter] = useState('')

  const [dialogOpen, setDialogOpen] = useState(false)
  const [dialogMode, setDialogMode] = useState('create')
  const [form, setForm] = useState(emptyForm)
  const [editingCloId, setEditingCloId] = useState(null)
  const [formError, setFormError] = useState('')
  const [saving, setSaving] = useState(false)
  const [deleteTarget, setDeleteTarget] = useState(null)

  const loadPrograms = useCallback(() => {
    const token = getAdminToken()
    if (!token) return
    fetchPrograms(token)
      .then((data) => setPrograms(Array.isArray(data) ? data : []))
      .catch((e) => setCatalogError(e instanceof Error ? e.message : 'Programlar yüklenemedi.'))
  }, [])

  const loadCourses = useCallback(() => {
    const token = getAdminToken()
    if (!token) {
      setLoadingCatalog(false)
      return
    }
    setLoadingCatalog(true)
    setCatalogError('')
    fetchCourses(token, programId || undefined)
      .then((data) => setCourses(Array.isArray(data) ? data : []))
      .catch((e) => {
        setCourses([])
        setCatalogError(e instanceof Error ? e.message : 'Ders listesi alınamadı.')
      })
      .finally(() => setLoadingCatalog(false))
  }, [programId])

  const loadClos = useCallback(() => {
    const token = getAdminToken()
    if (!token || !courseId) {
      setRows([])
      return
    }
    setLoading(true)
    setError('')
    fetchCourseClos(token, courseId)
      .then((data) => setRows(Array.isArray(data) ? data : []))
      .catch((e) => setError(e instanceof Error ? e.message : 'CLO listesi alınamadı.'))
      .finally(() => setLoading(false))
  }, [courseId])

  useEffect(() => {
    loadPrograms()
  }, [loadPrograms])

  useEffect(() => {
    loadCourses()
  }, [loadCourses])

  useEffect(() => {
    loadClos()
  }, [loadClos])

  const handleRefresh = useCallback(() => {
    loadPrograms()
    loadCourses()
    if (courseId) loadClos()
  }, [loadPrograms, loadCourses, loadClos, courseId])

  const openCreate = useCallback(() => {
    if (!courseId) return
    setDialogMode('create')
    setEditingCloId(null)
    setForm({ ...emptyForm })
    setFormError('')
    setDialogOpen(true)
  }, [courseId])

  const openEdit = useCallback((row) => {
    setDialogMode('edit')
    setEditingCloId(row.id)
    setForm({
      code: row.code ?? '',
      description: row.description ?? '',
      orderIndex: Number(row.orderIndex) || 1,
    })
    setFormError('')
    setDialogOpen(true)
  }, [])

  const handleSubmit = async (e) => {
    e.preventDefault()
    const token = getAdminToken()
    if (!token || !courseId) return
    if (!form.code.trim() || !form.description.trim()) {
      setFormError('Kod ve açıklama zorunludur.')
      return
    }
    setSaving(true)
    setFormError('')
    try {
      if (dialogMode === 'create') {
        await createClo(token, courseId, {
          code: form.code.trim(),
          description: form.description.trim(),
          orderIndex: Number(form.orderIndex) || 1,
        })
      } else {
        await updateClo(token, courseId, editingCloId, {
          code: form.code.trim(),
          description: form.description.trim(),
          orderIndex: Number(form.orderIndex) || 1,
        })
      }
      setDialogOpen(false)
      loadClos()
    } catch (err) {
      setFormError(err instanceof Error ? err.message : 'Kayıt başarısız.')
    } finally {
      setSaving(false)
    }
  }

  const handleDelete = async () => {
    const token = getAdminToken()
    if (!token || !deleteTarget || !courseId) return
    setSaving(true)
    try {
      await deleteClo(token, courseId, deleteTarget.id)
      setDeleteTarget(null)
      loadClos()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Silinemedi.')
    } finally {
      setSaving(false)
    }
  }

  const columns = useMemo(
    () => [
      columnHelper.accessor('code', { header: 'Kod' }),
      columnHelper.accessor('description', {
        header: 'Açıklama',
        cell: (info) => {
          const v = info.getValue() ?? ''
          return v.length > 80 ? `${v.slice(0, 80)}…` : v
        },
      }),
      columnHelper.accessor('orderIndex', { header: 'Sıra' }),
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
              onClick={() => setDeleteTarget(row.original)}
            >
              <Trash2 size={16} aria-hidden />
            </button>
          </div>
        ),
      }),
    ],
    [openEdit],
  )

  const combinedError = [catalogError, error].filter(Boolean).join(' ')

  return (
    <PageSection title={page.title} description={page.description} error={combinedError || undefined}>
      <div className={pageStyles.filterRow}>
        <div className={pageStyles.filterRowFields}>
          <label htmlFor="clo-filter-program">
            Program filtresi
            <select
              id="clo-filter-program"
              className={sectionStyles.select}
              value={programId}
              onChange={(e) => {
                setProgramId(e.target.value)
                setCourseId('')
              }}
            >
              <option value="">Tümü</option>
              {programs.map((p) => (
                <option key={p.id} value={p.id}>
                  {p.name}
                </option>
              ))}
            </select>
          </label>
          <label htmlFor="clo-filter-course">
            Ders
            <select
              id="clo-filter-course"
              className={sectionStyles.select}
              value={courseId}
              onChange={(e) => setCourseId(e.target.value)}
              disabled={loadingCatalog}
            >
              <option value="">{loadingCatalog ? 'Yükleniyor…' : 'Seçin'}</option>
              {courses.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.code} — {c.name}
                </option>
              ))}
            </select>
          </label>
        </div>
        <RefreshIconButton onClick={handleRefresh} loading={loadingCatalog || loading} />
      </div>

      {!courseId ? (
        <p className={pageStyles.hint}>CLO listesi için yukarıdan bir ders seçin.</p>
      ) : null}

      {courseId ? (
        <DataTable
          columns={columns}
          data={rows}
          globalFilter={globalFilter}
          onGlobalFilterChange={setGlobalFilter}
          searchPlaceholder="Kod veya açıklamada ara…"
          toolbarExtra={
            <>
              <RefreshIconButton onClick={handleRefresh} loading={loadingCatalog || loading} />
              <button
                type="button"
                className={`${formStyles.btn} ${formStyles.btnPrimary}`}
                onClick={openCreate}
                disabled={!courseId}
              >
                <Plus size={18} aria-hidden />
                Yeni CLO
              </button>
            </>
          }
          isLoading={loading}
        />
      ) : null}

      <AppDialog
        open={dialogOpen}
        onClose={() => setDialogOpen(false)}
        title={dialogMode === 'create' ? 'Yeni CLO' : 'CLO düzenle'}
        size="md"
        footer={
          <>
            <button type="button" className={`${formStyles.btn} ${formStyles.btnGhost}`} onClick={() => setDialogOpen(false)}>
              Vazgeç
            </button>
            <button type="submit" form="clo-form" className={`${formStyles.btn} ${formStyles.btnPrimary}`} disabled={saving}>
              {saving ? 'Kaydediliyor…' : 'Kaydet'}
            </button>
          </>
        }
      >
        <form id="clo-form" className={formStyles.form} onSubmit={handleSubmit}>
          {formError ? (
            <p className={sectionStyles.error} role="alert">
              {formError}
            </p>
          ) : null}
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="clo-code">
              Kod
            </label>
            <input
              id="clo-code"
              className={formStyles.input}
              value={form.code}
              onChange={(e) => setForm((f) => ({ ...f, code: e.target.value }))}
              required
            />
          </div>
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="clo-desc">
              Açıklama
            </label>
            <textarea
              id="clo-desc"
              className={formStyles.textarea}
              value={form.description}
              onChange={(e) => setForm((f) => ({ ...f, description: e.target.value }))}
              required
            />
          </div>
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="clo-order">
              Sıra
            </label>
            <input
              id="clo-order"
              type="number"
              min={1}
              className={formStyles.input}
              value={form.orderIndex}
              onChange={(e) => setForm((f) => ({ ...f, orderIndex: Number(e.target.value) }))}
            />
          </div>
        </form>
      </AppDialog>

      <AppDialog
        open={Boolean(deleteTarget)}
        onClose={() => setDeleteTarget(null)}
        title="CLO silinsin mi?"
        footer={
          <>
            <button type="button" className={`${formStyles.btn} ${formStyles.btnGhost}`} onClick={() => setDeleteTarget(null)}>
              Vazgeç
            </button>
            <button type="button" className={`${formStyles.btn} ${formStyles.btnPrimary}`} onClick={handleDelete} disabled={saving}>
              {saving ? '…' : 'Sil'}
            </button>
          </>
        }
      >
        <p className={sectionStyles.muted}>
          <strong>{deleteTarget?.code}</strong> kalıcı olarak silinecek.
        </p>
      </AppDialog>
    </PageSection>
  )
}
