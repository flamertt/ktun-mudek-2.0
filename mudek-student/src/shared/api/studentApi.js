import { getJson, postJsonWithAuth } from './httpClient'

const STUDENT = '/api/Student'

export function fetchStudentCourses(token) {
  return getJson(`${STUDENT}/my-courses`, { token })
}

export function fetchStudentSurveysForOffering(token, offeringId) {
  return getJson(`${STUDENT}/my-courses/${offeringId}/surveys`, { token })
}

export function fetchStudentSurveyDetail(token, surveyId) {
  return getJson(`${STUDENT}/surveys/${surveyId}`, { token })
}

export function submitStudentSurvey(token, surveyId, body) {
  return postJsonWithAuth(`${STUDENT}/surveys/${surveyId}/submit`, body, token)
}
