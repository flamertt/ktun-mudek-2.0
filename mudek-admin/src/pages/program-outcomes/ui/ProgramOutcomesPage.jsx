import { createColumnHelper } from '@tanstack/react-table'
import { Pencil, Plus, Trash2 } from 'lucide-react'
import { useCallback, useEffect, useMemo, useState } from 'react'

import {
  createProgram,
  createProgramOutcome,
  deleteProgram,
  deleteProgramOutcome,
  fetchProgramOutcomes,
  fetchPrograms,
  updateProgram,
  updateProgramOutcome,
} from '../../../shared/api/adminApi'
import { appConfig } from '../../../shared/config/appConfig'
import { getAdminToken } from '../../../shared/lib/authToken'
import formStyles from '../../../shared/ui/admin-form/AdminForm.module.css'
import { AdminSection } from '../../../shared/ui/admin-section/AdminSection.jsx'
import sectionStyles from '../../../shared/ui/admin-section/AdminSection.module.css'
import { DataTable } from '../../../shared/ui/data-table/DataTable.jsx'
import { AppDialog } from '../../../shared/ui/dialog/AppDialog.jsx'
import poStyles from './ProgramOutcomesPage.module.css'

const columnHelper = createColumnHelper()

const emptyProgram = { name: '', accreditationCycleYears: 5 }
const emptyOutcome = { code: '', title: '', description: '' }

export function ProgramOutcomesPage() {
  const page = appConfig.pages.programOutcomes
  const [programs, setPrograms] = useState([])
  const [programId, setProgramId] = useState('')
  const [outcomeRows, setOutcomeRows] = useState([])
  const [error, setError] = useState('')
  const [loadingPrograms, setLoadingPrograms] = useState(true)
  const [loadingOutcomes, setLoadingOutcomes] = useState(false)
  const [filterPrograms, setFilterPrograms] = useState('')
  const [filterOutcomes, setFilterOutcomes] = useState('')

  const [progDialogOpen, setProgDialogOpen] = useState(false)
  const [progMode, setProgMode] = useState('create')
  const [progForm, setProgForm] = useState(emptyProgram)
  const [progEditId, setProgEditId] = useState(null)
  const [progFormError, setProgFormError] = useState('')
  const [deleteProgramTarget, setDeleteProgramTarget] = useState(null)

  const [outDialogOpen, setOutDialogOpen] = useState(false)
  const [outMode, setOutMode] = useState('create')
  const [outForm, setOutForm] = useState(emptyOutcome)
  const [outEditId, setOutEditId] = useState(null)
  const [outFormError, setOutFormError] = useState('')
  const [deleteOutcomeTarget, setDeleteOutcomeTarget] = useState(null)

  const [saving, setSaving] = useState(false)

  const loadPrograms = useCallback(async () => {
    const token = getAdminToken()
    if (!token) return
    setLoadingPrograms(true)
    setError('')
    try {
      const data = await fetchPrograms(token)
      const list = Array.isArray(data) ? data : []
      setPrograms(list)
      setProgramId((prev) => {
        if (!list.length) return ''
        if (prev && list.some((p) => p.id === prev)) return prev
        return list[0].id
      })
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Programlar yüklenemedi.')
    } finally {
      setLoadingPrograms(false)
    }
  }, [])

  const loadOutcomes = useCallback(async () => {
    const token = getAdminToken()
    if (!token || !programId) {
      setOutcomeRows([])
      setLoadingOutcomes(false)
      return
    }
    setLoadingOutcomes(true)
    setError('')
    try {
      const data = await fetchProgramOutcomes(token, programId)
      setOutcomeRows(Array.isArray(data) ? data : [])
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Program çıktıları alınamadı.')
    } finally {
      setLoadingOutcomes(false)
    }
  }, [programId])

  useEffect(() => {
    loadPrograms()
  }, [loadPrograms])

  useEffect(() => {
    loadOutcomes()
  }, [loadOutcomes])

  const openCreateProgram = useCallback(() => {
    setProgMode('create')
    setProgEditId(null)
    setProgForm({ ...emptyProgram })
    setProgFormError('')
    setProgDialogOpen(true)
  }, [])

  const openEditProgram = useCallback((row) => {
    setProgMode('edit')
    setProgEditId(row.id)
    setProgForm({
      name: row.name ?? '',
      accreditationCycleYears: Number(row.accreditationCycleYears) || 5,
    })
    setProgFormError('')
    setProgDialogOpen(true)
  }, [])

  const submitProgram = async (e) => {
    e.preventDefault()
    const token = getAdminToken()
    if (!token) return
    if (!progForm.name.trim()) {
      setProgFormError('Program adı zorunludur.')
      return
    }
    setSaving(true)
    setProgFormError('')
    try {
      if (progMode === 'create') {
        await createProgram(token, {
          name: progForm.name.trim(),
          accreditationCycleYears: Number(progForm.accreditationCycleYears) || 5,
        })
      } else {
        await updateProgram(token, progEditId, {
          name: progForm.name.trim(),
          accreditationCycleYears: Number(progForm.accreditationCycleYears) || 5,
        })
      }
      setProgDialogOpen(false)
      await loadPrograms()
    } catch (err) {
      setProgFormError(err instanceof Error ? err.message : 'Kayıt başarısız.')
    } finally {
      setSaving(false)
    }
  }

  const handleDeleteProgram = async () => {
    const token = getAdminToken()
    if (!token || !deleteProgramTarget) return
    setSaving(true)
    try {
      await deleteProgram(token, deleteProgramTarget.id)
      setDeleteProgramTarget(null)
      await loadPrograms()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Program silinemedi.')
    } finally {
      setSaving(false)
    }
  }

  const openCreateOutcome = useCallback(() => {
    if (!programId) return
    setOutMode('create')
    setOutEditId(null)
    setOutForm({ ...emptyOutcome })
    setOutFormError('')
    setOutDialogOpen(true)
  }, [programId])

  const openEditOutcome = useCallback((row) => {
    setOutMode('edit')
    setOutEditId(row.id)
    setOutForm({
      code: row.code ?? '',
      title: row.title ?? '',
      description: row.description ?? '',
    })
    setOutFormError('')
    setOutDialogOpen(true)
  }, [])

  const submitOutcome = async (e) => {
    e.preventDefault()
    const token = getAdminToken()
    if (!token) return
    if (!outForm.code.trim() || !outForm.title.trim()) {
      setOutFormError('Kod ve başlık zorunludur.')
      return
    }
    setSaving(true)
    setOutFormError('')
    try {
      if (outMode === 'create') {
        await createProgramOutcome(token, {
          programEntityId: programId,
          code: outForm.code.trim(),
          title: outForm.title.trim(),
          description: outForm.description.trim() || '',
        })
      } else {
        await updateProgramOutcome(token, outEditId, {
          code: outForm.code.trim(),
          title: outForm.title.trim(),
          description: outForm.description.trim() || '',
        })
      }
      setOutDialogOpen(false)
      await loadOutcomes()
    } catch (err) {
      setOutFormError(err instanceof Error ? err.message : 'Kayıt başarısız.')
    } finally {
      setSaving(false)
    }
  }

  const handleDeleteOutcome = async () => {
    const token = getAdminToken()
    if (!token || !deleteOutcomeTarget) return
    setSaving(true)
    try {
      await deleteProgramOutcome(token, deleteOutcomeTarget.id)
      setDeleteOutcomeTarget(null)
      await loadOutcomes()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Silinemedi.')
    } finally {
      setSaving(false)
    }
  }

  const programColumns = useMemo(
    () => [
      columnHelper.accessor('name', { header: 'Program adı' }),
      columnHelper.accessor('accreditationCycleYears', {
        header: 'Akreditasyon döngüsü (yıl)',
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
              onClick={() => openEditProgram(row.original)}
            >
              <Pencil size={16} aria-hidden />
            </button>
            <button
              type="button"
              className={`${formStyles.actionIcon} ${formStyles.actionDanger}`}
              title="Sil"
              onClick={() => setDeleteProgramTarget(row.original)}
            >
              <Trash2 size={16} aria-hidden />
            </button>
          </div>
        ),
      }),
    ],
    [openEditProgram],
  )

  const outcomeColumns = useMemo(
    () => [
      columnHelper.accessor('code', { header: 'Kod' }),
      columnHelper.accessor('title', { header: 'Başlık' }),
      columnHelper.accessor('description', {
        header: 'Açıklama',
        cell: (info) => {
          const v = info.getValue() ?? ''
          return (
            <span className={poStyles.cellMuted} title={v}>
              {v || '—'}
            </span>
          )
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
              title="Düzenle"
              onClick={() => openEditOutcome(row.original)}
            >
              <Pencil size={16} aria-hidden />
            </button>
            <button
              type="button"
              className={`${formStyles.actionIcon} ${formStyles.actionDanger}`}
              title="Sil"
              onClick={() => setDeleteOutcomeTarget(row.original)}
            >
              <Trash2 size={16} aria-hidden />
            </button>
          </div>
        ),
      }),
    ],
    [openEditOutcome],
  )

  const outcomeToolbar = (
    <div className={poStyles.toolbarRow}>
      <label>
        Program
        <select
          className={sectionStyles.select}
          value={programId}
          onChange={(e) => setProgramId(e.target.value)}
          disabled={!programs.length}
        >
          {programs.map((p) => (
            <option key={p.id} value={p.id}>
              {p.name}
            </option>
          ))}
        </select>
      </label>
    </div>
  )

  return (
    <AdminSection title={page.title} description={page.description} error={error}>
      <div className={poStyles.block}>
        <h3 className={poStyles.subTitle}>Programlar</h3>
        <DataTable
          columns={programColumns}
          data={programs}
          globalFilter={filterPrograms}
          onGlobalFilterChange={setFilterPrograms}
          searchPlaceholder="Program adında ara…"
          toolbarExtra={
            <button
              type="button"
              className={`${formStyles.btn} ${formStyles.btnPrimary}`}
              onClick={openCreateProgram}
            >
              <Plus size={18} aria-hidden />
              Yeni program
            </button>
          }
          isLoading={loadingPrograms}
        />
      </div>

      <div className={poStyles.block}>
        <h3 className={poStyles.subTitle}>Program çıktıları (PÇ)</h3>
        {!programs.length ? (
          <p className={sectionStyles.muted}>Önce yukarıdan bir program ekleyin.</p>
        ) : (
          <>
            {outcomeToolbar}
            <DataTable
              columns={outcomeColumns}
              data={outcomeRows}
              globalFilter={filterOutcomes}
              onGlobalFilterChange={setFilterOutcomes}
              searchPlaceholder="Kod, başlık veya açıklamada ara…"
              toolbarExtra={
                <button
                  type="button"
                  className={`${formStyles.btn} ${formStyles.btnPrimary}`}
                  onClick={openCreateOutcome}
                  disabled={!programId}
                >
                  <Plus size={18} aria-hidden />
                  Yeni program çıktısı
                </button>
              }
              isLoading={loadingOutcomes}
            />
          </>
        )}
      </div>

      <AppDialog
        open={progDialogOpen}
        onClose={() => setProgDialogOpen(false)}
        title={progMode === 'create' ? 'Yeni program' : 'Programı düzenle'}
        size="md"
        footer={
          <>
            <button
              type="button"
              className={`${formStyles.btn} ${formStyles.btnGhost}`}
              onClick={() => setProgDialogOpen(false)}
            >
              Vazgeç
            </button>
            <button
              type="submit"
              form="po-program-form"
              className={`${formStyles.btn} ${formStyles.btnPrimary}`}
              disabled={saving}
            >
              {saving ? 'Kaydediliyor…' : 'Kaydet'}
            </button>
          </>
        }
      >
        <form id="po-program-form" className={formStyles.form} onSubmit={submitProgram}>
          {progFormError ? (
            <p className={sectionStyles.error} role="alert">
              {progFormError}
            </p>
          ) : null}
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="po-pname">
              Program adı
            </label>
            <input
              id="po-pname"
              className={formStyles.input}
              value={progForm.name}
              onChange={(e) => setProgForm((f) => ({ ...f, name: e.target.value }))}
              required
            />
          </div>
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="po-pcycle">
              Akreditasyon döngüsü (yıl)
            </label>
            <input
              id="po-pcycle"
              type="number"
              min={1}
              className={formStyles.input}
              value={progForm.accreditationCycleYears}
              onChange={(e) =>
                setProgForm((f) => ({ ...f, accreditationCycleYears: Number(e.target.value) }))
              }
            />
          </div>
        </form>
      </AppDialog>

      <AppDialog
        open={Boolean(deleteProgramTarget)}
        onClose={() => setDeleteProgramTarget(null)}
        title="Programı sil"
        size="sm"
        footer={
          <>
            <button
              type="button"
              className={`${formStyles.btn} ${formStyles.btnGhost}`}
              onClick={() => setDeleteProgramTarget(null)}
            >
              İptal
            </button>
            <button
              type="button"
              className={`${formStyles.btn} ${formStyles.btnDanger}`}
              disabled={saving}
              onClick={handleDeleteProgram}
            >
              {saving ? 'Siliniyor…' : 'Sil'}
            </button>
          </>
        }
      >
        <p className={sectionStyles.muted}>
          <strong>{deleteProgramTarget?.name}</strong> ve ilişkili veriler silinebilir. Emin misiniz?
        </p>
      </AppDialog>

      <AppDialog
        open={outDialogOpen}
        onClose={() => setOutDialogOpen(false)}
        title={outMode === 'create' ? 'Yeni program çıktısı' : 'Program çıktısını düzenle'}
        size="lg"
        footer={
          <>
            <button
              type="button"
              className={`${formStyles.btn} ${formStyles.btnGhost}`}
              onClick={() => setOutDialogOpen(false)}
            >
              Vazgeç
            </button>
            <button
              type="submit"
              form="po-outcome-form"
              className={`${formStyles.btn} ${formStyles.btnPrimary}`}
              disabled={saving}
            >
              {saving ? 'Kaydediliyor…' : 'Kaydet'}
            </button>
          </>
        }
      >
        <form id="po-outcome-form" className={formStyles.form} onSubmit={submitOutcome}>
          {outFormError ? (
            <p className={sectionStyles.error} role="alert">
              {outFormError}
            </p>
          ) : null}
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="po-oc">
              Kod
            </label>
            <input
              id="po-oc"
              className={formStyles.input}
              value={outForm.code}
              onChange={(e) => setOutForm((f) => ({ ...f, code: e.target.value }))}
              required
            />
          </div>
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="po-ot">
              Başlık
            </label>
            <input
              id="po-ot"
              className={formStyles.input}
              value={outForm.title}
              onChange={(e) => setOutForm((f) => ({ ...f, title: e.target.value }))}
              required
            />
          </div>
          <div className={formStyles.field}>
            <label className={formStyles.label} htmlFor="po-od">
              Açıklama
            </label>
            <textarea
              id="po-od"
              className={formStyles.textarea}
              value={outForm.description}
              onChange={(e) => setOutForm((f) => ({ ...f, description: e.target.value }))}
            />
          </div>
        </form>
      </AppDialog>

      <AppDialog
        open={Boolean(deleteOutcomeTarget)}
        onClose={() => setDeleteOutcomeTarget(null)}
        title="Program çıktısını sil"
        size="sm"
        footer={
          <>
            <button
              type="button"
              className={`${formStyles.btn} ${formStyles.btnGhost}`}
              onClick={() => setDeleteOutcomeTarget(null)}
            >
              İptal
            </button>
            <button
              type="button"
              className={`${formStyles.btn} ${formStyles.btnDanger}`}
              disabled={saving}
              onClick={handleDeleteOutcome}
            >
              {saving ? 'Siliniyor…' : 'Sil'}
            </button>
          </>
        }
      >
        <p className={sectionStyles.muted}>
          <strong>{deleteOutcomeTarget?.code}</strong> — {deleteOutcomeTarget?.title} silinsin mi?
        </p>
      </AppDialog>
    </AdminSection>
  )
}
