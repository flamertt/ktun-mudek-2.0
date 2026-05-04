using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    public interface IExamService
    {
        Task<List<ExamListDto>> GetByEvaluationIdAsync(Guid evaluationId);
        Task<List<ExamListDto>> GetByEvaluationIdForTeacherAsync(Guid evaluationId, int externalTeacherId);

        Task<ExamDetailDto?> GetByIdAsync(Guid id);
        Task<ExamDetailDto?> GetByIdForTeacherAsync(Guid id, int externalTeacherId);

        Task<ExamDetailDto> CreateAsync(CreateExamDto dto, int externalTeacherId);
        Task<ExamDetailDto> UpdateAsync(UpdateExamDto dto, int externalTeacherId);
        Task DeleteAsync(Guid id, int externalTeacherId);
    }
}
