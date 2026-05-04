using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    public interface IExamQuestionService
    {
        Task<List<ExamQuestionDto>> GetByExamIdAsync(Guid examId);
        Task<List<ExamQuestionDto>> GetByExamIdForTeacherAsync(Guid examId, int externalTeacherId);
        Task<ExamQuestionDto?> GetByIdAsync(Guid id);
        Task<ExamQuestionDto?> GetByIdForTeacherAsync(Guid id, int externalTeacherId);

        Task<ExamQuestionDto> CreateAsync(CreateExamQuestionDto dto, int externalTeacherId);
        Task<ExamQuestionDto> UpdateAsync(UpdateExamQuestionDto dto, int externalTeacherId);
        Task DeleteAsync(Guid id, int externalTeacherId);

        // CLO eşleme — artık ExamQuestionOutcomeMappingService ayrı manage ediyor
        Task<ExamQuestionOutcomeMappingDto> MapToOutcomeAsync(Guid questionId, Guid cloId, decimal weight, int externalTeacherId);
        Task<ExamQuestionOutcomeMappingDto> UpdateMappingWeightAsync(Guid questionId, Guid cloId, decimal weight, int externalTeacherId);
        Task UnmapOutcomeAsync(Guid questionId, Guid cloId, int externalTeacherId);
    }
}
