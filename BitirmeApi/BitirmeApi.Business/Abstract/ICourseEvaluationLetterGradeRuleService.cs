using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    public interface ICourseEvaluationLetterGradeRuleService
    {
        Task<List<CourseEvaluationLetterGradeRuleDto>> GetAllAsync();
        Task<CourseEvaluationLetterGradeRuleDto?> GetByIdAsync(Guid id);
        Task<List<CourseEvaluationLetterGradeRuleDto>> GetByCourseEvaluationIdAsync(Guid courseEvaluationId);
        Task<CourseEvaluationLetterGradeRuleDto> AddAsync(CreateCourseEvaluationLetterGradeRuleDto createDto);
        Task<CourseEvaluationLetterGradeRuleDto> UpdateAsync(UpdateCourseEvaluationLetterGradeRuleDto updateDto);
        Task DeleteAsync(Guid id);

        Task<List<CourseEvaluationLetterGradeRuleDto>> GetByEvaluationForTeacherAsync(Guid evaluationId, Guid teacherId);
        Task<CourseEvaluationLetterGradeRuleDto> AddForTeacherAsync(CreateCourseEvaluationLetterGradeRuleDto createDto, Guid teacherId);
        Task<CourseEvaluationLetterGradeRuleDto> UpdateForTeacherAsync(UpdateCourseEvaluationLetterGradeRuleDto updateDto, Guid teacherId);
        Task DeleteForTeacherAsync(Guid id, Guid teacherId);
    }
}
