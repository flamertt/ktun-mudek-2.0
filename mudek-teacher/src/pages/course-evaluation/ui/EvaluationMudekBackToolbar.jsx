import { ArrowLeft } from 'lucide-react'
import { useNavigate, useParams } from 'react-router-dom'

import formStyles from '../../../shared/ui/admin-form/AdminForm.module.css'
import styles from './EvaluationMudekBackToolbar.module.css'

/** MÜDEK alt sonuç sayfalarından `/evaluations/:offeringId` özetine dönüş. */
export function EvaluationMudekBackToolbar() {
  const { offeringId } = useParams()
  const navigate = useNavigate()
  if (!offeringId) return null
  return (
    <div className={styles.toolbar}>
      <button
        type="button"
        className={`${formStyles.btn} ${formStyles.btnGhost}`}
        onClick={() => navigate(`/evaluations/${offeringId}`)}
      >
        <ArrowLeft size={16} aria-hidden />
        Ders değerlendirmesi
      </button>
    </div>
  )
}
