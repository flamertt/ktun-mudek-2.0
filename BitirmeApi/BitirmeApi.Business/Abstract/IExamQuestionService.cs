using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    public interface IExamQuestionService
    {
        Task<List<ExamQuestionDto>> GetByExamIdAsync(Guid examId);
        Task<List<ExamQuestionDto>> GetByExamIdForTeacherAsync(Guid examId, Guid teacherId);
        Task<ExamQuestionDto?> GetByIdAsync(Guid id);
        Task<ExamQuestionDto?> GetByIdForTeacherAsync(Guid id, Guid teacherId);

        /// <summary>Sınava soru ekler — ownership: sınav bu öğretmene ait mi?</summary>
        Task<ExamQuestionDto> CreateAsync(CreateExamQuestionDto dto, Guid teacherId);

        Task<ExamQuestionDto> UpdateAsync(UpdateExamQuestionDto dto, Guid teacherId);

        Task DeleteAsync(Guid id, Guid teacherId);

        // ── CLO Eşleme ────────────────────────────────────────────────────────

        /// <summary>Soruyu bir CLO ile eşler (ağırlıklı)</summary>
        Task<ExamQuestionOutcomeMappingDto> MapToOutcomeAsync(Guid questionId, Guid cloId, decimal weight, Guid teacherId);

        /// <summary>Eşleme ağırlığını günceller</summary>
        Task<ExamQuestionOutcomeMappingDto> UpdateMappingWeightAsync(Guid questionId, Guid cloId, decimal weight, Guid teacherId);

        /// <summary>Soru–CLO eşlemesini kaldırır</summary>
        Task UnmapOutcomeAsync(Guid questionId, Guid cloId, Guid teacherId);
    }
}
