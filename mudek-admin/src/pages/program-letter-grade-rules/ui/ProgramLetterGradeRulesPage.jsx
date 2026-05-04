import { createColumnHelper } from '@tanstack/react-table'
import { Pencil, Plus, Trash2 } from 'lucide-react'
import { useCallback, useEffect, useMemo, useState } from 'react'

import {
  createLetterGradeRule,
  deleteLetterGradeRule,
  fetchLetterGradeRulesByProgram,
  fetchUniversityPrograms,
  updateLetterGradeRule,
} from '../../../shared/api/adminApi'
import { appConfig } from '../../../shared/config/appConfig'
import { getAdminToken } from '../../../shared/lib/authToken'
import formStyles from '../../../shared/ui/admin-form/AdminForm.module.css'
import { DataTable } from '../../../shared/ui/data-table/DataTable.jsx'
import { AppDialog } from '../../../shared/ui/dialog/AppDialog.jsx'
import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import sectionStyles from '@shared/ui/page-section/PageSection.module.css'
import { RefreshIconButton } from '../../../shared/ui/refresh-icon-button/RefreshIconButton.jsx'
import styles from './ProgramLetterGradeRulesPage.module.css'

const columnHelper = createColumnHelper()

function programKey(p) {
  return String(p?.programId ?? p?.ProgramId ?? '')
}

function ruleKey(r) {
  return String(r?.id ?? r?.Id ?? '')
}

function rstr(x, a, b) {
  return x?.[a] ?? x?.[b] ?? ''
}

const emptyForm = {
  letterGrade: '',
  minScore: '',
  maxScore: '',
  isPassing: true,
  minimumFinalScore: '',
  description: '',
}

export function ProgramLetterGradeRulesPage() {
  const page = appConfig.pages.letterGradeRules
  const [programs, setPrograms] = useState([])
  const [programId, setProgramId] = useState('')
  const [rows, setRows] = useState([])
  const [error, setError] = useState('')
  const [loadingPrograms, setLoadingPrograms] = useState(true)
  const [loadingRules, setLoadingRules] = useState(false)
  const [dialogOpen, setDialogOpen] = useState(false)
  const [editId, setEditId] = useState(null)
  const [form, setForm] = useState(emptyForm)
  const [formError, setFormError] = useState('')
  const [saving, setSaving] = useState(false)
  const [globalFilter, setGlobalFilter] = useState('')

  const loadPrograms = useCallback(async () => {
    const token = getAdminToken()
    if (!token) return
    setLoadingPrograms(true)
    setError('')
    try {
      const data = await fetchUniversityPrograms(token)
      const list = Array.isArray(data) ? data : []
      setPrograms(list)
      setProgramId((prev) => {
        if (!list.length) return ''
        if (prev && list.some((p) => programKey(p) === prev)) return prev
        return programKey(list[0])
      })
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Programlar yüklenemedi.')
    } finally {
      setLoadingPrograms(false)
    }
  }, [])

  const loadRules = useCallback(async () => {
    const token = getAdminToken()
    if (!token || !programId) {
      setRows([])
      return
    }
    setLoadingRules(true)
    setError('')
    try {
      const data = await fetchLetterGradeRulesByProgram(token, Number(programId))
      setRows(Array.isArray(data) ? data : [])
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Kurallar yüklenemedi.')
      setRows([])
    } finally {
      setLoadingRules(false)
    }
  }, [programId])

  useEffect(() => {
    void loadPrograms()
  }, [loadPrograms])

  useEffect(() => {
    void loadRules()
  }, [loadRules])

  const openCreate = () => {
    setEditId(null)
    setForm(emptyForm)
    setFormError('')
    setDialogOpen(true)
  }

  const openEdit = (r) => {
    const id = ruleKey(r)
    setEditId(id)
    setForm({
      letterGrade: rstr(r, 'letterGrade', 'LetterGrade'),
      minScore: String(r.minScore ?? r.MinScore ?? ''),
      maxScore: String(r.maxScore ?? r.MaxScore ?? ''),
      isPassing: r.isPassing ?? r.IsPassing ?? true,
      minimumFinalScore:
        r.minimumFinalScore != null || r.MinimumFinalScore != null
          ? String(r.minimumFinalScore ?? r.MinimumFinalScore)
          : '',
      description: rstr(r, 'description', 'Description'),
    })
    setFormError('')
    setDialogOpen(true)
  }

  const save = async () => {
    const token = getAdminToken()
    if (!token || !programId) return
    const letter = form.letterGrade.trim().toUpperCase()
    const min = Number(form.minScore)
    const max = Number(form.maxScore)
    if (!letter) {
      setFormError('Harf notu zorunlu.')
      return
    }
    if (!Number.isFinite(min) || !Number.isFinite(max)) {
      setFormError('Min / max sayı olmalı.')
      return
    }
    const minFinRaw = form.minimumFinalScore.trim()
    const minFin = minFinRaw === '' ? null : Number(minFinRaw)
    if (minFinRaw !== '' && !Number.isFinite(minFin)) {
      setFormError('Min. final geçersiz.')
      return
    }
    setSaving(true)
    setFormError('')
    try {
      const body = {
        letterGrade: letter,
        minScore: min,
        maxScore: max,
        isPassing: form.isPassing,
        minimumFinalScore: minFin,
        description: form.description.trim() || null,
      }
      if (editId) {
        await updateLetterGradeRule(token, editId, { id: editId, ...body })
      } else {
        await createLetterGradeRule(token, {
          externalProgramId: Number(programId),
          ...body,
        })
      }
      setDialogOpen(false)
      await loadRules()
    } catch (e) {
      setFormError(e instanceof Error ? e.message : 'Kaydedilemedi.')
    } finally {
      setSaving(false)
    }
  }

  const remove = async (id) => {
    if (!window.confirm('Bu kuralı silmek istiyor musunuz?')) return
    const token = getAdminToken()
    if (!token) return
    setError('')
    try {
      await deleteLetterGradeRule(token, id)
      await loadRules()
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Silinemedi.')
    }
  }

  const columns = useMemo(
    () => [
      columnHelper.accessor((r) => rstr(r, 'letterGrade', 'LetterGrade'), { header: 'Harf' }),
      columnHelper.accessor((r) => r.minScore ?? r.MinScore, { header: 'Min' }),
      columnHelper.accessor((r) => r.maxScore ?? r.MaxScore, { header: 'Max' }),
      columnHelper.accessor((r) => (r.isPassing ?? r.IsPassing ? 'Evet' : 'Hayır'), { header: 'Geçer' }),
      columnHelper.accessor((r) => r.minimumFinalScore ?? r.MinimumFinalScore ?? '—', {
        header: 'Min. final',
      }),
      columnHelper.display({
        id: 'actions',
        header: '',
        cell: (ctx) => {
          const r = ctx.row.original
          const id = ruleKey(r)
          return (
            <div className={styles.actions}>
              <button type="button" className={styles.mini} onClick={() => openEdit(r)} title="Düzenle">
                <Pencil size={14} aria-hidden />
              </button>
              <button type="button" className={styles.miniDanger} onClick={() => void remove(id)} title="Sil">
                <Trash2 size={14} aria-hidden />
              </button>
            </div>
          )
        },
      }),
    ],
    [],
  )

  const programName =
    programs.find((p) => programKey(p) === programId)?.programName ??
    programs.find((p) => programKey(p) === programId)?.ProgramName

  return (
    <PageSection title={page.title} description={page.description} error={error}>
      <div className={styles.toolbar}>
        <div className={styles.selectWrap}>
          <label htmlFor="lg-program">Program</label>
          <select
            id="lg-program"
            className={styles.select}
            value={programId}
            onChange={(e) => setProgramId(e.target.value)}
            disabled={loadingPrograms || !programs.length}
          >
            {!programs.length ? <option value="">—</option> : null}
            {programs.map((p) => (
              <option key={programKey(p)} value={programKey(p)}>
                {p.programName ?? p.ProgramName ?? programKey(p)}
              </option>
            ))}
          </select>
        </div>
        <RefreshIconButton onClick={() => void loadRules()} loading={loadingRules} title="Yenile" />
        <button
          type="button"
          className={`${formStyles.btn} ${formStyles.btnPrimary}`}
          disabled={!programId}
          onClick={openCreate}
        >
          <Plus size={16} aria-hidden />
          Kural ekle
        </button>
      </div>

      {programId ? (
        <p className={sectionStyles.muted} style={{ marginBottom: '0.75rem', fontSize: '0.85rem' }}>
          Seçili program: <strong>{programName ?? programId}</strong>
        </p>
      ) : null}

      <DataTable
        columns={columns}
        data={rows}
        globalFilter={globalFilter}
        onGlobalFilterChange={setGlobalFilter}
        isLoading={loadingRules}
        pageSize={15}
        searchPlaceholder="Harf ara…"
      />

      <AppDialog
        open={dialogOpen}
        title={editId ? 'Kuralı düzenle' : 'Yeni kural'}
        onClose={() => setDialogOpen(false)}
        footer={
          <div className={formStyles.actions}>
            <button type="button" className={`${formStyles.btn} ${formStyles.btnGhost}`} onClick={() => setDialogOpen(false)}>
              Vazgeç
            </button>
            <button type="button" className={`${formStyles.btn} ${formStyles.btnPrimary}`} disabled={saving} onClick={() => void save()}>
              {saving ? 'Kaydediliyor…' : 'Kaydet'}
            </button>
          </div>
        }
      >
        {formError ? (
          <p className={sectionStyles.error} role="alert" style={{ marginBottom: '0.75rem' }}>
            {formError}
          </p>
        ) : null}
        <div className={formStyles.field}>
          <label htmlFor="lg-letter">Harf notu</label>
          <input
            id="lg-letter"
            className={formStyles.input}
            value={form.letterGrade}
            onChange={(e) => setForm((f) => ({ ...f, letterGrade: e.target.value }))}
            maxLength={5}
            placeholder="AA"
          />
        </div>
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '0.75rem' }}>
          <div className={formStyles.field}>
            <label htmlFor="lg-min">Min puan</label>
            <input
              id="lg-min"
              type="number"
              className={formStyles.input}
              value={form.minScore}
              onChange={(e) => setForm((f) => ({ ...f, minScore: e.target.value }))}
            />
          </div>
          <div className={formStyles.field}>
            <label htmlFor="lg-max">Max puan</label>
            <input
              id="lg-max"
              type="number"
              className={formStyles.input}
              value={form.maxScore}
              onChange={(e) => setForm((f) => ({ ...f, maxScore: e.target.value }))}
            />
          </div>
        </div>
        <label className={formStyles.check} style={{ marginBottom: '0.75rem' }}>
          <input
            type="checkbox"
            checked={form.isPassing}
            onChange={(e) => setForm((f) => ({ ...f, isPassing: e.target.checked }))}
          />
          Geçer not
        </label>
        <div className={formStyles.field}>
          <label htmlFor="lg-minf">Minimum final (opsiyonel)</label>
          <input
            id="lg-minf"
            type="number"
            className={formStyles.input}
            value={form.minimumFinalScore}
            onChange={(e) => setForm((f) => ({ ...f, minimumFinalScore: e.target.value }))}
          />
        </div>
        <div className={formStyles.field}>
          <label htmlFor="lg-desc">Açıklama</label>
          <textarea
            id="lg-desc"
            className={formStyles.textarea}
            rows={2}
            value={form.description}
            onChange={(e) => setForm((f) => ({ ...f, description: e.target.value }))}
          />
        </div>
      </AppDialog>
    </PageSection>
  )
}
