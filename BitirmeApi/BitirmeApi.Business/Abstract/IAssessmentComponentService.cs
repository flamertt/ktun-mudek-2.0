using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    public interface IAssessmentComponentService
    {
        Task<List<AssessmentComponentListDto>> GetAllAsync();
        Task<AssessmentComponentDto?> GetByIdAsync(Guid id);
        Task<List<AssessmentComponentListDto>> GetByExamIdAsync(Guid examId);
        Task<AssessmentComponentDto> AddAsync(CreateAssessmentComponentDto createDto);
        Task<AssessmentComponentDto> UpdateAsync(UpdateAssessmentComponentDto updateDto);
        Task DeleteAsync(Guid id);

        Task<List<AssessmentComponentListDto>> GetByExamIdForTeacherAsync(Guid examId, int externalTeacherId);
        Task<AssessmentComponentDto?> GetByIdForTeacherAsync(Guid id, int externalTeacherId);
        Task<AssessmentComponentDto> AddForTeacherAsync(CreateAssessmentComponentDto createDto, int externalTeacherId);
        Task<AssessmentComponentDto> UpdateForTeacherAsync(UpdateAssessmentComponentDto updateDto, int externalTeacherId);
        Task DeleteForTeacherAsync(Guid id, int externalTeacherId);
    }
}
