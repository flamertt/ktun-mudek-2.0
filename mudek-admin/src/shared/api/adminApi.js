import {
  deleteJsonWithAuth,
  getJson,
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
