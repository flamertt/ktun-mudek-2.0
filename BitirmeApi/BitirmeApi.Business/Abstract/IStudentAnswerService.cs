using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    public interface IStudentAnswerService
    {
        Task<List<StudentAnswerDto>> GetByQuestionForTeacherAsync(Guid questionId, Guid teacherId);
        Task<StudentAnswerDto> AddForTeacherAsync(CreateStudentAnswerDto dto, Guid teacherId);
        Task<BulkOperationResultDto<Guid>> AddBulkForTeacherAsync(Guid questionId, List<BulkStudentAnswerItemDto> items, Guid teacherId);
        Task<StudentAnswerDto> UpdateForTeacherAsync(UpdateStudentAnswerDto dto, Guid teacherId);
        Task DeleteForTeacherAsync(Guid id, Guid teacherId);
    }
}
