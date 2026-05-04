using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    public interface ISurveyService
    {
        Task<List<SurveyListDto>> GetByOfferingIdAsync(int externalCourseOfferingId, int externalTeacherId);
        Task<SurveyDetailDto?> GetByIdAsync(Guid id, int externalTeacherId);
        Task<SurveyDetailDto> CreateAsync(CreateSurveyDto dto, int externalTeacherId);
        Task<SurveyDetailDto> UpdateAsync(UpdateSurveyDto dto, int externalTeacherId);
        Task DeleteAsync(Guid id, int externalTeacherId);
        Task ToggleActiveAsync(Guid id, int externalTeacherId);

        Task<SurveyQuestionDto> AddQuestionAsync(CreateSurveyQuestionDto dto, int externalTeacherId);
        Task<SurveyQuestionDto> UpdateQuestionAsync(UpdateSurveyQuestionDto dto, int externalTeacherId);
        Task DeleteQuestionAsync(Guid questionId, int externalTeacherId);

        Task<SurveyResultsDto> GetResultsAsync(Guid surveyId, int externalTeacherId, string universityToken);
    }
}
