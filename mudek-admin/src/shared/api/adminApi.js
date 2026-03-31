import {
  deleteJsonWithAuth,
  getJson,
  postFormDataWithAuth,
  postJsonWithAuth,
  putJsonWithAuth,
  requestJson,
} from './httpClient'

const ADMIN = '/api/Admin'

/** @param {string} token */
export function fetchPrograms(token) {
  return getJson(`${ADMIN}/programs`, { token })
}

/** @param {string} token */
export function fetchCourses(token, programId) {
  return getJson(`${ADMIN}/courses`, {
    token,
    query: programId ? { programId } : undefined,
  })
}

/** Tek ders (programEntityId için liste yanıtındaki courseId ile) */
export function fetchCourseById(token, courseId) {
  return getJson(`${ADMIN}/courses/${courseId}`, { token })
}

/** @param {string} token */
export function fetchTeachers(token, programId) {
  return getJson(`${ADMIN}/teachers`, {
    token,
    query: programId ? { programId } : undefined,
  })
}

/** @param {string} token */
export function fetchStudents(token, programId) {
  return getJson(`${ADMIN}/students`, {
    token,
    query: programId ? { programId } : undefined,
  })
}

/** @param {string} token */
export function fetchProgramOutcomes(token, programId) {
  return getJson(`${ADMIN}/program-outcomes`, {
    token,
    query: programId ? { programId } : undefined,
  })
}

/** @param {string} token */
export function fetchAcademicTerms(token) {
  return getJson(`${ADMIN}/academic-terms`, { token })
}

/** @param {string} token */
export function fetchActiveAcademicTerm(token) {
  return getJson(`${ADMIN}/academic-terms/active`, { token })
}

/** @param {string} token */
export function fetchCourseOfferingsActiveTerm(token) {
  return getJson(`${ADMIN}/course-offerings/active-term`, { token })
}

/** @param {string} token */
export function fetchEnrolledStudents(token, offeringId) {
  return getJson(`${ADMIN}/course-offerings/${offeringId}/students`, { token })
}

/** @param {string} token */
export function fetchCourseOffering(token, offeringId) {
  return getJson(`${ADMIN}/course-offerings/${offeringId}`, { token })
}

/** @param {string} token @param {string} studentId */
export function enrollStudentInOffering(token, offeringId, studentId) {
  return postJsonWithAuth(`${ADMIN}/course-offerings/${offeringId}/students`, { studentId }, token)
}

/** @param {string} token @param {string} status */
export function updateEnrollmentStatus(token, offeringId, studentId, status) {
  return putJsonWithAuth(
    `${ADMIN}/course-offerings/${offeringId}/students/${studentId}/status`,
    { status },
    token,
  )
}

/** @param {string} token */
export function removeEnrollment(token, offeringId, studentId) {
  return deleteJsonWithAuth(`${ADMIN}/course-offerings/${offeringId}/students/${studentId}`, token)
}

/** @param {string} token */
export function fetchCourseOfferingsByTerm(token, termId) {
  return getJson(`${ADMIN}/course-offerings/by-term/${termId}`, { token })
}

/** @param {string} token */
export function setActiveAcademicTerm(token, termId) {
  return requestJson('PUT', `${ADMIN}/academic-terms/${termId}/set-active`, { token })
}

// ——— Program ———

export function createProgram(token, body) {
  return postJsonWithAuth(`${ADMIN}/programs`, body, token)
}

export function updateProgram(token, id, body) {
  return putJsonWithAuth(`${ADMIN}/programs/${id}`, body, token)
}

export function deleteProgram(token, id) {
  return deleteJsonWithAuth(`${ADMIN}/programs/${id}`, token)
}

// ——— Program outcome ———

export function createProgramOutcome(token, body) {
  return postJsonWithAuth(`${ADMIN}/program-outcomes`, body, token)
}

export function updateProgramOutcome(token, id, body) {
  return putJsonWithAuth(`${ADMIN}/program-outcomes/${id}`, body, token)
}

export function deleteProgramOutcome(token, id) {
  return deleteJsonWithAuth(`${ADMIN}/program-outcomes/${id}`, token)
}

// ——— Course ———

export function createCourse(token, body) {
  return postJsonWithAuth(`${ADMIN}/courses`, body, token)
}

export function updateCourse(token, id, body) {
  return putJsonWithAuth(`${ADMIN}/courses/${id}`, body, token)
}

export function deleteCourse(token, id) {
  return deleteJsonWithAuth(`${ADMIN}/courses/${id}`, token)
}

// ——— Academic term ———

export function createAcademicTerm(token, body) {
  return postJsonWithAuth(`${ADMIN}/academic-terms`, body, token)
}

export function updateAcademicTerm(token, id, body) {
  return putJsonWithAuth(`${ADMIN}/academic-terms/${id}`, body, token)
}

export function deleteAcademicTerm(token, id) {
  return deleteJsonWithAuth(`${ADMIN}/academic-terms/${id}`, token)
}

// ——— Teacher ———

export function createTeacher(token, body) {
  return postJsonWithAuth(`${ADMIN}/teachers`, body, token)
}

export function updateTeacher(token, id, body) {
  return putJsonWithAuth(`${ADMIN}/teachers/${id}`, body, token)
}

export function deleteTeacher(token, id) {
  return deleteJsonWithAuth(`${ADMIN}/teachers/${id}`, token)
}

// ——— Student ———

export function createStudent(token, body) {
  return postJsonWithAuth(`${ADMIN}/students`, body, token)
}

export function updateStudent(token, id, body) {
  return putJsonWithAuth(`${ADMIN}/students/${id}`, body, token)
}

export function deleteStudent(token, id) {
  return deleteJsonWithAuth(`${ADMIN}/students/${id}`, token)
}

// ——— Program (tek kayıt) ———

/** @param {string} token @param {string} id */
export function fetchProgramById(token, id) {
  return getJson(`${ADMIN}/programs/${id}`, { token })
}

// ——— Ders öğrenme çıktıları (CLO) ———

/** @param {string} token @param {string} courseId */
export function fetchCourseClos(token, courseId) {
  return getJson(`${ADMIN}/courses/${courseId}/clos`, { token })
}

/** @param {string} token @param {string} courseId @param {string} cloId */
export function fetchCloById(token, courseId, cloId) {
  return getJson(`${ADMIN}/courses/${courseId}/clos/${cloId}`, { token })
}

/** @param {string} token @param {string} courseId @param {Record<string, unknown>} body */
export function createClo(token, courseId, body) {
  return postJsonWithAuth(`${ADMIN}/courses/${courseId}/clos`, body, token)
}

/** @param {string} token @param {string} courseId @param {string} cloId @param {Record<string, unknown>} body */
export function updateClo(token, courseId, cloId, body) {
  return putJsonWithAuth(`${ADMIN}/courses/${courseId}/clos/${cloId}`, body, token)
}

/** @param {string} token @param {string} courseId @param {string} cloId */
export function deleteClo(token, courseId, cloId) {
  return deleteJsonWithAuth(`${ADMIN}/courses/${courseId}/clos/${cloId}`, token)
}

// ——— CLO ↔ program çıktısı (PÇ) eşlemesi ———

/** @param {string} token @param {string} courseId @param {string} cloId */
export function fetchCloProgramOutcomes(token, courseId, cloId) {
  return getJson(`${ADMIN}/courses/${courseId}/clos/${cloId}/program-outcomes`, { token })
}

/** @param {string} token @param {string} courseId */
export function fetchCloPoMapsForCourse(token, courseId) {
  return getJson(`${ADMIN}/courses/${courseId}/clo-po-maps`, { token })
}

/** @param {string} token @param {string} courseId @param {string} cloId @param {Record<string, unknown>} body */
export function linkCloToProgramOutcome(token, courseId, cloId, body) {
  return postJsonWithAuth(`${ADMIN}/courses/${courseId}/clos/${cloId}/program-outcomes`, body, token)
}

/** @param {string} token @param {string} courseId @param {string} cloId @param {string} programOutcomeId @param {Record<string, unknown>} body */
export function updateCloProgramOutcomeWeight(token, courseId, cloId, programOutcomeId, body) {
  return putJsonWithAuth(
    `${ADMIN}/courses/${courseId}/clos/${cloId}/program-outcomes/${programOutcomeId}/weight`,
    body,
    token,
  )
}

/** @param {string} token @param {string} courseId @param {string} cloId @param {string} programOutcomeId */
export function unlinkCloFromProgramOutcome(token, courseId, cloId, programOutcomeId) {
  return deleteJsonWithAuth(
    `${ADMIN}/courses/${courseId}/clos/${cloId}/program-outcomes/${programOutcomeId}`,
    token,
  )
}

// ——— Akademik dönem (tek kayıt) ———

/** @param {string} token @param {string} id */
export function fetchAcademicTermById(token, id) {
  return getJson(`${ADMIN}/academic-terms/${id}`, { token })
}

// ——— Ders açılışı (course offering) ———

/** @param {string} token */
export function fetchAllCourseOfferings(token) {
  return getJson(`${ADMIN}/course-offerings`, { token })
}

/** @param {string} token @param {string} teacherId @param {string} [termId] */
export function fetchCourseOfferingsByTeacher(token, teacherId, termId) {
  return getJson(`${ADMIN}/course-offerings/by-teacher/${teacherId}`, {
    token,
    query: termId ? { termId } : undefined,
  })
}

/** @param {string} token @param {string} courseId */
export function fetchCourseOfferingsByCourse(token, courseId) {
  return getJson(`${ADMIN}/course-offerings/by-course/${courseId}`, { token })
}

/** @param {string} token @param {Record<string, unknown>} body */
export function createCourseOffering(token, body) {
  return postJsonWithAuth(`${ADMIN}/course-offerings`, body, token)
}

/** @param {string} token @param {string} offeringId @param {Record<string, unknown>} body */
export function updateCourseOffering(token, offeringId, body) {
  return putJsonWithAuth(`${ADMIN}/course-offerings/${offeringId}`, body, token)
}

/** @param {string} token @param {string} offeringId @param {Record<string, unknown>} body */
export function assignTeacherToCourseOffering(token, offeringId, body) {
  return putJsonWithAuth(`${ADMIN}/course-offerings/${offeringId}/assign-teacher`, body, token)
}

/** @param {string} token @param {string} offeringId */
export function removeTeacherFromCourseOffering(token, offeringId) {
  return deleteJsonWithAuth(`${ADMIN}/course-offerings/${offeringId}/remove-teacher`, token)
}

/** @param {string} token @param {string} offeringId */
export function deleteCourseOffering(token, offeringId) {
  return deleteJsonWithAuth(`${ADMIN}/course-offerings/${offeringId}`, token)
}

// ——— Kayıt (toplu / Excel) ———

/** @param {string} token @param {string} offeringId @param {Record<string, unknown>} body */
export function bulkEnrollStudents(token, offeringId, body) {
  return postJsonWithAuth(`${ADMIN}/course-offerings/${offeringId}/students/bulk`, body, token)
}

/**
 * @param {string} token
 * @param {string} offeringId
 * @param {File} file
 */
export function importEnrollmentsFromExcel(token, offeringId, file) {
  const formData = new FormData()
  formData.append('file', file)
  return postFormDataWithAuth(
    `${ADMIN}/course-offerings/${offeringId}/students/import`,
    formData,
    token,
  )
}

// ——— Ders değerlendirmeleri (salt okunur) ———

/** @param {string} token @param {string} offeringId */
export function fetchCourseOfferingEvaluation(token, offeringId) {
  return getJson(`${ADMIN}/course-offerings/${offeringId}/evaluation`, { token })
}

/** @param {string} token @param {string} id */
export function fetchCourseEvaluationById(token, id) {
  return getJson(`${ADMIN}/course-evaluations/${id}`, { token })
}

/** @param {string} token */
export function fetchCourseEvaluations(token) {
  return getJson(`${ADMIN}/course-evaluations`, { token })
}

// ——— Öğretmen / öğrenci (tek kayıt) ———

/** @param {string} token @param {string} id */
export function fetchTeacherById(token, id) {
  return getJson(`${ADMIN}/teachers/${id}`, { token })
}

/** @param {string} token @param {string} id */
export function fetchStudentById(token, id) {
  return getJson(`${ADMIN}/students/${id}`, { token })
}
