# MÜDEK Bitirme API — Endpoint referansı

Bu belge, `BitirmeApi.Presentation` içindeki controller’ların **tam yolunu**, **HTTP metodunu**, **kimlik doğrulamayı**, **sorgu/route parametrelerini**, **istek gövdelerini** ve **tipik JSON yanıt şekillerini** özetler. Üretimde varsayılan JSON serileştirme **camelCase** kullanır (`externalId`, `courseOfferingId`, …).

---

## 1. Genel bilgiler

| Konu | Açıklama |
|------|-----------|
| **Taban yol** | Controller adı `api/[ControllerName]` şeklindedir. Örnek: `api/Teacher`, `api/Student`, `api/Admin`. |
| **Kimlik doğrulama** | `Authorization: Bearer <üniversite JWT>` — üniversite API’sinin login yanıtındaki token olduğu gibi iletilir; sunucu imza doğrulaması yapmaz, süre (`ValidateLifetime`) kontrol edilir. |
| **Login uçları** | `[Authorize]` **yok**; gövde ile giriş yapılır. |
| **Korumalı uçlar** | `Admin`, `Teacher`, `Student` controller’ları `[Authorize]` ile korunur; geçerli herhangi bir JWT kabul edilir (rol bazlı policy tanımlı değildir). **Yetkilendirme iş kurallarında** yapılır (ör. öğretmen sadece kendi değerlendirmesi). |
| **Öğretmen / öğrenci kimliği** | JWT içindeki `nameidentifier` / `sub` / `nameid` claim’i **tam sayı** olarak okunur (`ExternalTeacherId` / `ExternalStudentId`). |
| **Üniversite proxy notu** | Bazı entegrasyon çağrılarında geçici sabitler kullanılabilir (ör. öğretmen ders listesi isteğinde e-posta, öğrenci ders listesinde `studentId`). Üretim öncesi kod ve yapılandırma ile doğrulanmalıdır. |

**Hata gövdesi (yaygın):** `{ "message": "..." }` veya ASP.NET **400** için `ModelState` (alan doğrulama hataları).  
**403 Forbidden:** `UnauthorizedAccessException` (ör. kaynak başka kullanıcıya ait).  
**401 Unauthorized:** Eksik/geçersiz token (JWT middleware).

---

## 2. Kimlik doğrulama (`*AuthController`)

Üç ayrı controller aynı `LoginDto` ile üniversiteye bağlanır; yanıt yapısı aynıdır. Fark sadece **route** ve iş mantığında log/ileride ayrım için ayrılmış olabilir.

### 2.1 `POST /api/AdminAuth/login`

| Alan | Tip | Zorunlu |
|------|-----|---------|
| `email` | string | Evet |
| `password` | string | Evet |

**200 OK** — `AuthResponseDto`:

```json
{
  "token": "<üniversite JWT>",
  "user": {
    "externalId": 0,
    "fullName": "",
    "email": "",
    "role": "",
    "studentNumber": null,
    "title": null
  },
  "message": "Giriş başarılı"
}
```

**401 / 403** — `{ "message": "..." }` (`AuthResult` içindeki mesaj; durum kodu `StatusCode` ile uyumlu).

### 2.2 `POST /api/TeacherAuth/login`

Gövde ve yanıt: **2.1 ile aynı** (`LoginDto` → `AuthResponseDto`).

### 2.3 `POST /api/StudentAuth/login`

Gövde ve yanıt: **2.1 ile aynı**.

---

## 3. Admin (`/api/Admin`)

Tüm uçlar: **`Authorization: Bearer`** gerekli. Üniversiteye giden proxy çağrıları bu token ile yapılır.

### 3.1 Üniversite — salt okuma / senkron

| Method | Path | Parametre | Yanıt |
|--------|------|-----------|--------|
| GET | `university/programs` | — | `UniversityProgramDto[]`: `{ programId, programName }` |
| GET | `university/academic-terms` | — | `UniversityAcademicTermDto[]`: `{ academicTermId, academicTermName }` (JSON’da üniversite `id`/`ad` alanlarından map edilir) |
| GET | `university/academic-terms/active` | — | Tek `UniversityAcademicTermDto` veya **404** `{ message }` |
| POST | `university/academic-terms/sync` | — | **200** `{ "message": "...", "term": { "id", "ad" } }` — `term`, `AcademicTerm` entity (DB’ye yazılan son kayıt) |
| GET | `active-term` | — | **200** `{ "id", "ad" }` (`AcademicTerm`) veya **404** sync gerekir mesajı |
| GET | `university/programs/{programId}/outcomes` | route: `programId` (int) | `UniversityProgramOutcomeDto[]`: `{ programOutcomeId, programOutcomeCode, description }` |
| GET | `university/courses/{courseId}/clos` | route: `courseId` (int) | `UniversityCloDto[]`: `{ cloId, description }` |
| GET | `university/courses/{courseId}/clo-po-map` | route: `courseId` (int) | `UniversityCloPloMapDto[]`: `{ cloId, programOutcomeId, weight }` |

Hatalarda çoğunlukla **400** `{ "message": ex.Message }`.

### 3.2 Ders değerlendirme (yerel DB — admin görünümü)

| Method | Path | Yanıt |
|--------|------|--------|
| GET | `course-evaluations` | `CourseEvaluationListDto[]` |
| GET | `course-evaluations/{id}` | `CourseEvaluationDetailDto` veya **404** |
| GET | `course-evaluations/by-offering/{externalCourseOfferingId}` | `CourseEvaluationDetailDto` veya **404** `{ message }` |
| GET | `course-evaluations/by-teacher/{externalTeacherId}` | `CourseEvaluationListDto[]` |

**`CourseEvaluationListDto`:** `id`, `externalCourseOfferingId`, `externalCourseId`, `externalProgramId`, `externalTeacherId`, `courseCode`, `courseName`, `academicTermName`, `createdDate`, `updatedAt`, `lastCalculatedAt`, `isCalculationDirty`.

**`CourseEvaluationDetailDto`:** Liste alanları + `studentFeedbackEvaluation`, `programOutcomeEvaluation`, `generalEvaluation`, `improvementSuggestions`.

### 3.3 Harf notu kuralları (`LetterGradeRule`)

| Method | Path | Gövde | Yanıt / notlar |
|--------|------|--------|----------------|
| GET | `letter-grade-rules` | — | `LetterGradeRuleDto[]` |
| GET | `programs/{externalProgramId}/letter-grade-rules` | route: int | `LetterGradeRuleDto[]` |
| GET | `letter-grade-rules/{id}` | route: guid | `LetterGradeRuleDto` veya **404** |
| POST | `letter-grade-rules` | `CreateLetterGradeRuleDto` | **201** + `LetterGradeRuleDto` (`CreatedAtAction`) |
| PUT | `letter-grade-rules/{id}` | `UpdateLetterGradeRuleDto` (gövde `id` = route) | **200** `LetterGradeRuleDto`; **404** / **409** |
| DELETE | `letter-grade-rules/{id}` | — | **204** veya **404** |

**`CreateLetterGradeRuleDto`:** `externalProgramId`, `letterGrade`, `minScore`, `maxScore`, `isPassing`, `minimumFinalScore?`, `description?`.

**`UpdateLetterGradeRuleDto`:** `id`, `letterGrade`, `minScore`, `maxScore`, `isPassing`, `minimumFinalScore?`, `description?`.

**`LetterGradeRuleDto`:** Yukarıdakiler + `id`, `createdAt`.

---

## 4. Öğretmen (`/api/Teacher`)

Tüm uçlar: **`Authorization: Bearer`** + JWT’den öğretmen ID.

### 4.1 Akademik dönem ve üniversite ders verileri

| Method | Path | Query / route | Yanıt |
|--------|------|---------------|--------|
| GET | `academic-terms` | — | `UniversityAcademicTermDto[]` |
| GET | `my-courses` | — | `UniversityCourseOfferingDto[]` (aktif dönem = üniversite listesinde en büyük id) |
| GET | `my-courses/academic-terms` | **`termId`** (int, zorunlu) | `UniversityCourseOfferingDto[]` |
| GET | `my-courses/{offeringId}/detail` | route: int | `UniversityCourseOfferingDetailDto` veya **404** |
| GET | `my-courses/{offeringId}/students` | route: int | `UniversityStudentDto[]` (`studentId`, `studentNumber`, `fullName`) veya **404** |
| GET | `my-courses/{offeringId}/clos` | route: int | `UniversityCloDto[]` |
| GET | `my-courses/{offeringId}/program-outcomes` | route: int | `UniversityProgramOutcomeDto[]` |
| GET | `courses/{courseId}/clo-po-map` | route: int | `UniversityCloPloMapDto[]` |

**`UniversityCourseOfferingDto`:** `courseOfferingId`, `courseId`, `courseCode`, `courseName`, `programId`.

### 4.2 Course evaluation (MÜDEK değerlendirme kaydı)

| Method | Path | Gövde | Yanıt |
|--------|------|--------|--------|
| GET | `my-evaluations` | — | `CourseEvaluationListDto[]` |
| GET | `my-courses/{offeringId}/evaluation` | — | `CourseEvaluationDetailDto` veya **404** |
| POST | `my-courses/{offeringId}/evaluation` | `CourseEvaluationCreateDto` (`externalCourseOfferingId` route ile ezilir) | **201** `CourseEvaluationDetailDto` |
| PUT | `evaluations/{evaluationId}` | `CourseEvaluationUpdateDto` (`id` route ile ezilir) | **200** `CourseEvaluationDetailDto` |
| DELETE | `evaluations/{evaluationId}` | — | **200** `{ "message": "Değerlendirme silindi." }` |

**`CourseEvaluationCreateDto`:** `externalCourseId`, `externalProgramId`, `courseCode?`, `courseName?`, `academicTermName?`, metin alanları opsiyonel.  
**`CourseEvaluationUpdateDto`:** `id`, dört metin alanı opsiyonel.

### 4.3 Sınav (Exam)

| Method | Path | Gövde | Yanıt |
|--------|------|--------|--------|
| GET | `evaluations/{evaluationId}/exams` | — | `ExamListDto[]` |
| GET | `exams/{examId}` | — | `ExamDetailDto` veya **404** |
| POST | `evaluations/{evaluationId}/exams` | `CreateExamDto` | **201** `ExamDetailDto` (oluşturma sonrası detay tipi servisten gelir) |
| PUT | `exams/{examId}` | `UpdateExamDto` | **200** `ExamDetailDto` |
| DELETE | `exams/{examId}` | — | **200** `{ "message": "Sınav silindi." }` |

**`ExamListDto`:** `id`, `courseEvaluationId`, `examType`, `weightPercentage`, `orderIndex`, `questionCount`.  
**`ExamDetailDto`:** Liste alanları + `questions: ExamQuestionDto[]`.  
**`CreateExamDto` / `UpdateExamDto`:** Kodda `examType`, `weightPercentage`, `orderIndex`; id’ler route ile set edilir.

### 4.4 Soru (ExamQuestion)

| Method | Path | Gövde | Yanıt |
|--------|------|--------|--------|
| GET | `exams/{examId}/questions` | — | `ExamQuestionDto[]` |
| GET | `exam-questions/{questionId}` | — | `ExamQuestionDto` veya **404** |
| POST | `exams/{examId}/questions` | `CreateExamQuestionDto` | **201** `ExamQuestionDto` |
| PUT | `exam-questions/{questionId}` | `UpdateExamQuestionDto` | **200** `ExamQuestionDto` |
| DELETE | `exam-questions/{questionId}` | — | **200** `{ "message": "Soru silindi." }` |

**`ExamQuestionDto`:** `id`, `examId`, `questionNumber`, `maxScore`, `title`, `description`, `questionType`, `outcomeMappings: ExamQuestionOutcomeMappingDto[]`.  
**`CreateExamQuestionDto`:** `questionNumber`, `maxScore` (varsayılan 100), `title?`, `description?`, `questionType?`.

### 4.5 Soru–DÖÇ eşlemesi

| Method | Path | Gövde | Yanıt |
|--------|------|--------|--------|
| GET | `exam-questions/{questionId}/clos` | — | `ExamQuestionOutcomeMappingDto[]` |
| POST | `exam-questions/{questionId}/clos` | `CreateExamQuestionOutcomeMappingDto` | **200** `ExamQuestionOutcomeMappingDto` |
| PUT | `exam-question-outcome-mappings/{mappingId}` | `UpdateExamQuestionOutcomeMappingDto` (`weight`) | **200** mapping DTO |
| DELETE | `exam-question-outcome-mappings/{mappingId}` | — | **200** `{ "message": "Mapping silindi." }` |

**`CreateExamQuestionOutcomeMappingDto`:** `externalCloId`, `weight`, `cloCode?`, `cloDescription?`.

### 4.6 Assessment component

| Method | Path | Gövde | Yanıt |
|--------|------|--------|--------|
| GET | `exams/{examId}/components` | — | `AssessmentComponentListDto[]` |
| GET | `assessment-components/{componentId}` | — | `AssessmentComponentDto` veya **404** |
| POST | `exams/{examId}/components` | `CreateAssessmentComponentDto` | **200** `AssessmentComponentDto` |
| PUT | `assessment-components/{componentId}` | `UpdateAssessmentComponentDto` | **200** `AssessmentComponentDto` |
| DELETE | `assessment-components/{componentId}` | — | **200** `{ "message": "Component silindi." }` |

**`CreateAssessmentComponentDto`:** `name`, `componentType`, `maxScore`, `weightPercentage?`, `orderIndex`, `description?`.  
**`UpdateAssessmentComponentDto`:** + `isActive`.

### 4.7 Bileşen–DÖÇ eşlemesi

| Method | Path | Gövde | Yanıt |
|--------|------|--------|--------|
| GET | `assessment-components/{componentId}/clos` | — | `AssessmentComponentOutcomeMappingDto[]` |
| POST | `assessment-components/{componentId}/clos` | `CreateAssessmentComponentOutcomeMappingDto` | **200** DTO |
| PUT | `assessment-component-outcome-mappings/{mappingId}` | `UpdateAssessmentComponentOutcomeMappingDto` | **200** DTO |
| DELETE | `assessment-component-outcome-mappings/{mappingId}` | — | **200** `{ "message": "Mapping silindi." }` |

### 4.8 Öğrenci cevapları (yazılı soru puanları)

| Method | Path | Gövde | Yanıt |
|--------|------|--------|--------|
| GET | `exam-questions/{questionId}/answers` | — | `StudentAnswerDto[]` |
| POST | `exam-questions/{questionId}/answers` | `CreateStudentAnswerDto` | **200** `StudentAnswerDto` |
| POST | `exam-questions/{questionId}/answers/bulk` | `BulkStudentAnswerRequestDto` | **200** `BulkOperationResultDto<int>` |
| PUT | `student-answers/{answerId}` | `UpdateStudentAnswerDto` | **200** `StudentAnswerDto` |
| DELETE | `student-answers/{answerId}` | — | **200** `{ "message": "Answer silindi." }` |

**`BulkStudentAnswerRequestDto`:** `items: { externalStudentId, score }[]`.  
**`BulkOperationResultDto<int>`:** `success: int[]`, `failed: int[]`, `errors: string[]` (öğrenci id’leri ile sonuç özeti).

### 4.9 Bileşen puanları (lab/ödev vb.)

| Method | Path | Gövde | Yanıt |
|--------|------|--------|--------|
| GET | `assessment-components/{componentId}/scores` | — | `StudentAssessmentComponentScoreDto[]` |
| POST | `assessment-components/{componentId}/scores` | `CreateStudentAssessmentComponentScoreDto` | **200** DTO |
| POST | `assessment-components/{componentId}/scores/bulk` | `BulkStudentScoreDto` | **200** `BulkOperationResultDto<int>` |
| PUT | `student-assessment-component-scores/{scoreId}` | `UpdateStudentAssessmentComponentScoreDto` | **200** DTO |
| DELETE | `student-assessment-component-scores/{scoreId}` | — | **200** `{ "message": "Score silindi." }` |

**`BulkStudentScoreDto`:** `scores: { externalStudentId, score?, notes? }[]`.

### 4.10 MÜDEK hesaplama ve özet

| Method | Path | Gövde | Yanıt |
|--------|------|--------|--------|
| GET | `my-courses/{offeringId}/mudek-evaluation/results` | — | `MudekEvaluationSnapshotDto` veya **404** |
| POST | `my-courses/{offeringId}/mudek-evaluation/calculate` | — | **200** `MudekEvaluationSnapshotDto` |
| POST | `my-courses/{offeringId}/mudek-evaluation/recalculate` | — | **200** `MudekEvaluationSnapshotDto` (calculate ile aynı mantık) |
| POST | `evaluations/{evaluationId}/mudek-evaluation/calculate` | — | **200** snapshot; değerlendirme yoksa **404**; sahip değilse **403** |
| GET | `evaluations/{evaluationId}/full-detail` | — | **200** özel bileşik nesne (aşağıda) |

**`MudekEvaluationSnapshotDto`:**  
`externalCourseOfferingId`, `courseEvaluationId?`, `lastCalculatedAt?`, `isCalculationDirty`,  
`studentResults[]` (`StudentEvaluationResultDto`),  
`examSummaries[]` (`ExamEvaluationResultDto`),  
`questionAndComponentResults[]` (`ExamQuestionEvaluationResultDto`),  
`itemCloAchievements[]` (`ExamAssessmentItemCloAchievementDto`),  
`cloResults[]` (`CloEvaluationResultDto`),  
`programOutcomeResults[]` (`ProgramOutcomeEvaluationResultDto`).

**`GET evaluations/{evaluationId}/full-detail` yanıtı:**

```json
{
  "evaluationId": "00000000-0000-0000-0000-000000000000",
  "exams": [ "ExamListDto..." ],
  "componentsByExam": {
    "<guid>": [ "AssessmentComponentListDto..." ]
  }
}
```

(`componentsByExam` anahtarları sınav `Id` guid string’idir.)

### 4.11 Anket (öğretmen)

| Method | Path | Gövde | Yanıt |
|--------|------|--------|--------|
| GET | `my-courses/{offeringId}/surveys` | — | `SurveyListDto[]` |
| GET | `surveys/{surveyId}` | — | `SurveyDetailDto` veya **404** |
| POST | `surveys` | `CreateSurveyDto` | **201** `SurveyDetailDto` |
| PUT | `surveys/{surveyId}` | `UpdateSurveyDto` (`id` = route) | **200** `SurveyDetailDto` |
| DELETE | `surveys/{surveyId}` | — | **204** veya **400** |
| PATCH | `surveys/{surveyId}/toggle-active` | — | **204** |
| POST | `surveys/{surveyId}/questions` | `CreateSurveyQuestionDto` (`surveyId` = route) | **200** `SurveyQuestionDto` |
| PUT | `surveys/{surveyId}/questions/{questionId}` | `UpdateSurveyQuestionDto` | **200** `SurveyQuestionDto` |
| DELETE | `surveys/{surveyId}/questions/{questionId}` | — | **204** |
| GET | `surveys/{surveyId}/results` | — | `SurveyResultsDto` |

**`SurveyListDto`:** `id`, `externalCourseOfferingId`, `title`, `description`, `isActive`, `createdAt`, `questionCount`, `submissionCount`.  
**`SurveyDetailDto`:** + `questions: SurveyQuestionDto[]`.  
**`SurveyResultsDto`:** `surveyId`, `title`, katılım sayıları, `questions: SurveyQuestionResultDto[]`, `cloResults: CloSurveyResultDto[]`.  
**`SurveyQuestionResultDto`:** `scoreDistribution` bir **sözlük** (`{ "0": 5, "1": 3, ... }` gibi).

---

## 5. Öğrenci (`/api/Student`)

| Method | Path | Gövde | Yanıt |
|--------|------|--------|--------|
| GET | `my-courses` | — | `StudentCourseDto[]` — aktif dönem **DB**’den (`AcademicTerms`); yoksa **400** sync uyarısı |
| GET | `my-courses/{offeringId}/surveys` | — | `StudentSurveyListDto[]` — kayıt üniversite listesi + yerel doğrulama |
| GET | `surveys/{surveyId}` | — | `StudentSurveyDetailDto` |
| POST | `surveys/{surveyId}/submit` | `SubmitSurveyDto` | **201** `StudentSubmissionResultDto` |

**`StudentCourseDto`:** `externalCourseOfferingId`, `externalCourseId`, `courseCode`, `courseName`, `externalProgramId`, `activeSurveyCount`.

**`SubmitSurveyDto`:** `answers: [{ questionId, valueNumeric }]` — `valueNumeric` doğrulamada **0–10** aralığında.

**`StudentSubmissionResultDto`:** `submissionId`, `surveyId`, `answeredQuestions`, `submittedAt`.

---

## 6. HTTP durum kodları — hızlı referans

| Kod | Ne zaman |
|-----|----------|
| 200 | Başarılı okuma/güncelleme (çoğu PUT/POST) |
| 201 | Oluşturma (login yanıtı, bazı `CreatedAtAction` uçları) |
| 204 | Silme / toggle başarılı gövdesiz |
| 400 | Geçersiz model, iş kuralı (`InvalidOperationException`), öğrenci aktif dönem yok |
| 401 | Token yok/geçersiz |
| 403 | Kaynak sahipliği yok |
| 404 | Kayıt yok |
| 409 | Çakışma (ör. harf notu kuralı, sınav/soru tekilliği) |

---

## 7. Swagger

Geliştirme ortamında `/swagger` üzerinden **MÜDEK API v1** şeması ve **Bearer** tanımı ile deneme yapılabilir.

---

*Bu dosya, mevcut kod tabanına göre üretilmiştir; üniversite API URL/sabitleri veya JWT claim yapısı değişirse entegrasyon katmanı ile birlikte güncellenmelidir.*
