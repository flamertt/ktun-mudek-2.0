using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    public interface IExamQuestionOutcomeMappingService
    {
        Task<List<ExamQuestionOutcomeMappingDto>> GetByQuestionIdForTeacherAsync(Guid questionId, int externalTeacherId);
        Task<ExamQuestionOutcomeMappingDto> AddForTeacherAsync(CreateExamQuestionOutcomeMappingDto createDto, int externalTeacherId);
        Task<ExamQuestionOutcomeMappingDto> UpdateForTeacherAsync(UpdateExamQuestionOutcomeMappingDto updateDto, int externalTeacherId);
        Task DeleteForTeacherAsync(Guid id, int externalTeacherId);
    }
}
