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

        Task<List<AssessmentComponentListDto>> GetByExamIdForTeacherAsync(Guid examId, Guid teacherId);
        Task<AssessmentComponentDto?> GetByIdForTeacherAsync(Guid id, Guid teacherId);
        Task<AssessmentComponentDto> AddForTeacherAsync(CreateAssessmentComponentDto createDto, Guid teacherId);
        Task<AssessmentComponentDto> UpdateForTeacherAsync(UpdateAssessmentComponentDto updateDto, Guid teacherId);
        Task DeleteForTeacherAsync(Guid id, Guid teacherId);
    }
}
