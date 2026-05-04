using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    public interface IStudentAssessmentComponentScoreService
    {
        Task<List<StudentAssessmentComponentScoreDto>> GetByComponentForTeacherAsync(Guid componentId, int externalTeacherId);
        Task<StudentAssessmentComponentScoreDto> AddForTeacherAsync(CreateStudentAssessmentComponentScoreDto createDto, int externalTeacherId);
        Task<BulkOperationResultDto<int>> AddBulkForTeacherAsync(Guid componentId, List<StudentScoreItem> items, int externalTeacherId);
        Task<StudentAssessmentComponentScoreDto> UpdateForTeacherAsync(UpdateStudentAssessmentComponentScoreDto updateDto, int externalTeacherId);
        Task DeleteForTeacherAsync(Guid id, int externalTeacherId);
    }
}
