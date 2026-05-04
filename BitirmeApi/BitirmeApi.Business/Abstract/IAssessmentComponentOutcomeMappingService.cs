using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    public interface IAssessmentComponentOutcomeMappingService
    {
        Task<List<AssessmentComponentOutcomeMappingDto>> GetByComponentIdForTeacherAsync(Guid componentId, int externalTeacherId);
        Task<AssessmentComponentOutcomeMappingDto> AddForTeacherAsync(CreateAssessmentComponentOutcomeMappingDto createDto, int externalTeacherId);
        Task<AssessmentComponentOutcomeMappingDto> UpdateForTeacherAsync(UpdateAssessmentComponentOutcomeMappingDto updateDto, int externalTeacherId);
        Task DeleteForTeacherAsync(Guid id, int externalTeacherId);
    }
}
