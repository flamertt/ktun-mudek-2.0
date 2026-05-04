using BitirmeApi.Business.Abstract;
using BitirmeApi.Business.DTO;
using BitirmeApi.Business.Integration.Abstract;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.Business.Concrete
{
    public class StudentSurveyService : IStudentSurveyService
    {
        private readonly IUniversityApiService _universityApi;
        private readonly ISurveyDal _surveyDal;
        private readonly ISubmissionDal _submissionDal;
        private readonly IAnswerDal _answerDal;
        private readonly IAcademicTermService _academicTermService;

        public StudentSurveyService(
            IUniversityApiService universityApi,
            ISurveyDal surveyDal,
            ISubmissionDal submissionDal,
            IAnswerDal answerDal,
            IAcademicTermService academicTermService)
        {
            _universityApi = universityApi;
            _surveyDal = surveyDal;
            _submissionDal = submissionDal;
            _answerDal = answerDal;
            _academicTermService = academicTermService;
        }

        public async Task<List<StudentCourseDto>> GetActiveTermCoursesAsync(int externalStudentId, int academicTermId, string universityToken)
        {
            var offerings = await _universityApi.GetStudentOfferingsAsync(externalStudentId, academicTermId, universityToken);
            if (offerings == null || offerings.Count == 0)
                return new List<StudentCourseDto>();

            var result = new List<StudentCourseDto>();
            foreach (var o in offerings)
            {
                // Aktif anket sayısını yerel DB'den al
                var surveys = await _surveyDal.GetActiveByOfferingIdAsync(o.CourseOfferingId);
                result.Add(new StudentCourseDto
                {
                    ExternalCourseOfferingId = o.CourseOfferingId,
                    ExternalCourseId = o.CourseId,
                    CourseCode = o.CourseCode,
                    CourseName = o.CourseName,
                    ExternalProgramId = o.ProgramId,
                    ActiveSurveyCount = surveys.Count
                });
            }
            return result;
        }

        public async Task<List<StudentSurveyListDto>> GetActiveSurveysAsync(int externalCourseOfferingId, int externalStudentId, string universityToken)
        {
            await VerifyEnrollmentAsync(externalCourseOfferingId, externalStudentId, universityToken);

            var surveys = await _surveyDal.GetActiveByOfferingIdAsync(externalCourseOfferingId);

            var result = new List<StudentSurveyListDto>();
            foreach (var survey in surveys)
            {
                var submitted = await _submissionDal.HasStudentSubmittedByExternalIdAsync(survey.Id, externalStudentId);
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

        public async Task<StudentSurveyDetailDto> GetSurveyDetailAsync(Guid surveyId, int externalStudentId, string universityToken)
        {
            var survey = await _surveyDal.GetActiveByIdAsync(surveyId)
                ?? throw new KeyNotFoundException("Anket bulunamadı veya artık aktif değil.");

            await VerifyEnrollmentAsync(survey.ExternalCourseOfferingId, externalStudentId, universityToken);

            var submitted = await _submissionDal.HasStudentSubmittedByExternalIdAsync(surveyId, externalStudentId);

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

        public async Task<StudentSubmissionResultDto> SubmitAsync(Guid surveyId, int externalStudentId, string universityToken, SubmitSurveyDto dto)
        {
            var survey = await _surveyDal.GetActiveByIdAsync(surveyId)
                ?? throw new KeyNotFoundException("Anket bulunamadı veya artık aktif değil.");

            await VerifyEnrollmentAsync(survey.ExternalCourseOfferingId, externalStudentId, universityToken);

            if (await _submissionDal.HasStudentSubmittedByExternalIdAsync(surveyId, externalStudentId))
                throw new InvalidOperationException("Bu ankete daha önce katıldınız. Tekrar gönderim yapılamaz.");

            var validQuestionIds = survey.Questions.Select(q => q.Id).ToHashSet();
            var invalidAnswers = dto.Answers.Where(a => !validQuestionIds.Contains(a.QuestionId)).ToList();
            if (invalidAnswers.Count > 0)
                throw new InvalidOperationException("Gönderilen bazı cevaplar bu ankete ait sorularla eşleşmiyor.");

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

            foreach (var answer in dto.Answers.Where(a => a.ValueNumeric > 0))
            {
                var question = survey.Questions.First(q => q.Id == answer.QuestionId);
                if (answer.ValueNumeric < question.ScaleMin || answer.ValueNumeric > question.ScaleMax)
                    throw new InvalidOperationException(
                        $"Soru cevabı ölçek aralığı dışında: {question.Text} ({question.ScaleMin}-{question.ScaleMax}).");
            }

            var submission = new Submission
            {
                Id = Guid.NewGuid(),
                SurveyId = surveyId,
                ExternalStudentId = externalStudentId,
                SubmittedAt = DateTime.UtcNow,
                IncludeInStatistics = true
            };
            _submissionDal.Add(submission);
            await _submissionDal.SaveChangesAsync();

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

        /// <summary>
        /// Öğrencinin ilgili ders açılışına kayıtlı olup olmadığını üniversite API üzerinden doğrular.
        /// Aktif dönem DB'den okunur; öğrencinin o dönemdeki dersleri arasında offering aranır.
        /// </summary>
        private async Task VerifyEnrollmentAsync(int externalCourseOfferingId, int externalStudentId, string universityToken)
        {
            var activeTerm = await _academicTermService.GetActiveAsync()
                ?? throw new InvalidOperationException("Aktif dönem bulunamadı. Admin panelinden senkronizasyon yapılmalıdır.");

            var offerings = await _universityApi.GetStudentOfferingsAsync(
                externalStudentId, activeTerm.Id, universityToken);

            var enrolled = offerings.Any(o => o.CourseOfferingId == externalCourseOfferingId);
            if (!enrolled)
                throw new UnauthorizedAccessException("Bu derse kayıtlı değilsiniz.");
        }
    }
}
