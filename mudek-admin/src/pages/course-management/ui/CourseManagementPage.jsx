import { createColumnHelper } from '@tanstack/react-table'
import { Pencil, Plus, Trash2 } from 'lucide-react'
import { useCallback, useEffect, useMemo, useState } from 'react'

import {
  createCourse,
  deleteCourse,
  fetchCourses,
  fetchPrograms,
  updateCourse,
} from '../../../shared/api/adminApi'
import { appConfig } from '../../../shared/config/appConfig'
import { getAdminToken } from '../../../shared/lib/authToken'
import formStyles from '../../../shared/ui/admin-form/AdminForm.module.css'
import { AdminSection } from '../../../shared/ui/admin-section/AdminSection.jsx'
import sectionStyles from '../../../shared/ui/admin-section/AdminSection.module.css'
import { DataTable } from '../../../shared/ui/data-table/DataTable.jsx'
import { AppDialog } from '../../../shared/ui/dialog/AppDialog.jsx'

const columnHelper = createColumnHelper()

const emptyCreate = {
  programEntityId: '',
  code: '',
  name: '',
  credits: 3,
  description: '',
  isActive: true,
}

export function CourseManagementPage() {
  const page = appConfig.pages.courseManagement
  const [programs, setPrograms] = useState([])
  const [programId, setProgramId] = useState('')
  const [rows, setRows] = useState([])
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(true)
  const [globalFilter, setGlobalFilter] = useState('')

  const [dialogOpen, setDialogOpen] = useState(false)
  const [dialogMode, setDialogMode] = useState('create')
  const [form, setForm] = useState(emptyCreate)
  const [editingId, setEditingId] = useState(null)
  const [formError, setFormError] = useState('')
  const [saving, setSaving] = useState(false)
  const [deleteTarget, setDeleteTarget] = useState(null)

  const loadPrograms = useCallback(() => {
    const token = getAdminToken()
    if (!token) return
    fetchPrograms(token).then(setPrograms).catch(() => {})
  }, [])

  const loadCourses = useCallback(() => {
    const token = getAdminToken()
    if (!token) return
    setLoading(true)
    setError('')
    fetchCourses(token, programId || undefined)
      .then((data) => setRows(Array.isArray(data) ? data : []))
      .catch((e) => setError(e instanceof Error ? e.message : 'Dersler alınamadı.'))
      .finally(() => setLoading(false))
  }, [programId])

  useEffect(() => {
    loadPrograms()
  }, [loadPrograms])

  useEffect(() => {
    loadCourses()
  }, [loadCourses])

  const openCreate = useCallback(() => {
    setDialogMode('create')
    setEditingId(null)
    setForm({
      ...emptyCreate,
      programEntityId: programId || (programs[0]?.id ?? ''),
    })
    setFormError('')
    setDialogOpen(true)
  }, [programId, programs])

  const openEdit = useCallback((row) => {
    setDialogMode('edit')
    setEditingId(row.id)
    setForm({
      programEntityId: row.programEntityId ?? '',
      code: row.code ?? '',
      name: row.name ?? '',
      credits: Number(row.credits) || 0,
      description: row.description ?? '',
      isActive: row.isActive !== false,
    })
    setFormError('')
    setDialogOpen(true)
  }, [])

  const handleSubmit = async (e) => {
    e.preventDefault()
    const token = getAdminToken()
    if (!token) return
    if (!form.code.trim() || !form.name.trim()) {
      setFormError('Kod ve ad zorunludur.')
      return
    }
    if (dialogMode === 'create' && !form.programEntityId) {
      setFormError('Program seçin.')
      return
    }
    setSaving(true)
    setFormError('')
    try {
      if (dialogMode === 'create') {
        await createCourse(token, {
          programEntityId: form.programEntityId,
          code: form.code.trim(),
          name: form.name.trim(),
          credits: Number(form.credits) || 0,
          description: form.description.trim() || undefined,
        })
      } else {
        await updateCourse(token, editingId, {
          code: form.code.trim(),
          name: form.name.trim(),
          credits: Number(form.credits) || 0,
          description: form.description.trim() || undefined,
          isActive: Boolean(form.isActive),
        })
      }
      setDialogOpen(false)
      loadCourses()
    } catch (err) {
      setFormError(err instanceof Error ? err.message : 'Kayıt başarısız.')
    } finally {
      setSaving(false)
    }
  }

  const handleDelete = async () => {
    const token = getAdminToken()
    if (!token || !deleteTarget) return
    setSaving(true)
    try {
      await deleteCourse(token, deleteTarget.id)
      setDeleteTarget(null)
      loadCourses()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Silinemedi.')
    } finally {
      setSaving(false)
    }
  }

  const columns = useMemo(
    () => [
      columnHelper.accessor('code', { header: 'Kod' }),
      columnHelper.accessor('name', { header: 'Ad' }),
      columnHelper.accessor('credits', { header: 'Kredi' }),
      columnHelper.accessor('programName', {
        header: 'Program',
        cell: (info) => info.getValue() ?? '—',
      }),
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
              title="Düzenle"
              onClick={() => openEdit(row.original)}
            >
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

  const toolbar = (
    <label>
      Program filtresi
      <select
        className={sectionStyles.select}
        value={programId}
        onChange={(e) => setProgramId(e.target.value)}
      >
        <option value="">Tümü</option>
        {programs.map((p) => (
          <option key={p.id} value={p.id}>
            {p.name}
          </option>
        ))}
      </select>
    </label>
  )

  return (
    <AdminSection title={page.title} description={page.description} toolbar={toolbar} error={error}>
      <DataTable
        columns={columns}
        data={rows}
        globalFilter={globalFilter}
        onGlobalFilterChange={setGlobalFilter}
        searchPlaceholder="Kod, ad veya program ara…"
        toolbarExtra={
          <button type="button" className={`${formStyles.btn} ${formStyles.btnPrimary}`} onClick={openCreate}>
            <Plus size={18} aria-hidden />
            Yeni ders
          </button>
        }
        isLoading={loading}
      />

      <AppDialog
        open={dialogOpen}
        onClose={() => setDialogOpen(false)}
        title={dialogMode === 'create' ? 'Yeni ders' : 'Dersi düzenle'}
        size="lg"
        footer={
          <>
            <button
              type="button"
              className={`${formStyles.btn} ${formStyles.btnGhost}`}
              onClick={() => setDialogOpen(false)}
            >
              Vazgeç
            </button>
            <button
              type="submit"
              form="course-form"
              className={`${formStyles.btn} ${formStyles.btnPrimary}`}
              disabled={saving}
            >
              {saving ? 'Kaydediliyor…' : 'Kaydet'}
            </button>
          </>
        }
      >
        <form id="course-form" className={formStyles.form} onSubmit={handleSubmit}>
          {formError ? (
            <p className={sectionStyles.error} role="alert">
              {formError}
            </p>
          ) : null}
          {dialogMode === 'create' ? (
            <div className={formStyles.field}>
              <label className={formStyles.label} htmlFor="c-prog">
                Program
              </label>
              <select
                id="c-prog"
                className={formStyles.select}
                value={form.programEntityId}
                onChange={(e) => setForm((f) => ({ ...f, programEntityId: e.target.value }))}
                required
              >
                <option value="">Seçin</option>
                {programs.map((p) => (
                  <option key={p.id} value={p.id}>
                    {p.name}
                  </option>
                ))}
              </select>
            </div>
          ) : null}
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="c-code">
              Ders kodu
            </label>
            <input
              id="c-code"
              className={formStyles.input}
              value={form.code}
              onChange={(e) => setForm((f) => ({ ...f, code: e.target.value }))}
              required
            />
          </div>
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="c-name">
              Ad
            </label>
            <input
              id="c-name"
              className={formStyles.input}
              value={form.name}
              onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))}
              required
            />
          </div>
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="c-cr">
              AKTS / kredi
            </label>
            <input
              id="c-cr"
              type="number"
              min={0}
              className={formStyles.input}
              value={form.credits}
              onChange={(e) => setForm((f) => ({ ...f, credits: Number(e.target.value) }))}
            />
          </div>
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="c-desc">
              Açıklama
            </label>
            <textarea
              id="c-desc"
              className={formStyles.textarea}
              value={form.description}
              onChange={(e) => setForm((f) => ({ ...f, description: e.target.value }))}
            />
          </div>
          {dialogMode === 'edit' ? (
            <div className={formStyles.checkRow}>
              <input
                id="c-act"
                type="checkbox"
                className={formStyles.check}
                checked={Boolean(form.isActive)}
                onChange={(e) => setForm((f) => ({ ...f, isActive: e.target.checked }))}
              />
              <label htmlFor="c-act">Ders aktif</label>
            </div>
          ) : null}
        </form>
      </AppDialog>

      <AppDialog
        open={Boolean(deleteTarget)}
        onClose={() => setDeleteTarget(null)}
        title="Dersi sil"
        size="sm"
        footer={
          <>
            <button
              type="button"
              className={`${formStyles.btn} ${formStyles.btnGhost}`}
              onClick={() => setDeleteTarget(null)}
            >
              İptal
            </button>
            <button
              type="button"
              className={`${formStyles.btn} ${formStyles.btnDanger}`}
              disabled={saving}
              onClick={handleDelete}
            >
              {saving ? 'Siliniyor…' : 'Sil'}
            </button>
          </>
        }
      >
        <p className={sectionStyles.muted}>
          <strong>{deleteTarget?.code}</strong> — {deleteTarget?.name} silinsin mi?
        </p>
      </AppDialog>
    </AdminSection>
  )
}
