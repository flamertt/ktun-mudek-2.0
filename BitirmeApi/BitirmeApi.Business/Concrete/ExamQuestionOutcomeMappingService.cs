using AutoMapper;
using BitirmeApi.Business.Abstract;
using BitirmeApi.Business.DTO;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.Business.Concrete
{
    public class ExamQuestionOutcomeMappingService : IExamQuestionOutcomeMappingService
    {
        private readonly IExamQuestionOutcomeMappingDal _mappingDal;
        private readonly IExamQuestionDal _questionDal;
        private readonly ICourseLearningOutcomeDal _cloDal;
        private readonly IMapper _mapper;

        public ExamQuestionOutcomeMappingService(
            IExamQuestionOutcomeMappingDal mappingDal,
            IExamQuestionDal questionDal,
            ICourseLearningOutcomeDal cloDal,
            IMapper mapper)
        {
            _mappingDal = mappingDal;
            _questionDal = questionDal;
            _cloDal = cloDal;
            _mapper = mapper;
        }

        public async Task<List<ExamQuestionOutcomeMappingDto>> GetAllAsync()
        {
            var entities = await _mappingDal.GetListAsync();
            return _mapper.Map<List<ExamQuestionOutcomeMappingDto>>(entities.ToList());
        }

        public async Task<ExamQuestionOutcomeMappingDto?> GetByIdAsync(Guid id)
        {
            var entity = await _mappingDal.GetByIdWithDetailsAsync(id);
            return entity != null ? _mapper.Map<ExamQuestionOutcomeMappingDto>(entity) : null;
        }

        public async Task<List<ExamQuestionOutcomeMappingDto>> GetByExamQuestionIdAsync(Guid examQuestionId)
        {
            return _mapper.Map<List<ExamQuestionOutcomeMappingDto>>(
                await _mappingDal.GetByQuestionIdWithDetailsAsync(examQuestionId));
        }

        public async Task<ExamQuestionOutcomeMappingDto> AddAsync(CreateExamQuestionOutcomeMappingDto createDto)
        {
            var entity = _mapper.Map<ExamQuestionOutcomeMapping>(createDto);
            entity.CreatedAt = DateTime.UtcNow;
            var addedEntity = _mappingDal.Add(entity);
            await _mappingDal.SaveChangesAsync();
            return _mapper.Map<ExamQuestionOutcomeMappingDto>((await _mappingDal.GetByIdWithDetailsAsync(addedEntity.Id))!);
        }

        public async Task<ExamQuestionOutcomeMappingDto> UpdateAsync(UpdateExamQuestionOutcomeMappingDto updateDto)
        {
            var existingEntity = await _mappingDal.GetAsync(m => m.Id == updateDto.Id);
            if (existingEntity == null)
                throw new KeyNotFoundException($"Mapping with ID {updateDto.Id} not found");

            _mapper.Map(updateDto, existingEntity);
            _mappingDal.Update(existingEntity);
            await _mappingDal.SaveChangesAsync();
            return _mapper.Map<ExamQuestionOutcomeMappingDto>((await _mappingDal.GetByIdWithDetailsAsync(updateDto.Id))!);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _mappingDal.GetAsync(m => m.Id == id);
            if (entity == null) throw new KeyNotFoundException($"Mapping with ID {id} not found");
            _mappingDal.Delete(entity);
            await _mappingDal.SaveChangesAsync();
        }

        public async Task<List<ExamQuestionOutcomeMappingDto>> GetByQuestionIdForTeacherAsync(Guid questionId, Guid teacherId)
        {
            await VerifyQuestionOwnershipAsync(questionId, teacherId);
            return _mapper.Map<List<ExamQuestionOutcomeMappingDto>>(
                await _mappingDal.GetByQuestionIdWithDetailsAsync(questionId));
        }

        public async Task<ExamQuestionOutcomeMappingDto> AddForTeacherAsync(CreateExamQuestionOutcomeMappingDto createDto, Guid teacherId)
        {
            await VerifyQuestionOwnershipAsync(createDto.ExamQuestionId, teacherId);
            await ValidateCloBelongsToQuestionCourseAsync(createDto.ExamQuestionId, createDto.CourseLearningOutcomeId);
            if (createDto.Weight < 0 || createDto.Weight > 1)
                throw new InvalidOperationException("Weight değeri 0 ile 1 arasında olmalıdır.");
            if (await _mappingDal.ExistsAsync(createDto.ExamQuestionId, createDto.CourseLearningOutcomeId))
                throw new InvalidOperationException("Bu soru için CLO eşlemesi zaten mevcut.");

            return await AddAsync(createDto);
        }

        public async Task<ExamQuestionOutcomeMappingDto> UpdateForTeacherAsync(UpdateExamQuestionOutcomeMappingDto updateDto, Guid teacherId)
        {
            var existing = await _mappingDal.GetByIdWithOwnershipAsync(updateDto.Id)
                ?? throw new KeyNotFoundException("Mapping bulunamadı.");
            if (existing.ExamQuestion?.Exam?.CourseEvaluation?.CourseOffering?.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bu mapping sizin dersinize ait değil.");
            if (updateDto.Weight < 0 || updateDto.Weight > 1)
                throw new InvalidOperationException("Weight değeri 0 ile 1 arasında olmalıdır.");

            return await UpdateAsync(updateDto);
        }

        public async Task DeleteForTeacherAsync(Guid id, Guid teacherId)
        {
            var existing = await _mappingDal.GetByIdWithOwnershipAsync(id)
                ?? throw new KeyNotFoundException("Mapping bulunamadı.");
            if (existing.ExamQuestion?.Exam?.CourseEvaluation?.CourseOffering?.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bu mapping sizin dersinize ait değil.");
            await DeleteAsync(id);
        }

        private async Task VerifyQuestionOwnershipAsync(Guid questionId, Guid teacherId)
        {
            var question = await _questionDal.GetByIdWithOwnershipAsync(questionId)
                ?? throw new KeyNotFoundException("Soru bulunamadı.");
            if (question.Exam?.CourseEvaluation?.CourseOffering?.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bu soru sizin dersinize ait değil.");
        }

        private async Task ValidateCloBelongsToQuestionCourseAsync(Guid questionId, Guid cloId)
        {
            var question = await _questionDal.GetByIdWithOwnershipAsync(questionId)
                ?? throw new KeyNotFoundException("Soru bulunamadı.");
            var clo = await _cloDal.GetAsync(c => c.Id == cloId)
                ?? throw new KeyNotFoundException("CLO bulunamadı.");
            if (question.Exam?.CourseEvaluation?.CourseOffering?.CourseId != clo.CourseId)
                throw new InvalidOperationException("CLO, bu sorunun ait olduğu derse bağlı değil.");
        }
    }
}
