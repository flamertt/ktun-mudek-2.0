import { createColumnHelper } from '@tanstack/react-table'
import { Pencil, Plus, Trash2, UserMinus, UserPlus } from 'lucide-react'
import { useCallback, useEffect, useMemo, useState } from 'react'

import {
  assignTeacherToCourseOffering,
  createCourseOffering,
  deleteCourseOffering,
  fetchAcademicTerms,
  fetchAllCourseOfferings,
  fetchCourseOfferingsByTerm,
  fetchCourses,
  fetchTeachers,
  removeTeacherFromCourseOffering,
  updateCourseOffering,
} from '../../../shared/api/adminApi'
import { appConfig } from '../../../shared/config/appConfig'
import { getAdminToken } from '../../../shared/lib/authToken'
import formStyles from '../../../shared/ui/admin-form/AdminForm.module.css'
import { RefreshIconButton } from '../../../shared/ui/refresh-icon-button/RefreshIconButton.jsx'
import { DataTable } from '../../../shared/ui/data-table/DataTable.jsx'
import { AppDialog } from '../../../shared/ui/dialog/AppDialog.jsx'
import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import sectionStyles from '@shared/ui/page-section/PageSection.module.css'

const columnHelper = createColumnHelper()

const emptyOffering = {
  courseId: '',
  academicTermId: '',
  teacherId: '',
  section: 'A',
  passingGrade: '',
  quota: '',
  isActive: true,
}

export function OfferingManagementPage() {
  const page = appConfig.pages.offeringManagement
  const [terms, setTerms] = useState([])
  const [courses, setCourses] = useState([])
  const [teachers, setTeachers] = useState([])

  const [termFilter, setTermFilter] = useState('')
  const [rows, setRows] = useState([])
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)
  const [globalFilter, setGlobalFilter] = useState('')

  const [createOpen, setCreateOpen] = useState(false)
  const [createForm, setCreateForm] = useState(emptyOffering)
  const [formError, setFormError] = useState('')
  const [saving, setSaving] = useState(false)

  const [editRow, setEditRow] = useState(null)
  const [editForm, setEditForm] = useState({
    section: 'A',
    passingGrade: '',
    quota: '',
    isActive: true,
  })

  const [assignRow, setAssignRow] = useState(null)
  const [assignTeacherId, setAssignTeacherId] = useState('')

  const [deleteTarget, setDeleteTarget] = useState(null)

  const loadMeta = useCallback(() => {
    const token = getAdminToken()
    if (!token) return
    Promise.all([fetchAcademicTerms(token), fetchCourses(token), fetchTeachers(token)])
      .then(([t, c, th]) => {
        setTerms(Array.isArray(t) ? t : [])
        setCourses(Array.isArray(c) ? c : [])
        setTeachers(Array.isArray(th) ? th : [])
      })
      .catch(() => {})
  }, [])

  const loadOfferings = useCallback(() => {
    const token = getAdminToken()
    if (!token) return
    setLoading(true)
    setError('')
    const p = termFilter
      ? fetchCourseOfferingsByTerm(token, termFilter)
      : fetchAllCourseOfferings(token)
    p.then((data) => setRows(Array.isArray(data) ? data : []))
      .catch((e) => setError(e instanceof Error ? e.message : 'Liste alınamadı.'))
      .finally(() => setLoading(false))
  }, [termFilter])

  useEffect(() => {
    loadMeta()
  }, [loadMeta])

  useEffect(() => {
    loadOfferings()
  }, [loadOfferings])

  const handleRefresh = useCallback(() => {
    loadMeta()
    loadOfferings()
  }, [loadMeta, loadOfferings])

  const openCreate = useCallback(() => {
    const firstTerm = terms[0]?.id ?? ''
    const firstCourse = courses[0]?.id ?? ''
    setCreateForm({
      ...emptyOffering,
      academicTermId: termFilter || firstTerm,
      courseId: firstCourse,
    })
    setFormError('')
    setCreateOpen(true)
  }, [termFilter, terms, courses])

  const handleCreate = async (e) => {
    e.preventDefault()
    const token = getAdminToken()
    if (!token) return
    if (!createForm.courseId || !createForm.academicTermId) {
      setFormError('Ders ve dönem seçin.')
      return
    }
    setSaving(true)
    setFormError('')
    try {
      const body = {
        courseId: createForm.courseId,
        academicTermId: createForm.academicTermId,
        section: createForm.section?.trim() || 'A',
        isActive: createForm.isActive,
      }
      if (createForm.teacherId) body.teacherId = createForm.teacherId
      if (createForm.passingGrade !== '' && createForm.passingGrade != null) {
        body.passingGrade = Number(createForm.passingGrade)
      }
      if (createForm.quota !== '' && createForm.quota != null) {
        body.quota = Number(createForm.quota)
      }
      await createCourseOffering(token, body)
      setCreateOpen(false)
      loadOfferings()
    } catch (err) {
      setFormError(err instanceof Error ? err.message : 'Oluşturulamadı.')
    } finally {
      setSaving(false)
    }
  }

  const openEdit = useCallback((row) => {
    setEditRow(row)
    setEditForm({
      section: row.section ?? 'A',
      passingGrade: row.passingGrade != null ? String(row.passingGrade) : '',
      quota: row.quota != null ? String(row.quota) : '',
      isActive: row.isActive !== false,
    })
  }, [])

  const handleEditSave = async (e) => {
    e.preventDefault()
    const token = getAdminToken()
    if (!token || !editRow) return
    setSaving(true)
    try {
      const body = {
        id: editRow.id,
        section: editForm.section?.trim() || 'A',
        isActive: editForm.isActive,
      }
      if (editForm.passingGrade !== '') body.passingGrade = Number(editForm.passingGrade)
      if (editForm.quota !== '') body.quota = Number(editForm.quota)
      await updateCourseOffering(token, editRow.id, body)
      setEditRow(null)
      loadOfferings()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Güncellenemedi.')
    } finally {
      setSaving(false)
    }
  }

  const openAssign = useCallback((row) => {
    setAssignRow(row)
    setAssignTeacherId(row.teacherId ?? '')
  }, [])

  const handleAssign = async () => {
    const token = getAdminToken()
    if (!token || !assignRow || !assignTeacherId) return
    setSaving(true)
    try {
      await assignTeacherToCourseOffering(token, assignRow.id, { teacherId: assignTeacherId })
      setAssignRow(null)
      loadOfferings()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Atama başarısız.')
    } finally {
      setSaving(false)
    }
  }

  const handleDelete = async () => {
    const token = getAdminToken()
    if (!token || !deleteTarget) return
    setSaving(true)
    try {
      await deleteCourseOffering(token, deleteTarget.id)
      setDeleteTarget(null)
      loadOfferings()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Silinemedi.')
    } finally {
      setSaving(false)
    }
  }

  const handleRemoveTeacher = useCallback(
    async (row) => {
      const token = getAdminToken()
      if (!token) return
      setSaving(true)
      try {
        await removeTeacherFromCourseOffering(token, row.id)
        loadOfferings()
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Öğretmen kaldırılamadı.')
      } finally {
        setSaving(false)
      }
    },
    [loadOfferings],
  )

  const columns = useMemo(
    () => [
      columnHelper.accessor('courseCode', { header: 'Ders kodu' }),
      columnHelper.accessor('courseName', {
        header: 'Ders',
        cell: (info) => {
          const v = info.getValue() ?? ''
          return v.length > 40 ? `${v.slice(0, 40)}…` : v
        },
      }),
      columnHelper.accessor('termName', { header: 'Dönem' }),
      columnHelper.accessor('section', { header: 'Şube' }),
      columnHelper.accessor('teacherName', {
        header: 'Öğretmen',
        cell: (info) => info.getValue() ?? '—',
      }),
      columnHelper.accessor('enrolledCount', { header: 'Kayıtlı' }),
      columnHelper.accessor('isActive', {
        header: 'Aktif',
        cell: (info) => (info.getValue() ? 'Evet' : 'Hayır'),
      }),
      columnHelper.display({
        id: 'actions',
        header: 'İşlemler',
        cell: ({ row }) => (
          <div className={formStyles.actions}>
            <button type="button" className={formStyles.actionIcon} title="Düzenle" onClick={() => openEdit(row.original)}>
              <Pencil size={16} aria-hidden />
            </button>
            <button type="button" className={formStyles.actionIcon} title="Öğretmen ata" onClick={() => openAssign(row.original)}>
              <UserPlus size={16} aria-hidden />
            </button>
            {row.original.teacherId ? (
              <button
                type="button"
                className={formStyles.actionIcon}
                title="Öğretmeni kaldır"
                onClick={() => void handleRemoveTeacher(row.original)}
              >
                <UserMinus size={16} aria-hidden />
              </button>
            ) : null}
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
    [openAssign, openEdit, handleRemoveTeacher],
  )

  const toolbarFilters = (
    <label>
      Dönem filtresi
      <select className={sectionStyles.select} value={termFilter} onChange={(e) => setTermFilter(e.target.value)}>
        <option value="">Tümü</option>
        {terms.map((t) => (
          <option key={t.id} value={t.id}>
            {t.name}
          </option>
        ))}
      </select>
    </label>
  )

  return (
    <PageSection title={page.title} description={page.description} error={error}>
      <DataTable
        columns={columns}
        data={rows}
        globalFilter={globalFilter}
        onGlobalFilterChange={setGlobalFilter}
        searchPlaceholder="Ders, dönem veya öğretmende ara…"
        toolbarFilters={toolbarFilters}
        toolbarExtra={
          <>
            <RefreshIconButton onClick={handleRefresh} loading={loading} />
            <button type="button" className={`${formStyles.btn} ${formStyles.btnPrimary}`} onClick={openCreate}>
              <Plus size={18} aria-hidden />
              Yeni açılış
            </button>
          </>
        }
        isLoading={loading}
      />

      <AppDialog
        open={createOpen}
        onClose={() => setCreateOpen(false)}
        title="Ders açılışı oluştur"
        size="md"
        footer={
          <>
            <button type="button" className={`${formStyles.btn} ${formStyles.btnGhost}`} onClick={() => setCreateOpen(false)}>
              Vazgeç
            </button>
            <button type="submit" form="offering-create" className={`${formStyles.btn} ${formStyles.btnPrimary}`} disabled={saving}>
              {saving ? 'Kaydediliyor…' : 'Oluştur'}
            </button>
          </>
        }
      >
        <form id="offering-create" className={formStyles.form} onSubmit={handleCreate}>
          {formError ? (
            <p className={sectionStyles.error} role="alert">
              {formError}
            </p>
          ) : null}
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="of-course">
              Ders
            </label>
            <select
              id="of-course"
              className={formStyles.input}
              value={createForm.courseId}
              onChange={(e) => setCreateForm((f) => ({ ...f, courseId: e.target.value }))}
              required
            >
              <option value="">Seçin</option>
              {courses.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.code} — {c.name}
                </option>
              ))}
            </select>
          </div>
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="of-term">
              Akademik dönem
            </label>
            <select
              id="of-term"
              className={formStyles.input}
              value={createForm.academicTermId}
              onChange={(e) => setCreateForm((f) => ({ ...f, academicTermId: e.target.value }))}
              required
            >
              <option value="">Seçin</option>
              {terms.map((t) => (
                <option key={t.id} value={t.id}>
                  {t.name}
                </option>
              ))}
            </select>
          </div>
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="of-teacher">
              Öğretmen (isteğe bağlı)
            </label>
            <select
              id="of-teacher"
              className={formStyles.input}
              value={createForm.teacherId}
              onChange={(e) => setCreateForm((f) => ({ ...f, teacherId: e.target.value }))}
            >
              <option value="">Atanmasın</option>
              {teachers.map((th) => (
                <option key={th.id} value={th.id}>
                  {th.fullName ?? th.email}
                </option>
              ))}
            </select>
          </div>
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="of-section">
              Şube
            </label>
            <input
              id="of-section"
              className={formStyles.input}
              value={createForm.section}
              onChange={(e) => setCreateForm((f) => ({ ...f, section: e.target.value }))}
            />
          </div>
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="of-pass">
              Geçme notu
            </label>
            <input
              id="of-pass"
              type="number"
              step="0.01"
              className={formStyles.input}
              value={createForm.passingGrade}
              onChange={(e) => setCreateForm((f) => ({ ...f, passingGrade: e.target.value }))}
            />
          </div>
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="of-quota">
              Kontenjan
            </label>
            <input
              id="of-quota"
              type="number"
              min={0}
              className={formStyles.input}
              value={createForm.quota}
              onChange={(e) => setCreateForm((f) => ({ ...f, quota: e.target.value }))}
            />
          </div>
          <label className={formStyles.checkRow}>
            <input
              className={formStyles.check}
              type="checkbox"
              checked={createForm.isActive}
              onChange={(e) => setCreateForm((f) => ({ ...f, isActive: e.target.checked }))}
            />
            Aktif
          </label>
        </form>
      </AppDialog>

      <AppDialog
        open={Boolean(editRow)}
        onClose={() => setEditRow(null)}
        title="Açılışı düzenle"
        footer={
          <>
            <button type="button" className={`${formStyles.btn} ${formStyles.btnGhost}`} onClick={() => setEditRow(null)}>
              Vazgeç
            </button>
            <button type="submit" form="offering-edit" className={`${formStyles.btn} ${formStyles.btnPrimary}`} disabled={saving}>
              {saving ? '…' : 'Kaydet'}
            </button>
          </>
        }
      >
        <form id="offering-edit" className={formStyles.form} onSubmit={handleEditSave}>
          <p className={sectionStyles.muted}>
            {editRow?.courseCode} — {editRow?.termName}
          </p>
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="ed-section">
              Şube
            </label>
            <input
              id="ed-section"
              className={formStyles.input}
              value={editForm.section}
              onChange={(e) => setEditForm((f) => ({ ...f, section: e.target.value }))}
            />
          </div>
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="ed-pass">
              Geçme notu
            </label>
            <input
              id="ed-pass"
              type="number"
              step="0.01"
              className={formStyles.input}
              value={editForm.passingGrade}
              onChange={(e) => setEditForm((f) => ({ ...f, passingGrade: e.target.value }))}
            />
          </div>
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="ed-quota">
              Kontenjan
            </label>
            <input
              id="ed-quota"
              type="number"
              min={0}
              className={formStyles.input}
              value={editForm.quota}
              onChange={(e) => setEditForm((f) => ({ ...f, quota: e.target.value }))}
            />
          </div>
          <label className={formStyles.checkRow}>
            <input
              className={formStyles.check}
              type="checkbox"
              checked={editForm.isActive}
              onChange={(e) => setEditForm((f) => ({ ...f, isActive: e.target.checked }))}
            />
            Aktif
          </label>
        </form>
      </AppDialog>

      <AppDialog
        open={Boolean(assignRow)}
        onClose={() => setAssignRow(null)}
        title="Öğretmen ata"
        footer={
          <>
            <button type="button" className={`${formStyles.btn} ${formStyles.btnGhost}`} onClick={() => setAssignRow(null)}>
              Vazgeç
            </button>
            <button type="button" className={`${formStyles.btn} ${formStyles.btnPrimary}`} onClick={handleAssign} disabled={saving || !assignTeacherId}>
              {saving ? '…' : 'Ata'}
            </button>
          </>
        }
      >
        <div className={formStyles.form}>
          <p className={sectionStyles.muted}>
            {assignRow?.courseCode} — {assignRow?.termName}
          </p>
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="assign-t">
              Öğretmen
            </label>
            <select
              id="assign-t"
              className={formStyles.input}
              value={assignTeacherId}
              onChange={(e) => setAssignTeacherId(e.target.value)}
            >
              <option value="">Seçin</option>
              {teachers.map((th) => (
                <option key={th.id} value={th.id}>
                  {th.fullName ?? th.email}
                </option>
              ))}
            </select>
          </div>
        </div>
      </AppDialog>

      <AppDialog
        open={Boolean(deleteTarget)}
        onClose={() => setDeleteTarget(null)}
        title="Açılış silinsin mi?"
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
          <strong>{deleteTarget?.courseCode}</strong> / {deleteTarget?.termName} — bu işlem geri alınamaz.
        </p>
      </AppDialog>
    </PageSection>
  )
}
