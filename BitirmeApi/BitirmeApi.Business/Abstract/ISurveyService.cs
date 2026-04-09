using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    public interface ISurveyService
    {
        // ── Anket CRUD ─────────────────────────────────────────────────────────────
        Task<List<SurveyListDto>> GetByOfferingIdAsync(Guid offeringId, Guid teacherId);
        Task<SurveyDetailDto?> GetByIdAsync(Guid id, Guid teacherId);
        Task<SurveyDetailDto> CreateAsync(CreateSurveyDto dto, Guid teacherId);
        Task<SurveyDetailDto> UpdateAsync(UpdateSurveyDto dto, Guid teacherId);
        Task DeleteAsync(Guid id, Guid teacherId);
        Task ToggleActiveAsync(Guid id, Guid teacherId);

        // ── Soru (Likert) CRUD ─────────────────────────────────────────────────────
        Task<SurveyQuestionDto> AddQuestionAsync(CreateSurveyQuestionDto dto, Guid teacherId);
        Task<SurveyQuestionDto> UpdateQuestionAsync(UpdateSurveyQuestionDto dto, Guid teacherId);
        Task DeleteQuestionAsync(Guid questionId, Guid teacherId);

        // ── Sonuçlar ───────────────────────────────────────────────────────────────
        Task<SurveyResultsDto> GetResultsAsync(Guid surveyId, Guid teacherId);
    }
}
