using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    public interface IAssessmentComponentOutcomeMappingService
    {
        Task<List<AssessmentComponentOutcomeMappingDto>> GetAllAsync();
        Task<AssessmentComponentOutcomeMappingDto?> GetByIdAsync(Guid id);
        Task<List<AssessmentComponentOutcomeMappingDto>> GetByAssessmentComponentIdAsync(Guid assessmentComponentId);
        Task<AssessmentComponentOutcomeMappingDto> AddAsync(CreateAssessmentComponentOutcomeMappingDto createDto);
        Task<AssessmentComponentOutcomeMappingDto> UpdateAsync(UpdateAssessmentComponentOutcomeMappingDto updateDto);
        Task DeleteAsync(Guid id);

        Task<List<AssessmentComponentOutcomeMappingDto>> GetByComponentIdForTeacherAsync(Guid componentId, Guid teacherId);
        Task<AssessmentComponentOutcomeMappingDto> AddForTeacherAsync(CreateAssessmentComponentOutcomeMappingDto createDto, Guid teacherId);
        Task<AssessmentComponentOutcomeMappingDto> UpdateForTeacherAsync(UpdateAssessmentComponentOutcomeMappingDto updateDto, Guid teacherId);
        Task DeleteForTeacherAsync(Guid id, Guid teacherId);
    }
}
