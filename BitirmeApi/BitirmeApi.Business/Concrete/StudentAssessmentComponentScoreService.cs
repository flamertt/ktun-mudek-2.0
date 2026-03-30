using AutoMapper;
using BitirmeApi.Business.Abstract;
using BitirmeApi.Business.DTO;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.Business.Concrete
{
    public class StudentAssessmentComponentScoreService : IStudentAssessmentComponentScoreService
    {
        private readonly IStudentAssessmentComponentScoreDal _scoreDal;
        private readonly IAssessmentComponentDal _componentDal;
        private readonly IEnrollmentDal _enrollmentDal;
        private readonly IMapper _mapper;
        private readonly IMudekEvaluationCalculatorService _mudekStale;

        public StudentAssessmentComponentScoreService(
            IStudentAssessmentComponentScoreDal scoreDal,
            IAssessmentComponentDal componentDal,
            IEnrollmentDal enrollmentDal,
            IMapper mapper,
            IMudekEvaluationCalculatorService mudekStale)
        {
            _scoreDal = scoreDal;
            _componentDal = componentDal;
            _enrollmentDal = enrollmentDal;
            _mapper = mapper;
            _mudekStale = mudekStale;
        }

        public async Task<List<StudentAssessmentComponentScoreDto>> GetAllAsync()
        {
            var entities = await _scoreDal.GetListAsync();
            return _mapper.Map<List<StudentAssessmentComponentScoreDto>>(entities.ToList());
        }

        public async Task<StudentAssessmentComponentScoreDto?> GetByIdAsync(Guid id)
        {
            var entity = await _scoreDal.GetAsync(s => s.Id == id);
            return entity != null ? _mapper.Map<StudentAssessmentComponentScoreDto>(entity) : null;
        }

        public async Task<List<StudentAssessmentComponentScoreDto>> GetByAssessmentComponentIdAsync(Guid assessmentComponentId)
        {
            var entities = await _scoreDal.GetListAsync(s => s.AssessmentComponentId == assessmentComponentId);
            return _mapper.Map<List<StudentAssessmentComponentScoreDto>>(entities.ToList());
        }

        public async Task<List<StudentAssessmentComponentScoreDto>> GetByEnrollmentIdAsync(Guid enrollmentId)
        {
            var entities = await _scoreDal.GetListAsync(s => s.EnrollmentId == enrollmentId);
            return _mapper.Map<List<StudentAssessmentComponentScoreDto>>(entities.ToList());
        }

        public async Task<StudentAssessmentComponentScoreDto> AddAsync(CreateStudentAssessmentComponentScoreDto createDto)
        {
            var entity = _mapper.Map<StudentAssessmentComponentScore>(createDto);
            entity.CreatedAt = DateTime.Now;
            if (createDto.EvaluatedBy != null)
                entity.EvaluatedAt = DateTime.Now;
            
            var addedEntity = _scoreDal.Add(entity);
            await _scoreDal.SaveChangesAsync();
            return await Task.FromResult(_mapper.Map<StudentAssessmentComponentScoreDto>(addedEntity));
        }

        public async Task<StudentAssessmentComponentScoreDto> UpdateAsync(UpdateStudentAssessmentComponentScoreDto updateDto)
        {
            var existingEntity = await _scoreDal.GetAsync(s => s.Id == updateDto.Id);
            if (existingEntity == null)
                throw new Exception($"Score with ID {updateDto.Id} not found");

            _mapper.Map(updateDto, existingEntity);
            existingEntity.UpdatedAt = DateTime.Now;
            var updatedEntity = _scoreDal.Update(existingEntity);
            await _scoreDal.SaveChangesAsync();
            return await Task.FromResult(_mapper.Map<StudentAssessmentComponentScoreDto>(updatedEntity));
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _scoreDal.GetAsync(s => s.Id == id);
            if (entity != null)
            {
                _scoreDal.Delete(entity);
                await _scoreDal.SaveChangesAsync();
            }
        }

        public async Task<List<StudentAssessmentComponentScoreDto>> GetByComponentForTeacherAsync(Guid componentId, Guid teacherId)
        {
            var component = await EnsureComponentOwnershipAsync(componentId, teacherId);
            var entities = await _scoreDal.GetByComponentIdWithDetailsAsync(componentId);
            return _mapper.Map<List<StudentAssessmentComponentScoreDto>>(entities);
        }

        public async Task<StudentAssessmentComponentScoreDto> AddForTeacherAsync(CreateStudentAssessmentComponentScoreDto createDto, Guid teacherId)
        {
            var component = await EnsureComponentOwnershipAsync(createDto.AssessmentComponentId, teacherId);
            var enrollment = await _enrollmentDal.GetAsync(e => e.Id == createDto.EnrollmentId)
                ?? throw new KeyNotFoundException("Enrollment bulunamadı.");
            if (enrollment.CourseOfferingId != component.Exam?.CourseEvaluation?.CourseOfferingId)
                throw new InvalidOperationException("Enrollment bu component'in offering kaydına ait değil.");
            if (createDto.Score.HasValue && (createDto.Score < 0 || createDto.Score > component.MaxScore))
                throw new InvalidOperationException($"Score 0 ile {component.MaxScore} arasında olmalıdır.");
            if (await _scoreDal.ExistsAsync(createDto.AssessmentComponentId, createDto.EnrollmentId))
                throw new InvalidOperationException("Bu component ve enrollment için score zaten mevcut.");
            var dto = await AddAsync(createDto);
            await _mudekStale.MarkStaleByEnrollmentIdAsync(createDto.EnrollmentId);
            return dto;
        }

        public async Task<BulkOperationResultDto<Guid>> AddBulkForTeacherAsync(Guid componentId, List<StudentScoreItem> items, Guid teacherId)
        {
            var result = new BulkOperationResultDto<Guid>();
            foreach (var item in items)
            {
                try
                {
                    await AddForTeacherAsync(new CreateStudentAssessmentComponentScoreDto
                    {
                        AssessmentComponentId = componentId,
                        EnrollmentId = item.EnrollmentId,
                        Score = item.Score,
                        Notes = item.Notes
                    }, teacherId);
                    result.Success.Add(item.EnrollmentId);
                }
                catch (Exception ex)
                {
                    result.Failed.Add(item.EnrollmentId);
                    result.Errors.Add($"{item.EnrollmentId}: {ex.Message}");
                }
            }

            var comp = await _componentDal.GetAsync(c => c.Id == componentId);
            if (comp != null) await _mudekStale.MarkStaleByExamIdAsync(comp.ExamId);
            return result;
        }

        public async Task<StudentAssessmentComponentScoreDto> UpdateForTeacherAsync(UpdateStudentAssessmentComponentScoreDto updateDto, Guid teacherId)
        {
            var snapshot = await _scoreDal.GetByIdWithOwnershipAsync(updateDto.Id)
                ?? throw new KeyNotFoundException("Score bulunamadı.");
            if (snapshot.AssessmentComponent?.Exam?.CourseEvaluation?.CourseOffering?.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bu score sizin dersinize ait değil.");
            if (updateDto.Score.HasValue && (updateDto.Score < 0 || updateDto.Score > snapshot.AssessmentComponent.MaxScore))
                throw new InvalidOperationException($"Score 0 ile {snapshot.AssessmentComponent.MaxScore} arasında olmalıdır.");
            var dto = await UpdateAsync(updateDto);
            await _mudekStale.MarkStaleByEnrollmentIdAsync(snapshot.EnrollmentId);
            return dto;
        }

        public async Task DeleteForTeacherAsync(Guid id, Guid teacherId)
        {
            var snapshot = await _scoreDal.GetByIdWithOwnershipAsync(id)
                ?? throw new KeyNotFoundException("Score bulunamadı.");
            if (snapshot.AssessmentComponent?.Exam?.CourseEvaluation?.CourseOffering?.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bu score sizin dersinize ait değil.");
            var enId = snapshot.EnrollmentId;
            await DeleteAsync(id);
            await _mudekStale.MarkStaleByEnrollmentIdAsync(enId);
        }

        private async Task<AssessmentComponent> EnsureComponentOwnershipAsync(Guid componentId, Guid teacherId)
        {
            var component = await _componentDal.GetByIdWithOwnershipAsync(componentId)
                ?? throw new KeyNotFoundException("Component bulunamadı.");
            if (component.Exam?.CourseEvaluation?.CourseOffering?.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bu component sizin dersinize ait değil.");
            return component;
        }
    }
}
