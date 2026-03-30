using AutoMapper;
using BitirmeApi.Business.Abstract;
using BitirmeApi.Business.DTO;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.Business.Concrete
{
    public class AssessmentComponentService : IAssessmentComponentService
    {
        private readonly IAssessmentComponentDal _componentDal;
        private readonly IExamDal _examDal;
        private readonly IStudentAssessmentComponentScoreDal _scoreDal;
        private readonly IAssessmentComponentOutcomeMappingDal _mappingDal;
        private readonly IMapper _mapper;
        private readonly IMudekEvaluationCalculatorService _mudekStale;

        public AssessmentComponentService(
            IAssessmentComponentDal componentDal,
            IExamDal examDal,
            IStudentAssessmentComponentScoreDal scoreDal,
            IAssessmentComponentOutcomeMappingDal mappingDal,
            IMapper mapper,
            IMudekEvaluationCalculatorService mudekStale)
        {
            _componentDal = componentDal;
            _examDal = examDal;
            _scoreDal = scoreDal;
            _mappingDal = mappingDal;
            _mapper = mapper;
            _mudekStale = mudekStale;
        }

        public async Task<List<AssessmentComponentListDto>> GetAllAsync()
        {
            var entities = await _componentDal.GetListAsync();
            return _mapper.Map<List<AssessmentComponentListDto>>(entities.ToList());
        }

        public async Task<AssessmentComponentDto?> GetByIdAsync(Guid id)
        {
            var entity = await _componentDal.GetByIdWithDetailsAsync(id);
            return entity != null ? _mapper.Map<AssessmentComponentDto>(entity) : null;
        }

        public async Task<List<AssessmentComponentListDto>> GetByExamIdAsync(Guid examId)
        {
            return _mapper.Map<List<AssessmentComponentListDto>>(
                await _componentDal.GetByExamIdWithDetailsAsync(examId));
        }

        public async Task<AssessmentComponentDto> AddAsync(CreateAssessmentComponentDto createDto)
        {
            var entity = _mapper.Map<AssessmentComponent>(createDto);
            entity.CreatedAt = DateTime.UtcNow;
            entity.IsActive = true;
            var addedEntity = _componentDal.Add(entity);
            await _componentDal.SaveChangesAsync();
            return _mapper.Map<AssessmentComponentDto>((await _componentDal.GetByIdWithDetailsAsync(addedEntity.Id))!);
        }

        public async Task<AssessmentComponentDto> UpdateAsync(UpdateAssessmentComponentDto updateDto)
        {
            var existingEntity = await _componentDal.GetAsync(c => c.Id == updateDto.Id);
            if (existingEntity == null)
                throw new KeyNotFoundException($"Assessment component with ID {updateDto.Id} not found");

            _mapper.Map(updateDto, existingEntity);
            existingEntity.UpdatedAt = DateTime.UtcNow;
            _componentDal.Update(existingEntity);
            await _componentDal.SaveChangesAsync();
            return _mapper.Map<AssessmentComponentDto>((await _componentDal.GetByIdWithDetailsAsync(updateDto.Id))!);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _componentDal.GetAsync(c => c.Id == id);
            if (entity == null) throw new KeyNotFoundException($"Assessment component with ID {id} not found");
            _componentDal.Delete(entity);
            await _componentDal.SaveChangesAsync();
        }

        public async Task<List<AssessmentComponentListDto>> GetByExamIdForTeacherAsync(Guid examId, Guid teacherId)
        {
            var exam = await _examDal.GetByIdWithOwnershipAsync(examId)
                ?? throw new KeyNotFoundException("Sınav bulunamadı.");
            if (exam.CourseEvaluation?.CourseOffering?.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bu sınav size ait değil.");
            return await GetByExamIdAsync(examId);
        }

        public async Task<AssessmentComponentDto?> GetByIdForTeacherAsync(Guid id, Guid teacherId)
        {
            var component = await _componentDal.GetByIdWithOwnershipAsync(id);
            if (component == null) return null;
            if (component.Exam?.CourseEvaluation?.CourseOffering?.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bu component size ait değil.");
            return await GetByIdAsync(id);
        }

        public async Task<AssessmentComponentDto> AddForTeacherAsync(CreateAssessmentComponentDto createDto, Guid teacherId)
        {
            var exam = await _examDal.GetByIdWithOwnershipAsync(createDto.ExamId)
                ?? throw new KeyNotFoundException("Sınav bulunamadı.");
            if (exam.CourseEvaluation?.CourseOffering?.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bu sınav size ait değil.");
            if (createDto.MaxScore <= 0) throw new InvalidOperationException("MaxScore 0'dan büyük olmalıdır.");
            if (createDto.WeightPercentage.HasValue && (createDto.WeightPercentage < 0 || createDto.WeightPercentage > 100))
                throw new InvalidOperationException("WeightPercentage 0-100 aralığında olmalıdır.");
            if ((await _componentDal.GetListAsync(c => c.ExamId == createDto.ExamId && c.OrderIndex == createDto.OrderIndex)).Any())
                throw new InvalidOperationException("Aynı sınav altında OrderIndex benzersiz olmalıdır.");
            var added = await AddAsync(createDto);
            await _mudekStale.MarkStaleByExamIdAsync(createDto.ExamId);
            return added;
        }

        public async Task<AssessmentComponentDto> UpdateForTeacherAsync(UpdateAssessmentComponentDto updateDto, Guid teacherId)
        {
            var snapshot = await _componentDal.GetByIdWithOwnershipAsync(updateDto.Id)
                ?? throw new KeyNotFoundException("Component bulunamadı.");
            if (snapshot.Exam?.CourseEvaluation?.CourseOffering?.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bu component size ait değil.");
            if (updateDto.MaxScore <= 0) throw new InvalidOperationException("MaxScore 0'dan büyük olmalıdır.");
            if (updateDto.WeightPercentage.HasValue && (updateDto.WeightPercentage < 0 || updateDto.WeightPercentage > 100))
                throw new InvalidOperationException("WeightPercentage 0-100 aralığında olmalıdır.");
            if ((await _componentDal.GetListAsync(c => c.ExamId == snapshot.ExamId && c.OrderIndex == updateDto.OrderIndex && c.Id != updateDto.Id)).Any())
                throw new InvalidOperationException("Aynı sınav altında OrderIndex benzersiz olmalıdır.");
            var updated = await UpdateAsync(updateDto);
            await _mudekStale.MarkStaleByExamIdAsync(snapshot.ExamId);
            return updated;
        }

        public async Task DeleteForTeacherAsync(Guid id, Guid teacherId)
        {
            var snapshot = await _componentDal.GetByIdWithOwnershipAsync(id)
                ?? throw new KeyNotFoundException("Component bulunamadı.");
            if (snapshot.Exam?.CourseEvaluation?.CourseOffering?.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bu component size ait değil.");
            if ((await _scoreDal.GetListAsync(s => s.AssessmentComponentId == id)).Any() ||
                (await _mappingDal.GetListAsync(m => m.AssessmentComponentId == id)).Any())
                throw new InvalidOperationException("Component altında score/mapping olduğu için silinemez.");
            var examId = snapshot.ExamId;
            await DeleteAsync(id);
            await _mudekStale.MarkStaleByExamIdAsync(examId);
        }
    }
}
