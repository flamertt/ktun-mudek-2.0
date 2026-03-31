import { createColumnHelper } from '@tanstack/react-table'
import { Pencil, Plus, Trash2 } from 'lucide-react'
import { useCallback, useEffect, useMemo, useState } from 'react'

import {
  fetchCourseById,
  fetchCourseClos,
  fetchCourses,
  fetchCloPoMapsForCourse,
  fetchProgramOutcomes,
  fetchPrograms,
  linkCloToProgramOutcome,
  unlinkCloFromProgramOutcome,
  updateCloProgramOutcomeWeight,
} from '../../../shared/api/adminApi'
import { appConfig } from '../../../shared/config/appConfig'
import { getAdminToken } from '../../../shared/lib/authToken'
import formStyles from '../../../shared/ui/admin-form/AdminForm.module.css'
import { DataTable } from '../../../shared/ui/data-table/DataTable.jsx'
import { AppDialog } from '../../../shared/ui/dialog/AppDialog.jsx'
import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import sectionStyles from '@shared/ui/page-section/PageSection.module.css'
import { RefreshIconButton } from '../../../shared/ui/refresh-icon-button/RefreshIconButton.jsx'
import pageStyles from './CloPoMappingPage.module.css'

const columnHelper = createColumnHelper()

export function CloPoMappingPage() {
  const page = appConfig.pages.cloPoMapping
  const [programs, setPrograms] = useState([])
  const [programId, setProgramId] = useState('')
  const [courses, setCourses] = useState([])
  const [courseId, setCourseId] = useState('')
  const [catalogError, setCatalogError] = useState('')
  const [loadingCatalog, setLoadingCatalog] = useState(true)
  const [programEntityId, setProgramEntityId] = useState('')
  const [clos, setClos] = useState([])
  const [outcomes, setOutcomes] = useState([])
  const [maps, setMaps] = useState([])
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)
  const [globalFilter, setGlobalFilter] = useState('')

  const [addOpen, setAddOpen] = useState(false)
  const [addForm, setAddForm] = useState({ cloId: '', programOutcomeId: '', weight: 1 })
  const [formError, setFormError] = useState('')
  const [saving, setSaving] = useState(false)

  const [editRow, setEditRow] = useState(null)
  const [editWeight, setEditWeight] = useState(1)

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

  const loadCourseContext = useCallback(async () => {
    const token = getAdminToken()
    if (!token || !courseId) {
      setProgramEntityId('')
      setClos([])
      setOutcomes([])
      setMaps([])
      return
    }
    setLoading(true)
    setError('')
    try {
      const course = await fetchCourseById(token, courseId)
      const pid = course?.programEntityId ? String(course.programEntityId) : ''
      setProgramEntityId(pid)

      const [cloList, mapList, poList] = await Promise.all([
        fetchCourseClos(token, courseId),
        fetchCloPoMapsForCourse(token, courseId),
        pid ? fetchProgramOutcomes(token, pid) : Promise.resolve([]),
      ])
      setClos(Array.isArray(cloList) ? cloList : [])
      setMaps(Array.isArray(mapList) ? mapList : [])
      setOutcomes(Array.isArray(poList) ? poList : [])
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Veriler yüklenemedi.')
      setMaps([])
      setClos([])
      setOutcomes([])
    } finally {
      setLoading(false)
    }
  }, [courseId])

  useEffect(() => {
    loadPrograms()
  }, [loadPrograms])

  useEffect(() => {
    loadCourses()
  }, [loadCourses])

  useEffect(() => {
    loadCourseContext()
  }, [loadCourseContext])

  const handleRefresh = useCallback(() => {
    loadPrograms()
    loadCourses()
    void loadCourseContext()
  }, [loadPrograms, loadCourses, loadCourseContext])

  const openAdd = useCallback(() => {
    if (!courseId || !clos.length) return
    setAddForm({
      cloId: clos[0]?.id ?? '',
      programOutcomeId: outcomes[0]?.id ?? '',
      weight: 1,
    })
    setFormError('')
    setAddOpen(true)
  }, [courseId, clos, outcomes])

  const handleAdd = async (e) => {
    e.preventDefault()
    const token = getAdminToken()
    if (!token || !courseId || !addForm.cloId || !addForm.programOutcomeId) {
      setFormError('CLO ve program çıktısı seçin.')
      return
    }
    setSaving(true)
    setFormError('')
    try {
      await linkCloToProgramOutcome(token, courseId, addForm.cloId, {
        programOutcomeId: addForm.programOutcomeId,
        weight: Number(addForm.weight) || 0,
      })
      setAddOpen(false)
      loadCourseContext()
    } catch (err) {
      setFormError(err instanceof Error ? err.message : 'Eşleme eklenemedi.')
    } finally {
      setSaving(false)
    }
  }

  const handleEditSave = async () => {
    const token = getAdminToken()
    if (!token || !courseId || !editRow) return
    setSaving(true)
    try {
      await updateCloProgramOutcomeWeight(
        token,
        courseId,
        editRow.courseLearningOutcomeId,
        editRow.programOutcomeId,
        { weight: Number(editWeight) || 0 },
      )
      setEditRow(null)
      loadCourseContext()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Ağırlık güncellenemedi.')
    } finally {
      setSaving(false)
    }
  }

  const handleDelete = async () => {
    const token = getAdminToken()
    if (!token || !deleteTarget || !courseId) return
    setSaving(true)
    try {
      await unlinkCloFromProgramOutcome(
        token,
        courseId,
        deleteTarget.courseLearningOutcomeId,
        deleteTarget.programOutcomeId,
      )
      setDeleteTarget(null)
      loadCourseContext()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Eşleme kaldırılamadı.')
    } finally {
      setSaving(false)
    }
  }

  const columns = useMemo(
    () => [
      columnHelper.accessor('cloCode', { header: 'CLO', cell: (info) => info.getValue() ?? '—' }),
      columnHelper.accessor('poCode', { header: 'PÇ', cell: (info) => info.getValue() ?? '—' }),
      columnHelper.accessor('weight', {
        header: 'Ağırlık',
        cell: (info) => String(info.getValue() ?? ''),
      }),
      columnHelper.display({
        id: 'actions',
        header: 'İşlemler',
        cell: ({ row }) => (
          <div className={formStyles.actions}>
            <button
              type="button"
              className={formStyles.actionIcon}
              title="Ağırlık düzenle"
              onClick={() => {
                setEditRow(row.original)
                setEditWeight(Number(row.original.weight) || 0)
              }}
            >
              <Pencil size={16} aria-hidden />
            </button>
            <button
              type="button"
              className={`${formStyles.actionIcon} ${formStyles.actionDanger}`}
              title="Eşlemeyi kaldır"
              onClick={() => setDeleteTarget(row.original)}
            >
              <Trash2 size={16} aria-hidden />
            </button>
          </div>
        ),
      }),
    ],
    [],
  )

  const missingProgram = courseId && !programEntityId

  const combinedError = [catalogError, error].filter(Boolean).join(' ')

  return (
    <PageSection title={page.title} description={page.description} error={combinedError || undefined}>
      <div className={pageStyles.filterRow}>
        <div className={pageStyles.filterRowFields}>
          <label htmlFor="map-filter-program">
            Program filtresi
            <select
              id="map-filter-program"
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
          <label htmlFor="map-filter-course">
            Ders
            <select
              id="map-filter-course"
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

      {!courseId ? <p className={pageStyles.hint}>Eşleme tablosu için yukarıdan bir ders seçin.</p> : null}

      {courseId ? (
        <>
          {missingProgram ? (
            <p className={sectionStyles.muted}>Bu ders için program bilgisi alınamadı; PÇ listesi yüklenemez.</p>
          ) : null}
          <DataTable
            columns={columns}
            data={maps}
            globalFilter={globalFilter}
            onGlobalFilterChange={setGlobalFilter}
            searchPlaceholder="CLO veya PÇ kodunda ara…"
            toolbarExtra={
              <>
                <RefreshIconButton onClick={handleRefresh} loading={loadingCatalog || loading} />
                <button
                  type="button"
                  className={`${formStyles.btn} ${formStyles.btnPrimary}`}
                  onClick={openAdd}
                  disabled={!courseId || !clos.length || !outcomes.length || missingProgram}
                >
                  <Plus size={18} aria-hidden />
                  Eşleme ekle
                </button>
              </>
            }
            isLoading={loading}
          />
        </>
      ) : null}

      <AppDialog
        open={addOpen}
        onClose={() => setAddOpen(false)}
        title="CLO ↔ PÇ eşlemesi"
        size="md"
        footer={
          <>
            <button type="button" className={`${formStyles.btn} ${formStyles.btnGhost}`} onClick={() => setAddOpen(false)}>
              Vazgeç
            </button>
            <button type="submit" form="clo-po-add" className={`${formStyles.btn} ${formStyles.btnPrimary}`} disabled={saving}>
              {saving ? 'Kaydediliyor…' : 'Ekle'}
            </button>
          </>
        }
      >
        <form id="clo-po-add" className={formStyles.form} onSubmit={handleAdd}>
          {formError ? (
            <p className={sectionStyles.error} role="alert">
              {formError}
            </p>
          ) : null}
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="map-clo">
              CLO
            </label>
            <select
              id="map-clo"
              className={formStyles.input}
              value={addForm.cloId}
              onChange={(e) => setAddForm((f) => ({ ...f, cloId: e.target.value }))}
              required
            >
              {clos.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.code}
                </option>
              ))}
            </select>
          </div>
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="map-po">
              Program çıktısı
            </label>
            <select
              id="map-po"
              className={formStyles.input}
              value={addForm.programOutcomeId}
              onChange={(e) => setAddForm((f) => ({ ...f, programOutcomeId: e.target.value }))}
              required
            >
              {outcomes.map((p) => (
                <option key={p.id} value={p.id}>
                  {p.code} — {p.title}
                </option>
              ))}
            </select>
          </div>
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="map-weight">
              Ağırlık
            </label>
            <input
              id="map-weight"
              type="number"
              step="0.01"
              min={0}
              className={formStyles.input}
              value={addForm.weight}
              onChange={(e) => setAddForm((f) => ({ ...f, weight: e.target.value }))}
            />
          </div>
        </form>
      </AppDialog>

      <AppDialog
        open={Boolean(editRow)}
        onClose={() => setEditRow(null)}
        title="Ağırlık güncelle"
        footer={
          <>
            <button type="button" className={`${formStyles.btn} ${formStyles.btnGhost}`} onClick={() => setEditRow(null)}>
              Vazgeç
            </button>
            <button type="button" className={`${formStyles.btn} ${formStyles.btnPrimary}`} onClick={handleEditSave} disabled={saving}>
              {saving ? '…' : 'Kaydet'}
            </button>
          </>
        }
      >
        <div className={formStyles.form}>
          <p className={sectionStyles.muted}>
            {editRow?.cloCode} → {editRow?.poCode}
          </p>
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="edit-weight">
              Ağırlık
            </label>
            <input
              id="edit-weight"
              type="number"
              step="0.01"
              min={0}
              className={formStyles.input}
              value={editWeight}
              onChange={(e) => setEditWeight(Number(e.target.value))}
            />
          </div>
        </div>
      </AppDialog>

      <AppDialog
        open={Boolean(deleteTarget)}
        onClose={() => setDeleteTarget(null)}
        title="Eşleme kaldırılsın mı?"
        footer={
          <>
            <button type="button" className={`${formStyles.btn} ${formStyles.btnGhost}`} onClick={() => setDeleteTarget(null)}>
              Vazgeç
            </button>
            <button type="button" className={`${formStyles.btn} ${formStyles.btnPrimary}`} onClick={handleDelete} disabled={saving}>
              {saving ? '…' : 'Kaldır'}
            </button>
          </>
        }
      >
        <p className={sectionStyles.muted}>
          <strong>{deleteTarget?.cloCode}</strong> ↔ <strong>{deleteTarget?.poCode}</strong>
        </p>
      </AppDialog>
    </PageSection>
  )
}
