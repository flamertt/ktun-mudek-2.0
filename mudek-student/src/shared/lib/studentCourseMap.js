/**
 * BitirmeApi StudentCourseDto: externalCourseOfferingId, externalCourseId, courseCode, courseName, externalProgramId, activeSurveyCount
 * (Eski / üniversite ham yanıtları için courseOfferingId vb. yedekler.)
 */
export function getStudentCourseOfferingId(row) {
  if (row == null || typeof row !== 'object') return ''
  const v =
    row.externalCourseOfferingId ??
    row.ExternalCourseOfferingId ??
    row.courseOfferingId ??
    row.CourseOfferingId ??
    row.id ??
    row.Id
  return v != null && v !== '' ? String(v) : ''
}
