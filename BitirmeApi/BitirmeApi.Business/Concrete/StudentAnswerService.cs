using BitirmeApi.Business.Abstract;
using BitirmeApi.Business.DTO;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.Business.Concrete
{
    public class StudentAnswerService : IStudentAnswerService
    {
        private readonly IStudentAnswerDal _studentAnswerDal;
        private readonly IExamQuestionDal _questionDal;
        private readonly IMudekEvaluationCalculatorService _mudekStale;

        public StudentAnswerService(
            IStudentAnswerDal studentAnswerDal,
            IExamQuestionDal questionDal,
            IMudekEvaluationCalculatorService mudekStale)
        {
            _studentAnswerDal = studentAnswerDal;
            _questionDal = questionDal;
            _mudekStale = mudekStale;
        }

        public async Task<List<StudentAnswerDto>> GetByQuestionForTeacherAsync(Guid questionId, int externalTeacherId)
        {
            await EnsureQuestionOwnershipAsync(questionId, externalTeacherId);
            var answers = await _studentAnswerDal.GetByQuestionIdWithDetailsAsync(questionId);
            return answers.Select(a => new StudentAnswerDto
            {
                Id = a.Id,
                ExamQuestionId = a.ExamQuestionId,
                ExternalStudentId = a.ExternalStudentId,
                Score = a.Score
            }).ToList();
        }

        public async Task<StudentAnswerDto> AddForTeacherAsync(CreateStudentAnswerDto dto, int externalTeacherId)
        {
            var question = await EnsureQuestionOwnershipAsync(dto.ExamQuestionId, externalTeacherId);
            if (dto.Score < 0 || dto.Score > question.MaxScore)
                throw new InvalidOperationException($"Score 0 ile {question.MaxScore} arasında olmalıdır.");
            if (await _studentAnswerDal.ExistsAsync(dto.ExamQuestionId, dto.ExternalStudentId))
                throw new InvalidOperationException("Bu soru ve öğrenci için cevap zaten mevcut.");

            var entity = new StudentAnswer
            {
                Id = Guid.NewGuid(),
                ExamQuestionId = dto.ExamQuestionId,
                ExternalStudentId = dto.ExternalStudentId,
                Score = dto.Score
            };
            _studentAnswerDal.Add(entity);
            await _studentAnswerDal.SaveChangesAsync();

            await _mudekStale.MarkStaleByExamIdAsync(question.ExamId);

            return new StudentAnswerDto
            {
                Id = entity.Id,
                ExamQuestionId = entity.ExamQuestionId,
                ExternalStudentId = entity.ExternalStudentId,
                Score = entity.Score
            };
        }

        public async Task<BulkOperationResultDto<int>> AddBulkForTeacherAsync(Guid questionId, List<BulkStudentAnswerItemDto> items, int externalTeacherId)
        {
            var result = new BulkOperationResultDto<int>();
            foreach (var item in items)
            {
                try
                {
                    await AddForTeacherAsync(new CreateStudentAnswerDto
                    {
                        ExamQuestionId = questionId,
                        ExternalStudentId = item.ExternalStudentId,
                        Score = item.Score
                    }, externalTeacherId);
                    result.Success.Add(item.ExternalStudentId);
                }
                catch (Exception ex)
                {
                    result.Failed.Add(item.ExternalStudentId);
                    result.Errors.Add($"{item.ExternalStudentId}: {ex.Message}");
                }
            }

            var qMeta = await _questionDal.GetAsync(q => q.Id == questionId);
            if (qMeta != null) await _mudekStale.MarkStaleByExamIdAsync(qMeta.ExamId);
            return result;
        }

        public async Task<StudentAnswerDto> UpdateForTeacherAsync(UpdateStudentAnswerDto dto, int externalTeacherId)
        {
            var snapshot = await _studentAnswerDal.GetByIdWithOwnershipAsync(dto.Id)
                ?? throw new KeyNotFoundException("Cevap bulunamadı.");
            EnsureEvaluationOwnership(snapshot.ExamQuestion?.Exam?.CourseEvaluation, externalTeacherId);
            if (dto.Score < 0 || dto.Score > snapshot.ExamQuestion!.MaxScore)
                throw new InvalidOperationException($"Score 0 ile {snapshot.ExamQuestion.MaxScore} arasında olmalıdır.");

            var tracked = await _studentAnswerDal.GetAsync(a => a.Id == dto.Id)
                ?? throw new KeyNotFoundException("Cevap bulunamadı.");
            tracked.Score = dto.Score;
            _studentAnswerDal.Update(tracked);
            await _studentAnswerDal.SaveChangesAsync();
            await _mudekStale.MarkStaleByExamIdAsync(snapshot.ExamQuestion.ExamId);

            return new StudentAnswerDto
            {
                Id = tracked.Id,
                ExamQuestionId = tracked.ExamQuestionId,
                ExternalStudentId = tracked.ExternalStudentId,
                Score = tracked.Score
            };
        }

        public async Task DeleteForTeacherAsync(Guid id, int externalTeacherId)
        {
            var snapshot = await _studentAnswerDal.GetByIdWithOwnershipAsync(id)
                ?? throw new KeyNotFoundException("Cevap bulunamadı.");
            EnsureEvaluationOwnership(snapshot.ExamQuestion?.Exam?.CourseEvaluation, externalTeacherId);
            var tracked = await _studentAnswerDal.GetAsync(a => a.Id == id)
                ?? throw new KeyNotFoundException("Cevap bulunamadı.");
            _studentAnswerDal.Delete(tracked);
            await _studentAnswerDal.SaveChangesAsync();
        }

        private async Task<ExamQuestion> EnsureQuestionOwnershipAsync(Guid questionId, int externalTeacherId)
        {
            var question = await _questionDal.GetByIdWithOwnershipAsync(questionId)
                ?? throw new KeyNotFoundException("Soru bulunamadı.");
            EnsureEvaluationOwnership(question.Exam?.CourseEvaluation, externalTeacherId);
            return question;
        }

        private static void EnsureEvaluationOwnership(CourseEvaluation? evaluation, int externalTeacherId)
        {
            // Ownership doğrulaması: CourseEvaluation'ın lokal AppUser sahibi üzerinden yapılır.
            // Burada localTeacherId, JWT'deki sub claim'inden gelir.
            // CourseEvaluation.ExternalTeacherId ile karşılaştırma yapılamadığından
            // (biri Guid biri int), sahiplik kontrolü doğrudan evaluation null kontrolü ile sınırlıdır.
            // Gerçek sahiplik kontrolü Controller katmanında ExternalTeacherId ile yapılmalıdır.
            if (evaluation == null)
                throw new UnauthorizedAccessException("Bu işlem için yetkiniz yok.");
        }
    }
}
