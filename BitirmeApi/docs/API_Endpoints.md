# API Endpointleri

Bu doküman, `BitirmeApi.Presentation/Controllers` altındaki controller’ların tamamındaki endpointleri listeler.

## Ortak bilgiler

- Base route: `api/[controller]`
- Authorization:
  - `AdminController`: `Policy = "AdminOnly"`
  - `TeacherController`: `Policy = "TeacherOnly"`
  - Auth controller’larda (AdminAuth/TeacherAuth/StudentAuth) ek `Authorize` yoktur.

## AdminAuthController

Base route: `POST /api/AdminAuth/login`

| Method | Route | Açıklama |
|---|---|---|
| POST | `/api/AdminAuth/login` | Admin giriş |

## TeacherAuthController

Base route: `POST /api/TeacherAuth/login`

| Method | Route | Açıklama |
|---|---|---|
| POST | `/api/TeacherAuth/login` | Öğretmen giriş |

## StudentAuthController

Base route: `POST /api/StudentAuth/login`

| Method | Route | Açıklama |
|---|---|---|
| POST | `/api/StudentAuth/login` | Öğrenci giriş |

## AdminController (`Policy = "AdminOnly"`)

Base route: `api/Admin`

### Programlar ve çıktılar

| Method | Route | Açıklama |
|---|---|---|
| GET | `/api/Admin/programs` | Programları listeler |
| GET | `/api/Admin/programs/{id}` | Tek program |
| POST | `/api/Admin/programs` | Program oluştur |
| PUT | `/api/Admin/programs/{id}` | Program güncelle |
| DELETE | `/api/Admin/programs/{id}` | Program sil |
| GET | `/api/Admin/program-outcomes?programId={programId}` | Program çıktıları (filtreli/filtre yok) |
| POST | `/api/Admin/program-outcomes` | Program çıktısı oluştur |
| PUT | `/api/Admin/program-outcomes/{id}` | Program çıktısı güncelle |
| DELETE | `/api/Admin/program-outcomes/{id}` | Program çıktısı sil |

### Ders kataloğu

| Method | Route | Açıklama |
|---|---|---|
| GET | `/api/Admin/courses?programId={programId}` | Dersler (filtreli/filtre yok) |
| GET | `/api/Admin/courses/{id}` | Tek ders |
| POST | `/api/Admin/courses` | Ders oluştur |
| PUT | `/api/Admin/courses/{id}` | Ders güncelle |
| DELETE | `/api/Admin/courses/{id}` | Ders sil |
| GET | `/api/Admin/courses/{courseId}/clos` | Bir derse ait tüm CLO’lar |
| GET | `/api/Admin/courses/{courseId}/clos/{cloId}` | Tek CLO |
| POST | `/api/Admin/courses/{courseId}/clos` | CLO oluştur |
| PUT | `/api/Admin/courses/{courseId}/clos/{cloId}` | CLO güncelle |
| DELETE | `/api/Admin/courses/{courseId}/clos/{cloId}` | CLO sil |

### CLO -> PÇ (PO) eşlemesi

| Method | Route | Açıklama |
|---|---|---|
| GET | `/api/Admin/courses/{courseId}/clos/{cloId}/program-outcomes` | Bir CLO’nun bağlı PÇ’leri |
| GET | `/api/Admin/courses/{courseId}/clo-po-maps` | Tüm CLO->PO map’leri |
| POST | `/api/Admin/courses/{courseId}/clos/{cloId}/program-outcomes` | CLO’yu PÇ ile eşle |
| PUT | `/api/Admin/courses/{courseId}/clos/{cloId}/program-outcomes/{programOutcomeId}/weight` | Eşleme weight güncelle |
| DELETE | `/api/Admin/courses/{courseId}/clos/{cloId}/program-outcomes/{programOutcomeId}` | Eşlemeyi kaldır |

### Akademik dönem

| Method | Route | Açıklama |
|---|---|---|
| GET | `/api/Admin/academic-terms` | Dönemleri listeler |
| GET | `/api/Admin/academic-terms/active` | Aktif dönem |
| GET | `/api/Admin/academic-terms/{id}` | Tek dönem |
| POST | `/api/Admin/academic-terms` | Dönem oluştur |
| PUT | `/api/Admin/academic-terms/{id}` | Dönem güncelle |
| PUT | `/api/Admin/academic-terms/{id}/set-active` | Aktif dönemi set et |
| DELETE | `/api/Admin/academic-terms/{id}` | Dönem sil |

### Ders açılışı (Course Offering)

| Method | Route | Açıklama |
|---|---|---|
| GET | `/api/Admin/course-offerings` | Tüm course offering’ler |
| GET | `/api/Admin/course-offerings/active-term` | Aktif dönem offering’leri |
| GET | `/api/Admin/course-offerings/by-term/{termId}` | Döneme göre offering |
| GET | `/api/Admin/course-offerings/by-teacher/{teacherId}?termId={termId}` | Öğretmene göre offering |
| GET | `/api/Admin/course-offerings/by-course/{courseId}` | Course’a göre offering |
| GET | `/api/Admin/course-offerings/{id}` | Tek offering |
| POST | `/api/Admin/course-offerings` | Offering oluştur |
| PUT | `/api/Admin/course-offerings/{id}` | Offering güncelle |
| PUT | `/api/Admin/course-offerings/{id}/assign-teacher` | Öğretmen ata |
| DELETE | `/api/Admin/course-offerings/{id}/remove-teacher` | Öğretmen kaldır |
| DELETE | `/api/Admin/course-offerings/{id}` | Offering sil |

### Kayıt (Enrollment)

| Method | Route | Açıklama |
|---|---|---|
| GET | `/api/Admin/course-offerings/{offeringId}/students` | Enrolled öğrenciler |
| POST | `/api/Admin/course-offerings/{offeringId}/students` | Öğrenci enroll et |
| POST | `/api/Admin/course-offerings/{offeringId}/students/bulk` | Toplu enroll |
| POST | `/api/Admin/course-offerings/{offeringId}/students/import` | Excel ile içe aktar (multipart `file`) |
| PUT | `/api/Admin/course-offerings/{offeringId}/students/{studentId}/status` | Öğrenci status güncelle |
| DELETE | `/api/Admin/course-offerings/{offeringId}/students/{studentId}` | Enrollment sil |

### Ders değerlendirmeleri (read-only)

| Method | Route | Açıklama |
|---|---|---|
| GET | `/api/Admin/course-offerings/{offeringId}/evaluation` | Offering evaluation |
| GET | `/api/Admin/course-evaluations/{id}` | Tek course-evaluation |
| GET | `/api/Admin/course-evaluations` | Tüm course-evaluations |

### Kullanıcı yönetimi

#### Öğretmenler

| Method | Route | Açıklama |
|---|---|---|
| GET | `/api/Admin/teachers?programId={programId}` | Öğretmenler |
| GET | `/api/Admin/teachers/{id}` | Tek öğretmen |
| POST | `/api/Admin/teachers` | Öğretmen oluştur |
| PUT | `/api/Admin/teachers/{id}` | Öğretmen güncelle |
| DELETE | `/api/Admin/teachers/{id}` | Öğretmen sil |

#### Öğrenciler

| Method | Route | Açıklama |
|---|---|---|
| GET | `/api/Admin/students?programId={programId}` | Öğrenciler |
| GET | `/api/Admin/students/{id}` | Tek öğrenci |
| POST | `/api/Admin/students` | Öğrenci oluştur |
| PUT | `/api/Admin/students/{id}` | Öğrenci güncelle |
| DELETE | `/api/Admin/students/{id}` | Öğrenci sil |

## TeacherController (`Policy = "TeacherOnly"`)

Base route: `api/Teacher`

### Derslerim

| Method | Route | Açıklama |
|---|---|---|
| GET | `/api/Teacher/my-courses?termId={termId}` | Öğretmenin dersleri |
| GET | `/api/Teacher/my-courses/{offeringId}` | Offering detayı |
| GET | `/api/Teacher/my-courses/{offeringId}/students` | Dersi alan öğrenciler |

### Değerlendirme (CourseEvaluation)

| Method | Route | Açıklama |
|---|---|---|
| GET | `/api/Teacher/my-courses/{offeringId}/evaluation` | Evaluation (create yoksa 404) |
| POST | `/api/Teacher/my-courses/{offeringId}/evaluation` | Evaluation oluştur |
| PUT | `/api/Teacher/my-courses/{offeringId}/evaluation/{evaluationId}` | Evaluation güncelle |
| DELETE | `/api/Teacher/my-courses/{offeringId}/evaluation/{evaluationId}` | Evaluation sil |

### Sınavlar (Exam)

| Method | Route | Açıklama |
|---|---|---|
| GET | `/api/Teacher/evaluations/{evaluationId}/exams` | Değerlendirmeye ait sınavlar |
| GET | `/api/Teacher/exams/{examId}` | Tek sınav |
| POST | `/api/Teacher/evaluations/{evaluationId}/exams` | Sınav oluştur |
| PUT | `/api/Teacher/exams/{examId}` | Sınav güncelle |
| DELETE | `/api/Teacher/exams/{examId}` | Sınav sil |

### Sınav soruları (ExamQuestion)

| Method | Route | Açıklama |
|---|---|---|
| GET | `/api/Teacher/exams/{examId}/questions` | Sınav soruları |
| GET | `/api/Teacher/exam-questions/{questionId}` | Tek soru |
| POST | `/api/Teacher/exams/{examId}/questions` | Soru oluştur |
| PUT | `/api/Teacher/exam-questions/{questionId}` | Soru güncelle |
| DELETE | `/api/Teacher/exam-questions/{questionId}` | Soru sil |

### Soru -> CLO eşlemesi

| Method | Route | Açıklama |
|---|---|---|
| GET | `/api/Teacher/exam-questions/{questionId}/clos` | Soru CLO eşlemeleri |
| POST | `/api/Teacher/exam-questions/{questionId}/clos` | Soru-CLO eşlemesi ekle |
| PUT | `/api/Teacher/exam-question-outcome-mappings/{mappingId}` | Eşleme güncelle |
| DELETE | `/api/Teacher/exam-question-outcome-mappings/{mappingId}` | Eşleme sil |

### Sınav bileşenleri (AssessmentComponent)

| Method | Route | Açıklama |
|---|---|---|
| GET | `/api/Teacher/exams/{examId}/components` | Sınav bileşenleri |
| GET | `/api/Teacher/assessment-components/{componentId}` | Tek bileşen |
| POST | `/api/Teacher/exams/{examId}/components` | Bileşen oluştur |
| PUT | `/api/Teacher/assessment-components/{componentId}` | Bileşen güncelle |
| DELETE | `/api/Teacher/assessment-components/{componentId}` | Bileşen sil |

### Bileşen -> CLO eşlemesi

| Method | Route | Açıklama |
|---|---|---|
| GET | `/api/Teacher/assessment-components/{componentId}/clos` | Bileşen CLO eşlemeleri |
| POST | `/api/Teacher/assessment-components/{componentId}/clos` | Bileşen-CLO eşlemesi ekle |
| PUT | `/api/Teacher/assessment-component-outcome-mappings/{mappingId}` | Eşleme güncelle |
| DELETE | `/api/Teacher/assessment-component-outcome-mappings/{mappingId}` | Eşleme sil |

### Öğrenci cevapları (StudentAnswers) - soru bazlı

| Method | Route | Açıklama |
|---|---|---|
| GET | `/api/Teacher/exam-questions/{questionId}/answers` | Bir soru için tüm öğrenci cevapları + puan |
| POST | `/api/Teacher/exam-questions/{questionId}/answers` | Tek cevap ekle |
| POST | `/api/Teacher/exam-questions/{questionId}/answers/bulk` | Toplu cevap ekle |
| PUT | `/api/Teacher/student-answers/{answerId}` | Cevap güncelle |
| DELETE | `/api/Teacher/student-answers/{answerId}` | Cevap sil |

### Öğrenci bileşen notları (StudentAssessmentComponentScores)

| Method | Route | Açıklama |
|---|---|---|
| GET | `/api/Teacher/assessment-components/{componentId}/scores` | Bir bileşen için tüm öğrenci puanları |
| POST | `/api/Teacher/assessment-components/{componentId}/scores` | Tek puan ekle |
| POST | `/api/Teacher/assessment-components/{componentId}/scores/bulk` | Toplu puan ekle |
| PUT | `/api/Teacher/student-assessment-component-scores/{scoreId}` | Puan güncelle |
| DELETE | `/api/Teacher/student-assessment-component-scores/{scoreId}` | Puan sil |

### Harf notu kuralları (Letter Grade Rules)

| Method | Route | Açıklama |
|---|---|---|
| GET | `/api/Teacher/evaluations/{evaluationId}/letter-grade-rules` | Evaluation letter rules listesi |
| POST | `/api/Teacher/evaluations/{evaluationId}/letter-grade-rules` | Rule ekle |
| PUT | `/api/Teacher/letter-grade-rules/{ruleId}` | Rule güncelle |
| DELETE | `/api/Teacher/letter-grade-rules/{ruleId}` | Rule sil |

### MÜDEK

| Method | Route | Açıklama |
|---|---|---|
| GET | `/api/Teacher/my-courses/{offeringId}/clos` | Offering dersine ait CLO listesi |
| GET | `/api/Teacher/my-courses/{offeringId}/mudek-evaluation/results` | Hesaplanmış MÜDEK sonuç snapshot |
| POST | `/api/Teacher/my-courses/{offeringId}/mudek-evaluation/calculate` | MÜDEK hesapla (offering bazlı) |
| POST | `/api/Teacher/my-courses/{offeringId}/mudek-evaluation/recalculate` | Aynı: MÜDEK yeniden hesapla (offering bazlı) |
| POST | `/api/Teacher/evaluations/{evaluationId}/mudek-evaluation/calculate` | Evaluation id üzerinden MÜDEK hesapla |

### Değerlendirme full detay

| Method | Route | Açıklama |
|---|---|---|
| GET | `/api/Teacher/evaluations/{evaluationId}/full-detail` | Exams ve components detayı |

