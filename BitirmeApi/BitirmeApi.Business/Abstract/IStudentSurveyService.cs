using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    public interface IStudentSurveyService
    {
        /// <summary>
        /// Öğrencinin belirtilen aktif dönemdeki derslerini döner.
        /// universityToken ile üniversite API'si çağrılır.
        /// </summary>
        Task<List<StudentCourseDto>> GetActiveTermCoursesAsync(int externalStudentId, int academicTermId, string universityToken);

        /// <summary>
        /// Belirtilen ders açılışına ait aktif anketleri döner.
        /// Öğrencinin o derse kaydı üniversite API'si ile doğrulanır.
        /// </summary>
        Task<List<StudentSurveyListDto>> GetActiveSurveysAsync(int externalCourseOfferingId, int externalStudentId, string universityToken);

        /// <summary>
        /// Anketi sorularıyla birlikte döner (doldurmak için).
        /// </summary>
        Task<StudentSurveyDetailDto> GetSurveyDetailAsync(Guid surveyId, int externalStudentId, string universityToken);

        /// <summary>
        /// Öğrencinin anket cevaplarını kaydeder. Daha önce katılmışsa hata fırlatır.
        /// </summary>
        Task<StudentSubmissionResultDto> SubmitAsync(Guid surveyId, int externalStudentId, string universityToken, SubmitSurveyDto dto);
    }
}
