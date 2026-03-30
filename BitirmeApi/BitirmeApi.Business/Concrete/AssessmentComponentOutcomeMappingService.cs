using AutoMapper;
using BitirmeApi.Business.Abstract;
using BitirmeApi.Business.DTO;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.Business.Concrete
{
    public class AssessmentComponentOutcomeMappingService : IAssessmentComponentOutcomeMappingService
    {
        private readonly IAssessmentComponentOutcomeMappingDal _mappingDal;
        private readonly IAssessmentComponentDal _componentDal;
        private readonly ICourseLearningOutcomeDal _cloDal;
        private readonly IMapper _mapper;
        private readonly IMudekEvaluationCalculatorService _mudekStale;

        public AssessmentComponentOutcomeMappingService(
            IAssessmentComponentOutcomeMappingDal mappingDal,
            IAssessmentComponentDal componentDal,
            ICourseLearningOutcomeDal cloDal,
            IMapper mapper,
            IMudekEvaluationCalculatorService mudekStale)
        {
            _mappingDal = mappingDal;
            _componentDal = componentDal;
            _cloDal = cloDal;
            _mapper = mapper;
            _mudekStale = mudekStale;
        }

        public async Task<List<AssessmentComponentOutcomeMappingDto>> GetAllAsync()
        {
            var entities = await _mappingDal.GetListAsync();
            return _mapper.Map<List<AssessmentComponentOutcomeMappingDto>>(entities.ToList());
        }

        public async Task<AssessmentComponentOutcomeMappingDto?> GetByIdAsync(Guid id)
        {
            var entity = await _mappingDal.GetByIdWithOwnershipAsync(id);
            return entity != null ? _mapper.Map<AssessmentComponentOutcomeMappingDto>(entity) : null;
        }

        public async Task<List<AssessmentComponentOutcomeMappingDto>> GetByAssessmentComponentIdAsync(Guid assessmentComponentId)
        {
            return _mapper.Map<List<AssessmentComponentOutcomeMappingDto>>(
                await _mappingDal.GetByComponentIdWithDetailsAsync(assessmentComponentId));
        }

        public async Task<AssessmentComponentOutcomeMappingDto> AddAsync(CreateAssessmentComponentOutcomeMappingDto createDto)
        {
            var entity = _mapper.Map<AssessmentComponentOutcomeMapping>(createDto);
            entity.CreatedAt = DateTime.UtcNow;
            var addedEntity = _mappingDal.Add(entity);
            await _mappingDal.SaveChangesAsync();
            return _mapper.Map<AssessmentComponentOutcomeMappingDto>(addedEntity);
        }

        public async Task<AssessmentComponentOutcomeMappingDto> UpdateAsync(UpdateAssessmentComponentOutcomeMappingDto updateDto)
        {
            var existingEntity = await _mappingDal.GetAsync(m => m.Id == updateDto.Id);
            if (existingEntity == null)
                throw new KeyNotFoundException($"Mapping with ID {updateDto.Id} not found");

            _mapper.Map(updateDto, existingEntity);
            _mappingDal.Update(existingEntity);
            await _mappingDal.SaveChangesAsync();
            return _mapper.Map<AssessmentComponentOutcomeMappingDto>(existingEntity);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _mappingDal.GetAsync(m => m.Id == id);
            if (entity == null) throw new KeyNotFoundException($"Mapping with ID {id} not found");
            _mappingDal.Delete(entity);
            await _mappingDal.SaveChangesAsync();
        }

        public async Task<List<AssessmentComponentOutcomeMappingDto>> GetByComponentIdForTeacherAsync(Guid componentId, Guid teacherId)
        {
            await EnsureComponentOwnershipAsync(componentId, teacherId);
            return await GetByAssessmentComponentIdAsync(componentId);
        }

        public async Task<AssessmentComponentOutcomeMappingDto> AddForTeacherAsync(CreateAssessmentComponentOutcomeMappingDto createDto, Guid teacherId)
        {
            var component = await EnsureComponentOwnershipAsync(createDto.AssessmentComponentId, teacherId);
            var clo = await _cloDal.GetAsync(c => c.Id == createDto.CourseLearningOutcomeId)
                ?? throw new KeyNotFoundException("CLO bulunamadı.");
            if (component.Exam?.CourseEvaluation?.CourseOffering?.CourseId != clo.CourseId)
                throw new InvalidOperationException("CLO bu dersin course kaydına ait değil.");
            if (createDto.Weight < 0 || createDto.Weight > 1)
                throw new InvalidOperationException("Weight değeri 0 ile 1 arasında olmalıdır.");
            if (await _mappingDal.ExistsAsync(createDto.AssessmentComponentId, createDto.CourseLearningOutcomeId))
                throw new InvalidOperationException("Bu component için CLO eşlemesi zaten var.");
            var created = await AddAsync(createDto);
            await _mudekStale.MarkStaleByExamIdAsync(component.ExamId);
            return created;
        }

        public async Task<AssessmentComponentOutcomeMappingDto> UpdateForTeacherAsync(UpdateAssessmentComponentOutcomeMappingDto updateDto, Guid teacherId)
        {
            var snapshot = await _mappingDal.GetByIdWithOwnershipAsync(updateDto.Id)
                ?? throw new KeyNotFoundException("Mapping bulunamadı.");
            if (snapshot.AssessmentComponent?.Exam?.CourseEvaluation?.CourseOffering?.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bu mapping size ait değil.");
            if (updateDto.Weight < 0 || updateDto.Weight > 1)
                throw new InvalidOperationException("Weight değeri 0 ile 1 arasında olmalıdır.");
            var updated = await UpdateAsync(updateDto);
            if (snapshot.AssessmentComponent != null)
                await _mudekStale.MarkStaleByExamIdAsync(snapshot.AssessmentComponent.ExamId);
            return updated;
        }

        public async Task DeleteForTeacherAsync(Guid id, Guid teacherId)
        {
            var snapshot = await _mappingDal.GetByIdWithOwnershipAsync(id)
                ?? throw new KeyNotFoundException("Mapping bulunamadı.");
            if (snapshot.AssessmentComponent?.Exam?.CourseEvaluation?.CourseOffering?.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bu mapping size ait değil.");
            var examId = snapshot.AssessmentComponent?.ExamId;
            await DeleteAsync(id);
            if (examId.HasValue) await _mudekStale.MarkStaleByExamIdAsync(examId.Value);
        }

        private async Task<AssessmentComponent> EnsureComponentOwnershipAsync(Guid componentId, Guid teacherId)
        {
            var component = await _componentDal.GetByIdWithOwnershipAsync(componentId)
                ?? throw new KeyNotFoundException("Component bulunamadı.");
            if (component.Exam?.CourseEvaluation?.CourseOffering?.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bu component size ait değil.");
            return component;
        }
    }
}
