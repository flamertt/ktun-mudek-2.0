import { createColumnHelper } from '@tanstack/react-table'
import { Pencil, Plus, Trash2 } from 'lucide-react'
import { useCallback, useEffect, useMemo, useState } from 'react'

import {
  createStudent,
  deleteStudent,
  fetchPrograms,
  fetchStudents,
  updateStudent,
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
  fullName: '',
  email: '',
  password: '',
  studentNumber: '',
  phoneNumber: '',
  programEntityId: '',
  isActive: true,
}

export function StudentManagementPage() {
  const page = appConfig.pages.studentManagement
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

  const loadStudents = useCallback(() => {
    const token = getAdminToken()
    if (!token) return
    setLoading(true)
    setError('')
    fetchStudents(token, programId || undefined)
      .then((data) => setRows(Array.isArray(data) ? data : []))
      .catch((e) => setError(e instanceof Error ? e.message : 'Liste alınamadı.'))
      .finally(() => setLoading(false))
  }, [programId])

  useEffect(() => {
    loadPrograms()
  }, [loadPrograms])

  useEffect(() => {
    loadStudents()
  }, [loadStudents])

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
      fullName: row.fullName ?? '',
      email: row.email ?? '',
      password: '',
      studentNumber: row.studentNumber ?? '',
      phoneNumber: row.phoneNumber ?? '',
      programEntityId: row.programEntityId ?? '',
      isActive: row.isActive !== false,
    })
    setFormError('')
    setDialogOpen(true)
  }, [])

  const handleSubmit = async (e) => {
    e.preventDefault()
    const token = getAdminToken()
    if (!token) return
    if (!form.fullName.trim() || !form.email.trim()) {
      setFormError('Ad ve e-posta zorunludur.')
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
        await createStudent(token, {
          fullName: form.fullName.trim(),
          email: form.email.trim(),
          password: form.password.trim() || undefined,
          studentNumber: form.studentNumber.trim() || undefined,
          phoneNumber: form.phoneNumber.trim() || undefined,
          programEntityId: form.programEntityId,
        })
      } else {
        await updateStudent(token, editingId, {
          fullName: form.fullName.trim(),
          email: form.email.trim(),
          studentNumber: form.studentNumber.trim() || undefined,
          phoneNumber: form.phoneNumber.trim() || undefined,
          programEntityId: form.programEntityId || undefined,
          isActive: Boolean(form.isActive),
        })
      }
      setDialogOpen(false)
      loadStudents()
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
      await deleteStudent(token, deleteTarget.id)
      setDeleteTarget(null)
      loadStudents()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Silinemedi.')
    } finally {
      setSaving(false)
    }
  }

  const columns = useMemo(
    () => [
      columnHelper.accessor('fullName', { header: 'Ad Soyad' }),
      columnHelper.accessor('studentNumber', {
        header: 'Öğrenci no',
        cell: (info) => info.getValue() ?? '—',
      }),
      columnHelper.accessor('email', { header: 'E-posta' }),
      columnHelper.accessor('programName', {
        header: 'Program',
        cell: (info) => info.getValue() ?? '—',
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
        searchPlaceholder="Ad, numara veya e-posta ara…"
        toolbarExtra={
          <button type="button" className={`${formStyles.btn} ${formStyles.btnPrimary}`} onClick={openCreate}>
            <Plus size={18} aria-hidden />
            Yeni öğrenci
          </button>
        }
        isLoading={loading}
      />

      <AppDialog
        open={dialogOpen}
        onClose={() => setDialogOpen(false)}
        title={dialogMode === 'create' ? 'Yeni öğrenci' : 'Öğrenciyi düzenle'}
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
              form="student-form"
              className={`${formStyles.btn} ${formStyles.btnPrimary}`}
              disabled={saving}
            >
              {saving ? 'Kaydediliyor…' : 'Kaydet'}
            </button>
          </>
        }
      >
        <form id="student-form" className={formStyles.form} onSubmit={handleSubmit}>
          {formError ? (
            <p className={sectionStyles.error} role="alert">
              {formError}
            </p>
          ) : null}
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="s-fullName">
              Ad soyad
            </label>
            <input
              id="s-fullName"
              className={formStyles.input}
              value={form.fullName}
              onChange={(e) => setForm((f) => ({ ...f, fullName: e.target.value }))}
              required
            />
          </div>
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="s-email">
              E-posta
            </label>
            <input
              id="s-email"
              type="email"
              className={formStyles.input}
              value={form.email}
              onChange={(e) => setForm((f) => ({ ...f, email: e.target.value }))}
              required
            />
          </div>
          {dialogMode === 'create' ? (
            <div className={formStyles.field}>
              <label className={formStyles.label} htmlFor="s-pass">
                Şifre (opsiyonel)
              </label>
              <input
                id="s-pass"
                type="password"
                className={formStyles.input}
                value={form.password}
                onChange={(e) => setForm((f) => ({ ...f, password: e.target.value }))}
                autoComplete="new-password"
              />
            </div>
          ) : null}
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="s-num">
              Öğrenci numarası
            </label>
            <input
              id="s-num"
              className={formStyles.input}
              value={form.studentNumber}
              onChange={(e) => setForm((f) => ({ ...f, studentNumber: e.target.value }))}
            />
          </div>
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="s-phone">
              Telefon
            </label>
            <input
              id="s-phone"
              className={formStyles.input}
              value={form.phoneNumber}
              onChange={(e) => setForm((f) => ({ ...f, phoneNumber: e.target.value }))}
            />
          </div>
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="s-prog">
              Program
            </label>
            <select
              id="s-prog"
              className={formStyles.select}
              value={form.programEntityId}
              onChange={(e) => setForm((f) => ({ ...f, programEntityId: e.target.value }))}
              required={dialogMode === 'create'}
            >
              <option value="">Seçin</option>
              {programs.map((p) => (
                <option key={p.id} value={p.id}>
                  {p.name}
                </option>
              ))}
            </select>
          </div>
          {dialogMode === 'edit' ? (
            <div className={formStyles.checkRow}>
              <input
                id="s-active"
                type="checkbox"
                className={formStyles.check}
                checked={Boolean(form.isActive)}
                onChange={(e) => setForm((f) => ({ ...f, isActive: e.target.checked }))}
              />
              <label htmlFor="s-active">Hesap aktif</label>
            </div>
          ) : null}
        </form>
      </AppDialog>

      <AppDialog
        open={Boolean(deleteTarget)}
        onClose={() => setDeleteTarget(null)}
        title="Öğrenciyi sil"
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
          <strong>{deleteTarget?.fullName}</strong> kalıcı olarak silinsin mi?
        </p>
      </AppDialog>
    </AdminSection>
  )
}
