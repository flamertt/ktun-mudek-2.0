using AutoMapper;
using BitirmeApi.Business.Abstract;
using BitirmeApi.Business.DTO;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.Business.Concrete
{
    public class SurveyService : ISurveyService
    {
        private readonly ISurveyDal _surveyDal;
        private readonly IQuestionDal _questionDal;
        private readonly ISubmissionDal _submissionDal;
        private readonly ICourseOfferingDal _offeringDal;
        private readonly IEnrollmentDal _enrollmentDal;
        private readonly ICourseLearningOutcomeDal _cloDal;
        private readonly ICloEvaluationResultDal _cloEvalDal;
        private readonly IStudentEvaluationResultDal _studentEvalDal;
        private readonly IMapper _mapper;

        public SurveyService(
            ISurveyDal surveyDal,
            IQuestionDal questionDal,
            ISubmissionDal submissionDal,
            ICourseOfferingDal offeringDal,
            IEnrollmentDal enrollmentDal,
            ICourseLearningOutcomeDal cloDal,
            ICloEvaluationResultDal cloEvalDal,
            IStudentEvaluationResultDal studentEvalDal,
            IMapper mapper)
        {
            _surveyDal = surveyDal;
            _questionDal = questionDal;
            _submissionDal = submissionDal;
            _offeringDal = offeringDal;
            _enrollmentDal = enrollmentDal;
            _cloDal = cloDal;
            _cloEvalDal = cloEvalDal;
            _studentEvalDal = studentEvalDal;
            _mapper = mapper;
        }

        // ── Anket CRUD ─────────────────────────────────────────────────────────────

        public async Task<List<SurveyListDto>> GetByOfferingIdAsync(Guid offeringId, Guid teacherId)
        {
            await VerifyOfferingOwnershipAsync(offeringId, teacherId);
            return _mapper.Map<List<SurveyListDto>>(await _surveyDal.GetByOfferingIdAsync(offeringId));
        }

        public async Task<SurveyDetailDto?> GetByIdAsync(Guid id, Guid teacherId)
        {
            var survey = await _surveyDal.GetByIdWithDetailsAsync(id);
            if (survey == null) return null;
            VerifySurveyOwnership(survey, teacherId);
            return _mapper.Map<SurveyDetailDto>(survey);
        }

        public async Task<SurveyDetailDto> CreateAsync(CreateSurveyDto dto, Guid teacherId)
        {
            await VerifyOfferingOwnershipAsync(dto.CourseOfferingId, teacherId);

            var entity = new Survey
            {
                Id = Guid.NewGuid(),
                CourseOfferingId = dto.CourseOfferingId,
                Title = dto.Title,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _surveyDal.Add(entity);
            await _surveyDal.SaveChangesAsync();

            return _mapper.Map<SurveyDetailDto>((await _surveyDal.GetByIdWithDetailsAsync(entity.Id))!);
        }

        public async Task<SurveyDetailDto> UpdateAsync(UpdateSurveyDto dto, Guid teacherId)
        {
            var snapshot = await _surveyDal.GetByIdWithDetailsAsync(dto.Id)
                ?? throw new KeyNotFoundException("Anket bulunamadı.");
            VerifySurveyOwnership(snapshot, teacherId);

            var tracked = await _surveyDal.GetAsync(s => s.Id == dto.Id)
                ?? throw new KeyNotFoundException("Anket bulunamadı.");

            tracked.Title = dto.Title;
            tracked.Description = dto.Description;
            tracked.IsActive = dto.IsActive;

            _surveyDal.Update(tracked);
            await _surveyDal.SaveChangesAsync();

            return _mapper.Map<SurveyDetailDto>((await _surveyDal.GetByIdWithDetailsAsync(dto.Id))!);
        }

        public async Task DeleteAsync(Guid id, Guid teacherId)
        {
            var snapshot = await _surveyDal.GetByIdWithDetailsAsync(id)
                ?? throw new KeyNotFoundException("Anket bulunamadı.");
            VerifySurveyOwnership(snapshot, teacherId);

            if (snapshot.Submissions.Any())
                throw new InvalidOperationException("Ankete ait gönderimler bulunduğu için silinemez.");

            var tracked = await _surveyDal.GetAsync(s => s.Id == id)
                ?? throw new KeyNotFoundException("Anket bulunamadı.");

            _surveyDal.Delete(tracked);
            await _surveyDal.SaveChangesAsync();
        }

        public async Task ToggleActiveAsync(Guid id, Guid teacherId)
        {
            var snapshot = await _surveyDal.GetByIdWithDetailsAsync(id)
                ?? throw new KeyNotFoundException("Anket bulunamadı.");
            VerifySurveyOwnership(snapshot, teacherId);

            var tracked = await _surveyDal.GetAsync(s => s.Id == id)
                ?? throw new KeyNotFoundException("Anket bulunamadı.");

            tracked.IsActive = !tracked.IsActive;
            _surveyDal.Update(tracked);
            await _surveyDal.SaveChangesAsync();
        }

        // ── Soru (Likert) CRUD ─────────────────────────────────────────────────────

        public async Task<SurveyQuestionDto> AddQuestionAsync(CreateSurveyQuestionDto dto, Guid teacherId)
        {
            var survey = await _surveyDal.GetByIdWithDetailsAsync(dto.SurveyId)
                ?? throw new KeyNotFoundException("Anket bulunamadı.");
            VerifySurveyOwnership(survey, teacherId);
            ValidateScale(dto.ScaleMin, dto.ScaleMax);

            if (dto.CourseLearningOutcomeId.HasValue)
                await ValidateCloForOfferingAsync(dto.CourseLearningOutcomeId.Value, survey.CourseOffering.CourseId);

            var entity = new Question
            {
                Id = Guid.NewGuid(),
                SurveyId = dto.SurveyId,
                Text = dto.Text,
                Type = QuestionType.Likert,
                OrderIndex = dto.OrderIndex,
                IsRequired = dto.IsRequired,
                ScaleMin = dto.ScaleMin,
                ScaleMax = dto.ScaleMax,
                CourseLearningOutcomeId = dto.CourseLearningOutcomeId
            };

            _questionDal.Add(entity);
            await _questionDal.SaveChangesAsync();

            // CLO bilgisini de getir
            var saved = await _questionDal.GetByIdWithSurveyAsync(entity.Id)!;
            return _mapper.Map<SurveyQuestionDto>(saved);
        }

        public async Task<SurveyQuestionDto> UpdateQuestionAsync(UpdateSurveyQuestionDto dto, Guid teacherId)
        {
            var question = await _questionDal.GetByIdWithSurveyAsync(dto.Id)
                ?? throw new KeyNotFoundException("Soru bulunamadı.");
            VerifySurveyOwnership(question.Survey, teacherId);
            ValidateScale(dto.ScaleMin, dto.ScaleMax);

            if (dto.CourseLearningOutcomeId.HasValue)
                await ValidateCloForOfferingAsync(dto.CourseLearningOutcomeId.Value, question.Survey.CourseOffering.CourseId);

            var tracked = await _questionDal.GetAsync(q => q.Id == dto.Id)
                ?? throw new KeyNotFoundException("Soru bulunamadı.");

            tracked.Text = dto.Text;
            tracked.OrderIndex = dto.OrderIndex;
            tracked.IsRequired = dto.IsRequired;
            tracked.ScaleMin = dto.ScaleMin;
            tracked.ScaleMax = dto.ScaleMax;
            tracked.CourseLearningOutcomeId = dto.CourseLearningOutcomeId;

            _questionDal.Update(tracked);
            await _questionDal.SaveChangesAsync();

            var saved = await _questionDal.GetByIdWithSurveyAsync(tracked.Id)!;
            return _mapper.Map<SurveyQuestionDto>(saved);
        }

        public async Task DeleteQuestionAsync(Guid questionId, Guid teacherId)
        {
            var question = await _questionDal.GetByIdWithSurveyAsync(questionId)
                ?? throw new KeyNotFoundException("Soru bulunamadı.");
            VerifySurveyOwnership(question.Survey, teacherId);

            var tracked = await _questionDal.GetAsync(q => q.Id == questionId)
                ?? throw new KeyNotFoundException("Soru bulunamadı.");

            _questionDal.Delete(tracked);
            await _questionDal.SaveChangesAsync();
        }

        // ── Sonuçlar ───────────────────────────────────────────────────────────────

        public async Task<SurveyResultsDto> GetResultsAsync(Guid surveyId, Guid teacherId)
        {
            var survey = await _surveyDal.GetByIdWithDetailsAsync(surveyId)
                ?? throw new KeyNotFoundException("Anket bulunamadı.");
            VerifySurveyOwnership(survey, teacherId);

            // 1. Katılım istatistikleri
            var enrolledCount = await _enrollmentDal.GetCountByOfferingAsync(survey.CourseOfferingId);

            // 2. Geçen öğrencileri belirle (MÜDEK hesabı yapılmamışsa boş → tümünü kullan)
            var passingIds = await _studentEvalDal.GetPassingStudentIdsAsync(survey.CourseOfferingId);
            bool filterApplied = passingIds.Count > 0;

            // 3. Gönderimler
            var allSubmissions = await _submissionDal.GetBySurveyIdWithAnswersAsync(surveyId);
            var submissions = filterApplied
                ? allSubmissions.Where(s => passingIds.Contains(s.UserId)).ToList()
                : allSubmissions;

            var questions = survey.Questions.OrderBy(q => q.OrderIndex).ToList();

            // 3. Soru bazlı hesaplama
            var questionResults = questions.Select(q =>
            {
                // 0 hariç yanıtlar
                var nonZeroAnswers = submissions
                    .SelectMany(s => s.Answers)
                    .Where(a => a.QuestionId == q.Id
                             && a.ValueNumeric.HasValue
                             && a.ValueNumeric.Value > 0)
                    .ToList();

                // Dağılım (0 dahil)
                var distribution = new Dictionary<int, int>();
                for (int i = q.ScaleMin; i <= q.ScaleMax; i++)
                    distribution[i] = 0;

                var allAnswers = submissions
                    .SelectMany(s => s.Answers)
                    .Where(a => a.QuestionId == q.Id && a.ValueNumeric.HasValue);

                foreach (var a in allAnswers)
                {
                    var val = (int)Math.Round(a.ValueNumeric!.Value);
                    if (distribution.ContainsKey(val))
                        distribution[val]++;
                }

                decimal? avg = nonZeroAnswers.Count > 0
                    ? Math.Round((decimal)nonZeroAnswers.Average(a => (double)a.ValueNumeric!.Value), 2)
                    : null;

                decimal? pct = avg.HasValue
                    ? Math.Round(avg.Value / q.ScaleMax * 100m, 2)
                    : null;

                return new SurveyQuestionResultDto
                {
                    QuestionId = q.Id,
                    Text = q.Text,
                    OrderIndex = q.OrderIndex,
                    CourseLearningOutcomeId = q.CourseLearningOutcomeId,
                    CloCode = q.CourseLearningOutcome?.Code,
                    ResponseCount = nonZeroAnswers.Count,
                    AverageScore = avg,
                    ScorePercentage = pct,
                    ScoreDistribution = distribution
                };
            }).ToList();

            // 4. DÖÇ bazlı hesaplama
            var cloResults = new List<CloSurveyResultDto>();

            var cloGroups = questions
                .Where(q => q.CourseLearningOutcomeId.HasValue && q.CourseLearningOutcome != null)
                .GroupBy(q => q.CourseLearningOutcomeId!.Value)
                .ToList();

            // MÜDEK Combined skorları (tracked — güncelleyeceğiz)
            var mudekRows = await _cloEvalDal.GetCombinedByOfferingAsync(survey.CourseOfferingId);

            foreach (var group in cloGroups)
            {
                var cloId = group.Key;
                var clo = group.First().CourseLearningOutcome!;

                var questionPcts = group
                    .Select(q => questionResults.FirstOrDefault(r => r.QuestionId == q.Id)?.ScorePercentage)
                    .Where(p => p.HasValue)
                    .Select(p => p!.Value)
                    .ToList();

                decimal? surveyScore = questionPcts.Count > 0
                    ? Math.Round(questionPcts.Average(), 2)
                    : null;

                // MÜDEK skoru — AchievementScore 0-1 değeri, 100 ile çarp
                var mudekRow = mudekRows.FirstOrDefault(m => m.CourseLearningOutcomeId == cloId);
                decimal? mudekScore = mudekRow?.AchievementScore.HasValue == true
                    ? Math.Round(mudekRow.AchievementScore!.Value * 100m, 2)
                    : null;

                decimal? diff = (surveyScore.HasValue && mudekScore.HasValue)
                    ? Math.Round(surveyScore.Value - mudekScore.Value, 2)
                    : null;

                string? evaluation = diff.HasValue
                    ? diff.Value switch
                    {
                        var d when Math.Abs(d) <= 10m => "Uyumlu",
                        var d when d > 10m => "Anket Yüksek",
                        _ => "Anket Düşük"
                    }
                    : null;

                cloResults.Add(new CloSurveyResultDto
                {
                    CloId = cloId,
                    CloCode = clo.Code,
                    CloDescription = clo.Description,
                    QuestionCount = group.Count(),
                    SurveyScore = surveyScore,
                    MudekScore = mudekScore,
                    Difference = diff,
                    Evaluation = evaluation
                });

                // 5. MÜDEK CloEvaluationResult tablosunu güncelle
                if (mudekRow != null && surveyScore.HasValue)
                {
                    mudekRow.SurveyScore = surveyScore.Value / 100m; // 0-1 aralığında sakla
                    mudekRow.SurveyDifference = diff.HasValue
                        ? diff.Value / 100m
                        : null;
                    _cloEvalDal.Update(mudekRow);
                }
            }

            if (cloGroups.Count > 0)
                await _cloEvalDal.SaveChangesAsync();

            return new SurveyResultsDto
            {
                SurveyId = surveyId,
                Title = survey.Title,
                EnrolledStudentCount = enrolledCount,
                TotalSubmissions = allSubmissions.Count,
                EvaluatedSubmissions = submissions.Count,
                NotParticipatedCount = enrolledCount - allSubmissions.Count,
                SubmittedButExcludedCount = allSubmissions.Count - submissions.Count,
                IsPassingFilterApplied = filterApplied,
                Questions = questionResults,
                CloResults = cloResults
            };
        }

        // ── Yardımcı metotlar ──────────────────────────────────────────────────────

        private async Task VerifyOfferingOwnershipAsync(Guid offeringId, Guid teacherId)
        {
            var offering = await _offeringDal.GetAsync(o => o.Id == offeringId)
                ?? throw new KeyNotFoundException("Ders açılışı bulunamadı.");
            if (offering.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bu ders açılışı size ait değil.");
        }

        private static void VerifySurveyOwnership(Survey survey, Guid teacherId)
        {
            if (survey.CourseOffering?.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bu anket size ait değil.");
        }

        private static void ValidateScale(int min, int max)
        {
            if (min < 0 || max > 10 || min >= max)
                throw new InvalidOperationException("Geçersiz Likert ölçeği. min ≥ 0, max ≤ 10 ve min < max olmalıdır.");
        }

        private async Task ValidateCloForOfferingAsync(Guid cloId, Guid courseId)
        {
            var clo = await _cloDal.GetAsync(c => c.Id == cloId)
                ?? throw new KeyNotFoundException($"DÖÇ bulunamadı: {cloId}");
            if (clo.CourseId != courseId)
                throw new InvalidOperationException("Seçilen DÖÇ bu dersin kursu ile eşleşmiyor.");
        }
    }
}
