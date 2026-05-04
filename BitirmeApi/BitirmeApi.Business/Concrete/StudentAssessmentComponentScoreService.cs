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
        private readonly IMudekEvaluationCalculatorService _mudekStale;

        public StudentAssessmentComponentScoreService(
            IStudentAssessmentComponentScoreDal scoreDal,
            IAssessmentComponentDal componentDal,
            IMudekEvaluationCalculatorService mudekStale)
        {
            _scoreDal = scoreDal;
            _componentDal = componentDal;
            _mudekStale = mudekStale;
        }

        public async Task<List<StudentAssessmentComponentScoreDto>> GetByComponentForTeacherAsync(Guid componentId, int externalTeacherId)
        {
            await EnsureComponentOwnershipAsync(componentId, externalTeacherId);
            var entities = await _scoreDal.GetByComponentIdWithDetailsAsync(componentId);
            return entities.Select(s => MapToDto(s)).ToList();
        }

        public async Task<StudentAssessmentComponentScoreDto> AddForTeacherAsync(CreateStudentAssessmentComponentScoreDto createDto, int externalTeacherId)
        {
            var component = await EnsureComponentOwnershipAsync(createDto.AssessmentComponentId, externalTeacherId);
            if (createDto.Score.HasValue && (createDto.Score < 0 || createDto.Score > component.MaxScore))
                throw new InvalidOperationException($"Score 0 ile {component.MaxScore} arasında olmalıdır.");
            if (await _scoreDal.ExistsAsync(createDto.AssessmentComponentId, createDto.ExternalStudentId))
                throw new InvalidOperationException("Bu component ve öğrenci için score zaten mevcut.");

            var entity = new StudentAssessmentComponentScore
            {
                Id = Guid.NewGuid(),
                AssessmentComponentId = createDto.AssessmentComponentId,
                ExternalStudentId = createDto.ExternalStudentId,
                Score = createDto.Score,
                Notes = createDto.Notes,
                EvaluatedBy = createDto.EvaluatedBy,
                EvaluatedAt = createDto.EvaluatedBy != null ? DateTime.UtcNow : null,
                CreatedAt = DateTime.UtcNow
            };
            _scoreDal.Add(entity);
            await _scoreDal.SaveChangesAsync();
            await _mudekStale.MarkStaleByExamIdAsync(component.ExamId);
            return MapToDto(entity);
        }

        public async Task<BulkOperationResultDto<int>> AddBulkForTeacherAsync(Guid componentId, List<StudentScoreItem> items, int externalTeacherId)
        {
            var result = new BulkOperationResultDto<int>();
            foreach (var item in items)
            {
                try
                {
                    await AddForTeacherAsync(new CreateStudentAssessmentComponentScoreDto
                    {
                        AssessmentComponentId = componentId,
                        ExternalStudentId = item.ExternalStudentId,
                        Score = item.Score,
                        Notes = item.Notes
                    }, externalTeacherId);
                    result.Success.Add(item.ExternalStudentId);
                }
                catch (Exception ex)
                {
                    result.Failed.Add(item.ExternalStudentId);
                    result.Errors.Add($"{item.ExternalStudentId}: {ex.Message}");
                }
            }

            var comp = await _componentDal.GetAsync(c => c.Id == componentId);
            if (comp != null) await _mudekStale.MarkStaleByExamIdAsync(comp.ExamId);
            return result;
        }

        public async Task<StudentAssessmentComponentScoreDto> UpdateForTeacherAsync(UpdateStudentAssessmentComponentScoreDto updateDto, int externalTeacherId)
        {
            var snapshot = await _scoreDal.GetByIdWithOwnershipAsync(updateDto.Id)
                ?? throw new KeyNotFoundException("Score bulunamadı.");
            EnsureEvaluationOwnership(snapshot.AssessmentComponent?.Exam?.CourseEvaluation);
            if (updateDto.Score.HasValue && (updateDto.Score < 0 || updateDto.Score > snapshot.AssessmentComponent!.MaxScore))
                throw new InvalidOperationException($"Score 0 ile {snapshot.AssessmentComponent.MaxScore} arasında olmalıdır.");

            var tracked = await _scoreDal.GetAsync(s => s.Id == updateDto.Id)
                ?? throw new KeyNotFoundException("Score bulunamadı.");
            tracked.Score = updateDto.Score;
            tracked.Notes = updateDto.Notes;
            tracked.EvaluatedBy = updateDto.EvaluatedBy;
            tracked.EvaluatedAt = updateDto.EvaluatedAt;
            tracked.UpdatedAt = DateTime.UtcNow;
            _scoreDal.Update(tracked);
            await _scoreDal.SaveChangesAsync();
            await _mudekStale.MarkStaleByExamIdAsync(snapshot.AssessmentComponent!.ExamId);
            return MapToDto(tracked);
        }

        public async Task DeleteForTeacherAsync(Guid id, int externalTeacherId)
        {
            var snapshot = await _scoreDal.GetByIdWithOwnershipAsync(id)
                ?? throw new KeyNotFoundException("Score bulunamadı.");
            EnsureEvaluationOwnership(snapshot.AssessmentComponent?.Exam?.CourseEvaluation);
            var tracked = await _scoreDal.GetAsync(s => s.Id == id)
                ?? throw new KeyNotFoundException("Score bulunamadı.");
            _scoreDal.Delete(tracked);
            await _scoreDal.SaveChangesAsync();
        }

        private async Task<AssessmentComponent> EnsureComponentOwnershipAsync(Guid componentId, int externalTeacherId)
        {
            var component = await _componentDal.GetByIdWithOwnershipAsync(componentId)
                ?? throw new KeyNotFoundException("Component bulunamadı.");
            EnsureEvaluationOwnership(component.Exam?.CourseEvaluation);
            return component;
        }

        private static void EnsureEvaluationOwnership(CourseEvaluation? evaluation)
        {
            if (evaluation == null)
                throw new UnauthorizedAccessException("Bu işlem için yetkiniz yok.");
        }

        private static StudentAssessmentComponentScoreDto MapToDto(StudentAssessmentComponentScore s) =>
            new StudentAssessmentComponentScoreDto
            {
                Id = s.Id,
                AssessmentComponentId = s.AssessmentComponentId,
                ExternalStudentId = s.ExternalStudentId,
                Score = s.Score,
                Notes = s.Notes,
                EvaluatedBy = s.EvaluatedBy,
                EvaluatedAt = s.EvaluatedAt,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            };
    }
}
