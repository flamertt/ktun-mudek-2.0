using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    public interface IExamService
    {
        /// <summary>Bir değerlendirmeye ait sınavlar</summary>
        Task<List<ExamListDto>> GetByEvaluationIdAsync(Guid evaluationId);
        Task<List<ExamListDto>> GetByEvaluationIdForTeacherAsync(Guid evaluationId, Guid teacherId);

        Task<ExamDetailDto?> GetByIdAsync(Guid id);
        Task<ExamDetailDto?> GetByIdForTeacherAsync(Guid id, Guid teacherId);

        /// <summary>
        /// Değerlendirmeye sınav ekler.
        /// Ownership: CourseEvaluation.CourseOffering.TeacherId == teacherId
        /// </summary>
        Task<ExamDetailDto> CreateAsync(CreateExamDto dto, Guid teacherId);

        Task<ExamDetailDto> UpdateAsync(UpdateExamDto dto, Guid teacherId);

        Task DeleteAsync(Guid id, Guid teacherId);
    }
}
