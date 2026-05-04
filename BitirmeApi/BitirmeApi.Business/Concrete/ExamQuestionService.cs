using AutoMapper;
using BitirmeApi.Business.Abstract;
using BitirmeApi.Business.DTO;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.Business.Concrete
{
    public class ExamQuestionService : IExamQuestionService
    {
        private readonly IExamQuestionDal _questionDal;
        private readonly IExamDal _examDal;
        private readonly IExamQuestionOutcomeMappingDal _mappingDal;
        private readonly IStudentAnswerDal _answerDal;
        private readonly IMapper _mapper;
        private readonly IMudekEvaluationCalculatorService _mudekStale;

        public ExamQuestionService(
            IExamQuestionDal questionDal,
            IExamDal examDal,
            IExamQuestionOutcomeMappingDal mappingDal,
            IStudentAnswerDal answerDal,
            IMapper mapper,
            IMudekEvaluationCalculatorService mudekStale)
        {
            _questionDal = questionDal;
            _examDal = examDal;
            _mappingDal = mappingDal;
            _answerDal = answerDal;
            _mapper = mapper;
            _mudekStale = mudekStale;
        }

        public async Task<List<ExamQuestionDto>> GetByExamIdAsync(Guid examId) =>
            _mapper.Map<List<ExamQuestionDto>>(await _questionDal.GetByExamIdWithDetailsAsync(examId));

        public async Task<List<ExamQuestionDto>> GetByExamIdForTeacherAsync(Guid examId, int externalTeacherId)
        {
            var exam = await _examDal.GetByIdWithOwnershipAsync(examId)
                ?? throw new KeyNotFoundException("Sınav bulunamadı.");
            if (exam.CourseEvaluation?.ExternalTeacherId != externalTeacherId)
                throw new UnauthorizedAccessException("Bu sınav size ait değil.");
            return await GetByExamIdAsync(examId);
        }

        public async Task<ExamQuestionDto?> GetByIdAsync(Guid id)
        {
            var entity = await _questionDal.GetByIdWithDetailsAsync(id);
            return entity != null ? _mapper.Map<ExamQuestionDto>(entity) : null;
        }

        public async Task<ExamQuestionDto?> GetByIdForTeacherAsync(Guid id, int externalTeacherId)
        {
            await VerifyOwnershipAsync(id, externalTeacherId);
            return await GetByIdAsync(id);
        }

        public async Task<ExamQuestionDto> CreateAsync(CreateExamQuestionDto dto, int externalTeacherId)
        {
            var exam = await _examDal.GetByIdWithOwnershipAsync(dto.ExamId)
                ?? throw new KeyNotFoundException("Belirtilen sınav bulunamadı.");
            if (exam.CourseEvaluation?.ExternalTeacherId != externalTeacherId)
                throw new UnauthorizedAccessException("Bu sınav sizin dersinize ait değil.");
            if (dto.MaxScore <= 0)
                throw new InvalidOperationException("MaxScore 0'dan büyük olmalıdır.");
            if ((await _questionDal.GetListAsync(q => q.ExamId == dto.ExamId && q.QuestionNumber == dto.QuestionNumber)).Any())
                throw new InvalidOperationException("Aynı sınavda QuestionNumber benzersiz olmalıdır.");

            var entity = new ExamQuestion
            {
                Id = Guid.NewGuid(),
                ExamId = dto.ExamId,
                QuestionNumber = dto.QuestionNumber,
                MaxScore = dto.MaxScore,
                Title = dto.Title,
                Description = dto.Description,
                QuestionType = dto.QuestionType
            };

            _questionDal.Add(entity);
            await _questionDal.SaveChangesAsync();
            await _mudekStale.MarkStaleByExamIdAsync(dto.ExamId);

            return _mapper.Map<ExamQuestionDto>((await _questionDal.GetByIdWithDetailsAsync(entity.Id))!);
        }

        public async Task<ExamQuestionDto> UpdateAsync(UpdateExamQuestionDto dto, int externalTeacherId)
        {
            await VerifyOwnershipAsync(dto.Id, externalTeacherId);

            var tracked = await _questionDal.GetAsync(q => q.Id == dto.Id)
                ?? throw new KeyNotFoundException("Sınav sorusu bulunamadı.");
            if (dto.MaxScore <= 0)
                throw new InvalidOperationException("MaxScore 0'dan büyük olmalıdır.");
            if ((await _questionDal.GetListAsync(q =>
                q.ExamId == tracked.ExamId &&
                q.QuestionNumber == dto.QuestionNumber &&
                q.Id != dto.Id)).Any())
                throw new InvalidOperationException("Aynı sınavda QuestionNumber benzersiz olmalıdır.");

            tracked.QuestionNumber = dto.QuestionNumber;
            tracked.MaxScore = dto.MaxScore;
            tracked.Title = dto.Title;
            tracked.Description = dto.Description;
            tracked.QuestionType = dto.QuestionType;
            tracked.UpdatedAt = DateTime.UtcNow;

            _questionDal.Update(tracked);
            await _questionDal.SaveChangesAsync();
            await _mudekStale.MarkStaleByExamIdAsync(tracked.ExamId);

            return _mapper.Map<ExamQuestionDto>((await _questionDal.GetByIdWithDetailsAsync(dto.Id))!);
        }

        public async Task DeleteAsync(Guid id, int externalTeacherId)
        {
            await VerifyOwnershipAsync(id, externalTeacherId);
            if ((await _answerDal.GetListAsync(a => a.ExamQuestionId == id)).Any() ||
                (await _mappingDal.GetListAsync(m => m.ExamQuestionId == id)).Any())
                throw new InvalidOperationException("Soruya bağlı answer/mapping olduğu için silinemez.");

            var tracked = await _questionDal.GetAsync(q => q.Id == id)
                ?? throw new KeyNotFoundException("Sınav sorusu bulunamadı.");
            var examId = tracked.ExamId;
            _questionDal.Delete(tracked);
            await _questionDal.SaveChangesAsync();
            await _mudekStale.MarkStaleByExamIdAsync(examId);
        }

        // CLO mapping stubs — ExamQuestionOutcomeMappingService kullanılmalı
        public Task<ExamQuestionOutcomeMappingDto> MapToOutcomeAsync(Guid questionId, Guid cloId, decimal weight, int externalTeacherId) =>
            throw new NotSupportedException("ExamQuestionOutcomeMappingService.AddForTeacherAsync kullanın.");
        public Task<ExamQuestionOutcomeMappingDto> UpdateMappingWeightAsync(Guid questionId, Guid cloId, decimal weight, int externalTeacherId) =>
            throw new NotSupportedException("ExamQuestionOutcomeMappingService.UpdateForTeacherAsync kullanın.");
        public Task UnmapOutcomeAsync(Guid questionId, Guid cloId, int externalTeacherId) =>
            throw new NotSupportedException("ExamQuestionOutcomeMappingService.DeleteForTeacherAsync kullanın.");

        private async Task VerifyOwnershipAsync(Guid questionId, int externalTeacherId)
        {
            var question = await _questionDal.GetByIdWithOwnershipAsync(questionId)
                ?? throw new KeyNotFoundException("Sınav sorusu bulunamadı.");
            if (question.Exam?.CourseEvaluation?.ExternalTeacherId != externalTeacherId)
                throw new UnauthorizedAccessException("Bu soru sizin dersinize ait değil.");
        }
    }
}
