using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    public interface IExamQuestionOutcomeMappingService
    {
        Task<List<ExamQuestionOutcomeMappingDto>> GetAllAsync();
        Task<ExamQuestionOutcomeMappingDto?> GetByIdAsync(Guid id);
        Task<List<ExamQuestionOutcomeMappingDto>> GetByExamQuestionIdAsync(Guid examQuestionId);
        Task<ExamQuestionOutcomeMappingDto> AddAsync(CreateExamQuestionOutcomeMappingDto createDto);
        Task<ExamQuestionOutcomeMappingDto> UpdateAsync(UpdateExamQuestionOutcomeMappingDto updateDto);
        Task DeleteAsync(Guid id);

        // Teacher ownership enforced
        Task<List<ExamQuestionOutcomeMappingDto>> GetByQuestionIdForTeacherAsync(Guid questionId, Guid teacherId);
        Task<ExamQuestionOutcomeMappingDto> AddForTeacherAsync(CreateExamQuestionOutcomeMappingDto createDto, Guid teacherId);
        Task<ExamQuestionOutcomeMappingDto> UpdateForTeacherAsync(UpdateExamQuestionOutcomeMappingDto updateDto, Guid teacherId);
        Task DeleteForTeacherAsync(Guid id, Guid teacherId);
    }
}
