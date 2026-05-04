using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    public interface IStudentAnswerService
    {
        Task<List<StudentAnswerDto>> GetByQuestionForTeacherAsync(Guid questionId, int externalTeacherId);
        Task<StudentAnswerDto> AddForTeacherAsync(CreateStudentAnswerDto dto, int externalTeacherId);
        Task<BulkOperationResultDto<int>> AddBulkForTeacherAsync(Guid questionId, List<BulkStudentAnswerItemDto> items, int externalTeacherId);
        Task<StudentAnswerDto> UpdateForTeacherAsync(UpdateStudentAnswerDto dto, int externalTeacherId);
        Task DeleteForTeacherAsync(Guid id, int externalTeacherId);
    }
}
