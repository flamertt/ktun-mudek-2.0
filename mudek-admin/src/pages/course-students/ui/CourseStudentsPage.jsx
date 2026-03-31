import { createColumnHelper } from '@tanstack/react-table'
import { Pencil, Plus, Trash2 } from 'lucide-react'
import { useCallback, useEffect, useMemo, useState } from 'react'

import {
  enrollStudentInOffering,
  fetchCourseById,
  fetchCourseOfferingsActiveTerm,
  fetchEnrolledStudents,
  fetchStudents,
  removeEnrollment,
  updateEnrollmentStatus,
} from '../../../shared/api/adminApi'
import { appConfig } from '../../../shared/config/appConfig'
import { getAdminToken } from '../../../shared/lib/authToken'
import formStyles from '../../../shared/ui/admin-form/AdminForm.module.css'
import { RefreshIconButton } from '../../../shared/ui/refresh-icon-button/RefreshIconButton.jsx'
import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import sectionStyles from '@shared/ui/page-section/PageSection.module.css'
import { PersonNameCell } from '@shared/ui/entity-avatar/PersonNameCell.jsx'
import { DataTable } from '../../../shared/ui/data-table/DataTable.jsx'
import { AppDialog } from '../../../shared/ui/dialog/AppDialog.jsx'
import { DEFAULT_ENROLLMENT_STATUS, ENROLLMENT_STATUS_OPTIONS, getEnrollmentStatusLabel } from '../../../shared/lib/texts/enrollmentStatus.js'

const columnHelper = createColumnHelper()

export function CourseStudentsPage() {
  const page = appConfig.pages.courseStudents
  const [offerings, setOfferings] = useState([])
  const [offeringId, setOfferingId] = useState('')
  const [programEntityId, setProgramEntityId] = useState('')
  const [rows, setRows] = useState([])
  const [error, setError] = useState('')
  const [loadingOfferings, setLoadingOfferings] = useState(true)
  const [loadingTable, setLoadingTable] = useState(false)
  const [globalFilter, setGlobalFilter] = useState('')

  const [addOpen, setAddOpen] = useState(false)
  const [addStudentId, setAddStudentId] = useState('')
  const [availableStudents, setAvailableStudents] = useState([])
  const [loadingAddList, setLoadingAddList] = useState(false)
  const [addError, setAddError] = useState('')
  const [saving, setSaving] = useState(false)

  const [statusRow, setStatusRow] = useState(null)
  const [statusValue, setStatusValue] = useState(DEFAULT_ENROLLMENT_STATUS)
  const [statusError, setStatusError] = useState('')

  const [deleteTarget, setDeleteTarget] = useState(null)

  const loadOfferings = useCallback(async () => {
    const token = getAdminToken()
    if (!token) return
    setLoadingOfferings(true)
    setError('')
    try {
      const data = await fetchCourseOfferingsActiveTerm(token)
      const list = Array.isArray(data) ? data : []
      setOfferings(list)
      setOfferingId((prev) => {
        if (!list.length) return ''
        if (prev && list.some((o) => o.id === prev)) return prev
        return list[0].id
      })
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Ders açılışları alınamadı.')
    } finally {
      setLoadingOfferings(false)
    }
  }, [])

  const loadOfferingAndStudents = useCallback(async () => {
    const token = getAdminToken()
    if (!token || !offeringId) {
      setRows([])
      setProgramEntityId('')
      setLoadingTable(false)
      return
    }

    const selected = offerings.find((o) => String(o.id) === String(offeringId))
    if (!selected?.courseId) {
      setError('Seçilen ders açılışında ders bilgisi yok; listeyi yenileyin.')
      setRows([])
      setProgramEntityId('')
      setLoadingTable(false)
      return
    }

    setLoadingTable(true)
    setError('')
    try {
      const [enrolled, course] = await Promise.all([
        fetchEnrolledStudents(token, offeringId),
        fetchCourseById(token, selected.courseId),
      ])
      setProgramEntityId(course?.programEntityId ? String(course.programEntityId) : '')
      setRows(Array.isArray(enrolled) ? enrolled : [])
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Veriler alınamadı.')
      setRows([])
      setProgramEntityId('')
    } finally {
      setLoadingTable(false)
    }
  }, [offeringId, offerings])

  useEffect(() => {
    loadOfferings()
  }, [loadOfferings])

  useEffect(() => {
    loadOfferingAndStudents()
  }, [loadOfferingAndStudents])

  const handleRefresh = useCallback(() => {
    void loadOfferings()
    void loadOfferingAndStudents()
  }, [loadOfferings, loadOfferingAndStudents])

  const openAdd = useCallback(async () => {
    const token = getAdminToken()
    if (!token || !offeringId) {
      setAddError('Önce ders açılışı seçin.')
      return
    }

    setAddError('')
    setAddStudentId('')
    setAddOpen(true)
    setLoadingAddList(true)

    let pid = programEntityId
    if (!pid) {
      const selected = offerings.find((o) => String(o.id) === String(offeringId))
      if (selected?.courseId) {
        try {
          const course = await fetchCourseById(token, selected.courseId)
          pid = course?.programEntityId ? String(course.programEntityId) : ''
          if (pid) setProgramEntityId(pid)
        } catch {
          pid = ''
        }
      }
    }

    if (!pid) {
      setAddError('Program bilgisi alınamadı. Ders açılışını tekrar seçin.')
      setAvailableStudents([])
      setLoadingAddList(false)
      return
    }

    try {
      const list = await fetchStudents(token, pid)
      const enrolledIds = new Set(rows.map((r) => String(r.studentId)))
      const all = Array.isArray(list) ? list : []
      setAvailableStudents(all.filter((s) => s.id && !enrolledIds.has(String(s.id))))
    } catch (e) {
      setAddError(e instanceof Error ? e.message : 'Öğrenci listesi alınamadı.')
      setAvailableStudents([])
    } finally {
      setLoadingAddList(false)
    }
  }, [offeringId, programEntityId, offerings, rows])

  const submitAdd = async (e) => {
    e.preventDefault()
    const token = getAdminToken()
    if (!token || !addStudentId) {
      setAddError('Öğrenci seçin.')
      return
    }
    setSaving(true)
    setAddError('')
    try {
      await enrollStudentInOffering(token, offeringId, addStudentId)
      setAddOpen(false)
      await loadOfferingAndStudents()
    } catch (err) {
      setAddError(err instanceof Error ? err.message : 'Kayıt eklenemedi.')
    } finally {
      setSaving(false)
    }
  }

  const openStatus = useCallback((row) => {
    setStatusRow(row)
    setStatusValue(row.status || 'Enrolled')
    setStatusError('')
  }, [])

  const submitStatus = async (e) => {
    e.preventDefault()
    const token = getAdminToken()
    if (!token || !statusRow) return
    setSaving(true)
    setStatusError('')
    try {
      await updateEnrollmentStatus(token, offeringId, statusRow.studentId, statusValue)
      setStatusRow(null)
      await loadOfferingAndStudents()
    } catch (err) {
      setStatusError(err instanceof Error ? err.message : 'Durum güncellenemedi.')
    } finally {
      setSaving(false)
    }
  }

  const handleDelete = async () => {
    const token = getAdminToken()
    if (!token || !deleteTarget) return
    setSaving(true)
    try {
      await removeEnrollment(token, offeringId, deleteTarget.studentId)
      setDeleteTarget(null)
      await loadOfferingAndStudents()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Kayıt silinemedi.')
    } finally {
      setSaving(false)
    }
  }

  const columns = useMemo(
    () => [
      columnHelper.display({
        id: 'studentFullName',
        header: 'Öğrenci',
        cell: ({ row }) => (
          <PersonNameCell
            variant="student"
            name={row.original.studentFullName}
            seedKey={row.original.studentId}
          />
        ),
      }),
      columnHelper.accessor('studentNumber', {
        header: 'Numara',
        cell: (info) => info.getValue() ?? '—',
      }),
      columnHelper.accessor('status', {
        header: 'Durum',
        cell: (info) => getEnrollmentStatusLabel(info.getValue()),
      }),
      columnHelper.accessor('enrolledAt', {
        header: 'Kayıt tarihi',
        cell: (info) => {
          const v = info.getValue()
          return v ? new Date(v).toLocaleString('tr-TR') : '—'
        },
      }),
      columnHelper.display({
        id: 'actions',
        header: 'İşlemler',
        cell: ({ row }) => (
          <div className={formStyles.actions}>
            <button
              type="button"
              className={formStyles.actionIcon}
              title="Durumu düzenle"
              onClick={() => openStatus(row.original)}
            >
              <Pencil size={16} aria-hidden />
            </button>
            <button
              type="button"
              className={`${formStyles.actionIcon} ${formStyles.actionDanger}`}
              title="Kaydı sil"
              onClick={() => setDeleteTarget(row.original)}
            >
              <Trash2 size={16} aria-hidden />
            </button>
          </div>
        ),
      }),
    ],
    [openStatus],
  )

  const toolbar = (
    <label>
      Ders açılışı (aktif dönem)
      <select
        className={sectionStyles.select}
        value={offeringId}
        onChange={(e) => setOfferingId(e.target.value)}
        disabled={!offerings.length}
      >
        {offerings.map((o) => (
          <option key={o.id} value={o.id}>
            {o.courseCode} — {o.courseName} ({o.section}) · {o.termName}
          </option>
        ))}
      </select>
    </label>
  )

  const sectionLoading = loadingOfferings

  return (
    <PageSection title={page.title} description={page.description} error={error} loading={sectionLoading}>
      {!loadingOfferings && !offerings.length ? (
        <p className={sectionStyles.muted}>
          Aktif dönemde açılmış ders bulunamadı. Önce akademik dönem ve ders açılışı oluşturun.
        </p>
      ) : null}

      {offeringId && !loadingOfferings ? (
        <DataTable
          columns={columns}
          data={rows}
          globalFilter={globalFilter}
          onGlobalFilterChange={setGlobalFilter}
          searchPlaceholder="Ad, numara veya durum ara…"
          toolbarFilters={toolbar}
          toolbarExtra={
            <>
              <RefreshIconButton onClick={handleRefresh} loading={loadingTable || loadingOfferings} />
              <button
                type="button"
                className={`${formStyles.btn} ${formStyles.btnPrimary}`}
                onClick={openAdd}
                disabled={!programEntityId}
              >
                <Plus size={18} aria-hidden />
                Öğrenci kaydet
              </button>
            </>
          }
          isLoading={loadingTable}
        />
      ) : null}

      <AppDialog
        open={addOpen}
        onClose={() => setAddOpen(false)}
        title="Derse öğrenci kaydet"
        size="md"
        footer={
          <>
            <button
              type="button"
              className={`${formStyles.btn} ${formStyles.btnGhost}`}
              onClick={() => setAddOpen(false)}
            >
              Vazgeç
            </button>
            <button
              type="submit"
              form="enroll-add-form"
              className={`${formStyles.btn} ${formStyles.btnPrimary}`}
              disabled={saving || loadingAddList || !availableStudents.length}
            >
              {saving ? 'Kaydediliyor…' : 'Kaydet'}
            </button>
          </>
        }
      >
        <form id="enroll-add-form" className={formStyles.form} onSubmit={submitAdd}>
          {addError ? (
            <p className={sectionStyles.error} role="alert">
              {addError}
            </p>
          ) : null}
          {loadingAddList ? (
            <p className={sectionStyles.muted}>Öğrenci listesi yükleniyor…</p>
          ) : (
            <div className={formStyles.field}>
              <label className={formStyles.label} htmlFor="enroll-student">
                Öğrenci
              </label>
              <select
                id="enroll-student"
                className={formStyles.select}
                value={addStudentId}
                onChange={(e) => setAddStudentId(e.target.value)}
                required
              >
                <option value="">Seçin</option>
                {availableStudents.map((s) => (
                  <option key={s.id} value={s.id}>
                    {s.fullName}
                    {s.studentNumber ? ` (${s.studentNumber})` : ''}
                  </option>
                ))}
              </select>
              {!availableStudents.length && !addError ? (
                <p className={sectionStyles.muted}>
                  Bu programa kayıtlı veya eklenebilir öğrenci kalmadı.
                </p>
              ) : null}
            </div>
          )}
        </form>
      </AppDialog>

      <AppDialog
        open={Boolean(statusRow)}
        onClose={() => setStatusRow(null)}
        title="Kayıt durumunu güncelle"
        size="sm"
        footer={
          <>
            <button
              type="button"
              className={`${formStyles.btn} ${formStyles.btnGhost}`}
              onClick={() => setStatusRow(null)}
            >
              Vazgeç
            </button>
            <button
              type="submit"
              form="enroll-status-form"
              className={`${formStyles.btn} ${formStyles.btnPrimary}`}
              disabled={saving}
            >
              {saving ? 'Kaydediliyor…' : 'Kaydet'}
            </button>
          </>
        }
      >
        <form id="enroll-status-form" className={formStyles.form} onSubmit={submitStatus}>
          {statusError ? (
            <p className={sectionStyles.error} role="alert">
              {statusError}
            </p>
          ) : null}
          <p className={sectionStyles.muted}>
            <strong>{statusRow?.studentFullName}</strong>
          </p>
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="enroll-status">
              Durum
            </label>
            <select
              id="enroll-status"
              className={formStyles.select}
              value={statusValue}
              onChange={(e) => setStatusValue(e.target.value)}
            >
              {ENROLLMENT_STATUS_OPTIONS.map((o) => (
                <option key={o.value} value={o.value}>
                  {o.label}
                </option>
              ))}
            </select>
          </div>
        </form>
      </AppDialog>

      <AppDialog
        open={Boolean(deleteTarget)}
        onClose={() => setDeleteTarget(null)}
        title="Kaydı sil"
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
          <strong>{deleteTarget?.studentFullName}</strong> bu ders açılışından çıkarılsın mı? Sınav/cevap
          kaydı varsa API işlemi reddedebilir; o durumda durumu &quot;Çekildi&quot; yapın.
        </p>
      </AppDialog>
    </PageSection>
  )
}
