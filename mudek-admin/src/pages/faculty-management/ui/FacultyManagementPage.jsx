import { createColumnHelper } from '@tanstack/react-table'
import { Pencil, Plus, Trash2 } from 'lucide-react'
import { useCallback, useEffect, useMemo, useState } from 'react'

import {
  createAcademicTerm,
  deleteAcademicTerm,
  fetchAcademicTerms,
  fetchActiveAcademicTerm,
  setActiveAcademicTerm,
  updateAcademicTerm,
} from '../../../shared/api/adminApi'
import { appConfig } from '../../../shared/config/appConfig'
import { getAdminToken } from '../../../shared/lib/authToken'
import formStyles from '../../../shared/ui/admin-form/AdminForm.module.css'
import { RefreshIconButton } from '../../../shared/ui/refresh-icon-button/RefreshIconButton.jsx'
import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import sectionStyles from '@shared/ui/page-section/PageSection.module.css'
import { DataTable } from '../../../shared/ui/data-table/DataTable.jsx'
import { AppDialog } from '../../../shared/ui/dialog/AppDialog.jsx'
import { DEFAULT_TERM_TYPE, TERM_TYPE_OPTIONS, getTermTypeLabel } from '../../../shared/lib/texts/academicTermTypes.js'

const columnHelper = createColumnHelper()

const emptyTerm = {
  startYear: new Date().getFullYear(),
  endYear: new Date().getFullYear() + 1,
  termType: DEFAULT_TERM_TYPE,
  name: '',
}

export function FacultyManagementPage() {
  const page = appConfig.pages.facultyManagement
  const [terms, setTerms] = useState([])
  const [active, setActive] = useState(null)
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(true)
  const [actionError, setActionError] = useState('')
  const [settingId, setSettingId] = useState(null)
  const [globalFilter, setGlobalFilter] = useState('')

  const [dialogOpen, setDialogOpen] = useState(false)
  const [dialogMode, setDialogMode] = useState('create')
  const [form, setForm] = useState(emptyTerm)
  const [editingId, setEditingId] = useState(null)
  const [formError, setFormError] = useState('')
  const [saving, setSaving] = useState(false)
  const [deleteTarget, setDeleteTarget] = useState(null)

  const load = useCallback(async () => {
    const token = getAdminToken()
    if (!token) return
    setLoading(true)
    setError('')
    try {
      const [list, current] = await Promise.all([
        fetchAcademicTerms(token),
        fetchActiveAcademicTerm(token).catch(() => null),
      ])
      setTerms(Array.isArray(list) ? list : [])
      setActive(current)
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Dönemler yüklenemedi.')
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => {
    load()
  }, [load])

  const handleSetActive = useCallback(
    async (id) => {
      const token = getAdminToken()
      if (!token) return
      setActionError('')
      setSettingId(id)
      try {
        await setActiveAcademicTerm(token, id)
        await load()
      } catch (e) {
        setActionError(e instanceof Error ? e.message : 'Aktif dönem güncellenemedi.')
      } finally {
        setSettingId(null)
      }
    },
    [load],
  )

  const openCreate = useCallback(() => {
    setDialogMode('create')
    setEditingId(null)
    const y = new Date().getFullYear()
    setForm({ ...emptyTerm, startYear: y, endYear: y + 1 })
    setFormError('')
    setDialogOpen(true)
  }, [])

  const openEdit = useCallback((row) => {
    setDialogMode('edit')
    setEditingId(row.id)
    setForm({
      startYear: Number(row.startYear) || new Date().getFullYear(),
      endYear: Number(row.endYear) || new Date().getFullYear() + 1,
      termType: row.termType || 'Guz',
      name: row.name ?? '',
    })
    setFormError('')
    setDialogOpen(true)
  }, [])

  const handleSubmit = async (e) => {
    e.preventDefault()
    const token = getAdminToken()
    if (!token) return
    if (form.startYear > form.endYear) {
      setFormError('Başlangıç yılı bitiş yılından büyük olamaz.')
      return
    }
    setSaving(true)
    setFormError('')
    try {
      const nameTrim = form.name.trim()
      if (dialogMode === 'create') {
        await createAcademicTerm(token, {
          startYear: Number(form.startYear),
          endYear: Number(form.endYear),
          termType: form.termType,
          name: nameTrim || undefined,
        })
      } else {
        await updateAcademicTerm(token, editingId, {
          startYear: Number(form.startYear),
          endYear: Number(form.endYear),
          termType: form.termType,
          name: nameTrim || undefined,
        })
      }
      setDialogOpen(false)
      await load()
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
      await deleteAcademicTerm(token, deleteTarget.id)
      setDeleteTarget(null)
      await load()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Silinemedi.')
    } finally {
      setSaving(false)
    }
  }

  const columns = useMemo(
    () => [
      columnHelper.accessor('name', { header: 'Dönem' }),
      columnHelper.display({
        id: 'years',
        header: 'Yıl',
        cell: ({ row }) => (
          <span>
            {row.original.startYear}–{row.original.endYear}
          </span>
        ),
      }),
      columnHelper.accessor('termType', {
        header: 'Tür',
        cell: (info) => {
          const v = info.getValue()
          return getTermTypeLabel(v)
        },
      }),
      columnHelper.accessor('isActive', {
        header: 'Durum',
        cell: (info) =>
          info.getValue() ? <span className={sectionStyles.badge}>Aktif</span> : '—',
      }),
      columnHelper.display({
        id: 'actions',
        header: 'İşlemler',
        cell: ({ row }) => {
          const t = row.original
          return (
            <div className={formStyles.actions}>
              {!t.isActive ? (
                <button
                  type="button"
                  className={sectionStyles.linkButton}
                  disabled={settingId === t.id}
                  onClick={() => handleSetActive(t.id)}
                  title="Aktif dönem yap"
                >
                  {settingId === t.id ? '…' : 'Aktif yap'}
                </button>
              ) : null}
              <button
                type="button"
                className={formStyles.actionIcon}
                title="Düzenle"
                onClick={() => openEdit(t)}
              >
                <Pencil size={16} aria-hidden />
              </button>
              <button
                type="button"
                className={`${formStyles.actionIcon} ${formStyles.actionDanger}`}
                title="Sil"
                onClick={() => setDeleteTarget(t)}
              >
                <Trash2 size={16} aria-hidden />
              </button>
            </div>
          )
        },
      }),
    ],
    [handleSetActive, openEdit, settingId],
  )

  return (
    <PageSection title={page.title} description={page.description} error={error}>
      {actionError ? (
        <p className={sectionStyles.error} role="alert">
          {actionError}
        </p>
      ) : null}

      {active ? (
        <p className={sectionStyles.muted}>
          Şu an aktif dönem: <strong>{active.name}</strong> ({active.termType})
        </p>
      ) : (
        <p className={sectionStyles.muted}>Aktif dönem atanmamış olabilir.</p>
      )}

      <DataTable
        columns={columns}
        data={terms}
        globalFilter={globalFilter}
        onGlobalFilterChange={setGlobalFilter}
        searchPlaceholder="Dönem adı veya yıl ara…"
        toolbarExtra={
          <>
            <RefreshIconButton onClick={load} loading={loading} />
            <button type="button" className={`${formStyles.btn} ${formStyles.btnPrimary}`} onClick={openCreate}>
              <Plus size={18} aria-hidden />
              Yeni dönem
            </button>
          </>
        }
        isLoading={loading}
      />

      <AppDialog
        open={dialogOpen}
        onClose={() => setDialogOpen(false)}
        title={dialogMode === 'create' ? 'Yeni akademik dönem' : 'Dönemi düzenle'}
        size="md"
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
              form="term-form"
              className={`${formStyles.btn} ${formStyles.btnPrimary}`}
              disabled={saving}
            >
              {saving ? 'Kaydediliyor…' : 'Kaydet'}
            </button>
          </>
        }
      >
        <form id="term-form" className={formStyles.form} onSubmit={handleSubmit}>
          {formError ? (
            <p className={sectionStyles.error} role="alert">
              {formError}
            </p>
          ) : null}
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="term-sy">
              Başlangıç yılı
            </label>
            <input
              id="term-sy"
              type="number"
              className={formStyles.input}
              value={form.startYear}
              onChange={(e) => setForm((f) => ({ ...f, startYear: Number(e.target.value) }))}
              required
            />
          </div>
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="term-ey">
              Bitiş yılı
            </label>
            <input
              id="term-ey"
              type="number"
              className={formStyles.input}
              value={form.endYear}
              onChange={(e) => setForm((f) => ({ ...f, endYear: Number(e.target.value) }))}
              required
            />
          </div>
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="term-tt">
              Dönem türü
            </label>
            <select
              id="term-tt"
              className={formStyles.select}
              value={form.termType}
              onChange={(e) => setForm((f) => ({ ...f, termType: e.target.value }))}
            >
              {TERM_TYPE_OPTIONS.map((o) => (
                <option key={o.value} value={o.value}>
                  {o.label}
                </option>
              ))}
            </select>
          </div>
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="term-nm">
              Ad (isteğe bağlı)
            </label>
            <input
              id="term-nm"
              className={formStyles.input}
              value={form.name}
              onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))}
              placeholder="Boş bırakılırsa otomatik oluşturulur"
            />
          </div>
        </form>
      </AppDialog>

      <AppDialog
        open={Boolean(deleteTarget)}
        onClose={() => setDeleteTarget(null)}
        title="Dönemi sil"
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
          <strong>{deleteTarget?.name}</strong> silinsin mi? Aktif dönem veya kayıtlı açılışlar varsa işlem
          reddedilebilir.
        </p>
      </AppDialog>
    </PageSection>
  )
}
