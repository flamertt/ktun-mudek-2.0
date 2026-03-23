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
        private readonly ICourseLearningOutcomeDal _cloDal;
        private readonly IExamQuestionOutcomeMappingDal _mappingDal;
        private readonly IStudentAnswerDal _answerDal;
        private readonly IMapper _mapper;

        public ExamQuestionService(
            IExamQuestionDal questionDal,
            IExamDal examDal,
            ICourseLearningOutcomeDal cloDal,
            IExamQuestionOutcomeMappingDal mappingDal,
            IStudentAnswerDal answerDal,
            IMapper mapper)
        {
            _questionDal = questionDal;
            _examDal = examDal;
            _cloDal = cloDal;
            _mappingDal = mappingDal;
            _answerDal = answerDal;
            _mapper = mapper;
        }

        public async Task<List<ExamQuestionDto>> GetByExamIdAsync(Guid examId) =>
            _mapper.Map<List<ExamQuestionDto>>(await _questionDal.GetByExamIdWithDetailsAsync(examId));

        public async Task<List<ExamQuestionDto>> GetByExamIdForTeacherAsync(Guid examId, Guid teacherId)
        {
            var exam = await _examDal.GetByIdWithOwnershipAsync(examId)
                ?? throw new KeyNotFoundException("Sınav bulunamadı.");
            if (exam.CourseEvaluation?.CourseOffering?.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bu sınav size ait değil.");
            return await GetByExamIdAsync(examId);
        }

        public async Task<ExamQuestionDto?> GetByIdAsync(Guid id)
        {
            var entity = await _questionDal.GetByIdWithDetailsAsync(id);
            return entity != null ? _mapper.Map<ExamQuestionDto>(entity) : null;
        }

        public async Task<ExamQuestionDto?> GetByIdForTeacherAsync(Guid id, Guid teacherId)
        {
            await VerifyOwnershipAsync(id, teacherId);
            return await GetByIdAsync(id);
        }

        public async Task<ExamQuestionDto> CreateAsync(CreateExamQuestionDto dto, Guid teacherId)
        {
            // Sınav var mı ve bu öğretmene mi ait?
            var exam = await _examDal.GetByIdWithOwnershipAsync(dto.ExamId)
                ?? throw new KeyNotFoundException("Belirtilen sınav bulunamadı.");

            if (exam.CourseEvaluation?.CourseOffering?.TeacherId != teacherId)
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

            return _mapper.Map<ExamQuestionDto>((await _questionDal.GetByIdWithDetailsAsync(entity.Id))!);
        }

        public async Task<ExamQuestionDto> UpdateAsync(UpdateExamQuestionDto dto, Guid teacherId)
        {
            await VerifyOwnershipAsync(dto.Id, teacherId);

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

            return _mapper.Map<ExamQuestionDto>((await _questionDal.GetByIdWithDetailsAsync(dto.Id))!);
        }

        public async Task DeleteAsync(Guid id, Guid teacherId)
        {
            await VerifyOwnershipAsync(id, teacherId);
            if ((await _answerDal.GetListAsync(a => a.ExamQuestionId == id)).Any() ||
                (await _mappingDal.GetListAsync(m => m.ExamQuestionId == id)).Any())
                throw new InvalidOperationException("Soruya bağlı answer/mapping olduğu için silinemez.");

            var tracked = await _questionDal.GetAsync(q => q.Id == id)
                ?? throw new KeyNotFoundException("Sınav sorusu bulunamadı.");

            _questionDal.Delete(tracked);
            await _questionDal.SaveChangesAsync();
        }

        // ── CLO Eşleme ────────────────────────────────────────────────────────

        public async Task<ExamQuestionOutcomeMappingDto> MapToOutcomeAsync(
            Guid questionId, Guid cloId, decimal weight, Guid teacherId)
        {
            await VerifyOwnershipAsync(questionId, teacherId);

            // CLO var mı?
            if (!await _cloDal.ExistsAsync(cloId))
                throw new KeyNotFoundException("Belirtilen ders öğrenim çıktısı (CLO) bulunamadı.");

            // CLO bu dersin CLO'su mu? (Ownership zinciri üzerinden CourseId kontrolü)
            var question = await _questionDal.GetByIdWithOwnershipAsync(questionId)
                ?? throw new KeyNotFoundException("Sınav sorusu bulunamadı.");

            var courseId = question.Exam?.CourseEvaluation?.CourseOffering?.CourseId;
            var clo = await _cloDal.GetAsync(c => c.Id == cloId)
                ?? throw new KeyNotFoundException("CLO bulunamadı.");

            if (clo.CourseId != courseId)
                throw new InvalidOperationException(
                    "Bu CLO, sorunun bağlı olduğu derse ait değil.");

            // Zaten eşlenmiş mi?
            if (await _mappingDal.ExistsAsync(questionId, cloId))
                throw new InvalidOperationException("Bu soru ile CLO zaten eşleştirilmiş.");

            if (weight <= 0 || weight > 1)
                throw new InvalidOperationException("Ağırlık değeri 0'dan büyük ve 1'den küçük ya da eşit olmalıdır.");

            var entity = new ExamQuestionOutcomeMapping
            {
                Id = Guid.NewGuid(),
                ExamQuestionId = questionId,
                CourseLearningOutcomeId = cloId,
                Weight = weight,
                CreatedAt = DateTime.UtcNow
            };

            _mappingDal.Add(entity);
            await _mappingDal.SaveChangesAsync();

            var mappings = await _mappingDal.GetByQuestionIdWithDetailsAsync(questionId);
            return _mapper.Map<ExamQuestionOutcomeMappingDto>(mappings.First(m => m.CourseLearningOutcomeId == cloId));
        }

        public async Task<ExamQuestionOutcomeMappingDto> UpdateMappingWeightAsync(
            Guid questionId, Guid cloId, decimal weight, Guid teacherId)
        {
            await VerifyOwnershipAsync(questionId, teacherId);

            if (weight <= 0 || weight > 1)
                throw new InvalidOperationException("Ağırlık değeri 0'dan büyük ve 1'den küçük ya da eşit olmalıdır.");

            var mapping = await _mappingDal.GetByIdsAsync(questionId, cloId)
                ?? throw new KeyNotFoundException("Belirtilen soru–CLO eşlemesi bulunamadı.");

            mapping.Weight = weight;
            _mappingDal.Update(mapping);
            await _mappingDal.SaveChangesAsync();

            var mappings = await _mappingDal.GetByQuestionIdWithDetailsAsync(questionId);
            return _mapper.Map<ExamQuestionOutcomeMappingDto>(mappings.First(m => m.CourseLearningOutcomeId == cloId));
        }

        public async Task UnmapOutcomeAsync(Guid questionId, Guid cloId, Guid teacherId)
        {
            await VerifyOwnershipAsync(questionId, teacherId);

            var mapping = await _mappingDal.GetByIdsAsync(questionId, cloId)
                ?? throw new KeyNotFoundException("Belirtilen soru–CLO eşlemesi bulunamadı.");

            _mappingDal.Delete(mapping);
            await _mappingDal.SaveChangesAsync();
        }

        // ── Ownership yardımcısı ──────────────────────────────────────────────

        /// <summary>
        /// Soru → Sınav → Değerlendirme → CourseOffering → TeacherId zincirini doğrular.
        /// </summary>
        private async Task VerifyOwnershipAsync(Guid questionId, Guid teacherId)
        {
            var question = await _questionDal.GetByIdWithOwnershipAsync(questionId)
                ?? throw new KeyNotFoundException("Sınav sorusu bulunamadı.");

            if (question.Exam?.CourseEvaluation?.CourseOffering?.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bu soru sizin dersinize ait değil.");
        }
    }
}
