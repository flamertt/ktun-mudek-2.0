import { Navigate, useParams } from 'react-router-dom'

/**
 * Derslerim akışında ara sayfa yok; eski /courses/:id bağlantıları öğrenci listesine yönlendirilir.
 */
export function CourseDetailPage() {
  const { offeringId } = useParams()
  if (!offeringId) return <Navigate to="/courses" replace />
  return <Navigate to={`/courses/${offeringId}/students`} replace />
}
