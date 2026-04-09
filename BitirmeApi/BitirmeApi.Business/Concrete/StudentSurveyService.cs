using BitirmeApi.Business.Abstract;
using BitirmeApi.Business.DTO;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.Business.Concrete
{
    public class StudentSurveyService : IStudentSurveyService
    {
        private readonly IEnrollmentDal _enrollmentDal;
        private readonly ISurveyDal _surveyDal;
        private readonly ISubmissionDal _submissionDal;
        private readonly IAnswerDal _answerDal;

        public StudentSurveyService(
            IEnrollmentDal enrollmentDal,
            ISurveyDal surveyDal,
            ISubmissionDal submissionDal,
            IAnswerDal answerDal)
        {
            _enrollmentDal = enrollmentDal;
            _surveyDal = surveyDal;
            _submissionDal = submissionDal;
            _answerDal = answerDal;
        }

        public async Task<List<StudentCourseDto>> GetActiveTermCoursesAsync(Guid studentId)
        {
            var enrollments = await _enrollmentDal.GetActiveTermEnrollmentsByStudentAsync(studentId);

            return enrollments.Select(e => new StudentCourseDto
            {
                CourseOfferingId = e.CourseOfferingId,
                CourseCode = e.CourseOffering.Course.Code,
                CourseName = e.CourseOffering.Course.Name,
                TeacherName = e.CourseOffering.Teacher?.FullName,
                Section = e.CourseOffering.Section,
                TermName = e.CourseOffering.AcademicTerm.Name,
                ActiveSurveyCount = e.CourseOffering.Surveys.Count(s => s.IsActive)
            }).ToList();
        }

        public async Task<List<StudentSurveyListDto>> GetActiveSurveysAsync(Guid offeringId, Guid studentId)
        {
            await VerifyActiveEnrollmentAsync(offeringId, studentId);

            var surveys = await _surveyDal.GetActiveByOfferingIdAsync(offeringId);

            var result = new List<StudentSurveyListDto>();
            foreach (var survey in surveys)
            {
                var submitted = await _submissionDal.HasStudentSubmittedAsync(survey.Id, studentId);
                result.Add(new StudentSurveyListDto
                {
                    Id = survey.Id,
                    Title = survey.Title,
                    Description = survey.Description,
                    QuestionCount = survey.Questions.Count,
                    HasSubmitted = submitted
                });
            }
            return result;
        }

        public async Task<StudentSurveyDetailDto> GetSurveyDetailAsync(Guid surveyId, Guid studentId)
        {
            var survey = await _surveyDal.GetActiveByIdAsync(surveyId)
                ?? throw new KeyNotFoundException("Anket bulunamadı veya artık aktif değil.");

            await VerifyActiveEnrollmentAsync(survey.CourseOfferingId, studentId);

            var submitted = await _submissionDal.HasStudentSubmittedAsync(surveyId, studentId);

            return new StudentSurveyDetailDto
            {
                Id = survey.Id,
                Title = survey.Title,
                Description = survey.Description,
                HasSubmitted = submitted,
                Questions = survey.Questions
                    .OrderBy(q => q.OrderIndex)
                    .Select(q => new StudentSurveyQuestionDto
                    {
                        Id = q.Id,
                        Text = q.Text,
                        OrderIndex = q.OrderIndex,
                        IsRequired = q.IsRequired,
                        ScaleMin = q.ScaleMin,
                        ScaleMax = q.ScaleMax
                    }).ToList()
            };
        }

        public async Task<StudentSubmissionResultDto> SubmitAsync(Guid surveyId, Guid studentId, SubmitSurveyDto dto)
        {
            var survey = await _surveyDal.GetActiveByIdAsync(surveyId)
                ?? throw new KeyNotFoundException("Anket bulunamadı veya artık aktif değil.");

            await VerifyActiveEnrollmentAsync(survey.CourseOfferingId, studentId);

            if (await _submissionDal.HasStudentSubmittedAsync(surveyId, studentId))
                throw new InvalidOperationException("Bu ankete daha önce katıldınız. Tekrar gönderim yapılamaz.");

            // Cevap edilen soru ID'lerinin ankette var olup olmadığını doğrula
            var validQuestionIds = survey.Questions.Select(q => q.Id).ToHashSet();
            var invalidAnswers = dto.Answers.Where(a => !validQuestionIds.Contains(a.QuestionId)).ToList();
            if (invalidAnswers.Count > 0)
                throw new InvalidOperationException("Gönderilen bazı cevaplar bu ankete ait sorularla eşleşmiyor.");

            // Zorunlu soruların cevaplandığını kontrol et (0 kabul edilmez)
            var requiredIds = survey.Questions
                .Where(q => q.IsRequired)
                .Select(q => q.Id)
                .ToHashSet();
            var answeredNonZero = dto.Answers
                .Where(a => a.ValueNumeric > 0)
                .Select(a => a.QuestionId)
                .ToHashSet();
            var missing = requiredIds.Except(answeredNonZero).ToList();
            if (missing.Count > 0)
                throw new InvalidOperationException($"{missing.Count} zorunlu soru cevaplanmamış.");

            // Ölçek aralığı doğrula
            foreach (var answer in dto.Answers.Where(a => a.ValueNumeric > 0))
            {
                var question = survey.Questions.First(q => q.Id == answer.QuestionId);
                if (answer.ValueNumeric < question.ScaleMin || answer.ValueNumeric > question.ScaleMax)
                    throw new InvalidOperationException(
                        $"Soru cevabı ölçek aralığı dışında: {question.Text} ({question.ScaleMin}-{question.ScaleMax}).");
            }

            // Submission oluştur
            var submission = new Submission
            {
                Id = Guid.NewGuid(),
                SurveyId = surveyId,
                UserId = studentId,
                SubmittedAt = DateTime.UtcNow,
                IncludeInStatistics = true
            };
            _submissionDal.Add(submission);
            await _submissionDal.SaveChangesAsync();

            // Cevapları kaydet
            foreach (var a in dto.Answers)
            {
                _answerDal.Add(new Answer
                {
                    Id = Guid.NewGuid(),
                    SubmissionId = submission.Id,
                    QuestionId = a.QuestionId,
                    ValueNumeric = a.ValueNumeric
                });
            }
            await _answerDal.SaveChangesAsync();

            return new StudentSubmissionResultDto
            {
                SubmissionId = submission.Id,
                SurveyId = surveyId,
                AnsweredQuestions = dto.Answers.Count,
                SubmittedAt = submission.SubmittedAt
            };
        }

        // ── Yardımcı ──────────────────────────────────────────────────────────────

        private async Task VerifyActiveEnrollmentAsync(Guid offeringId, Guid studentId)
        {
            if (!await _enrollmentDal.IsEnrolledInActiveTermAsync(offeringId, studentId))
                throw new UnauthorizedAccessException("Bu derse aktif dönemde kaydınız bulunmuyor.");
        }
    }
}
