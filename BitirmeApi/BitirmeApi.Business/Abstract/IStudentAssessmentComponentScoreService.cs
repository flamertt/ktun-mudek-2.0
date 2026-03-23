using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    public interface IStudentAssessmentComponentScoreService
    {
        Task<List<StudentAssessmentComponentScoreDto>> GetAllAsync();
        Task<StudentAssessmentComponentScoreDto?> GetByIdAsync(Guid id);
        Task<List<StudentAssessmentComponentScoreDto>> GetByAssessmentComponentIdAsync(Guid assessmentComponentId);
        Task<List<StudentAssessmentComponentScoreDto>> GetByEnrollmentIdAsync(Guid enrollmentId);
        Task<StudentAssessmentComponentScoreDto> AddAsync(CreateStudentAssessmentComponentScoreDto createDto);
        Task<StudentAssessmentComponentScoreDto> UpdateAsync(UpdateStudentAssessmentComponentScoreDto updateDto);
        Task DeleteAsync(Guid id);

        Task<List<StudentAssessmentComponentScoreDto>> GetByComponentForTeacherAsync(Guid componentId, Guid teacherId);
        Task<StudentAssessmentComponentScoreDto> AddForTeacherAsync(CreateStudentAssessmentComponentScoreDto createDto, Guid teacherId);
        Task<BulkOperationResultDto<Guid>> AddBulkForTeacherAsync(Guid componentId, List<StudentScoreItem> items, Guid teacherId);
        Task<StudentAssessmentComponentScoreDto> UpdateForTeacherAsync(UpdateStudentAssessmentComponentScoreDto updateDto, Guid teacherId);
        Task DeleteForTeacherAsync(Guid id, Guid teacherId);
    }
}
