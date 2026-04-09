using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    public interface IStudentSurveyService
    {
        /// <summary>
        /// Öğrencinin aktif akademik dönemde kayıtlı olduğu dersleri döner.
        /// Her derste kaç aktif anket olduğunu da gösterir.
        /// </summary>
        Task<List<StudentCourseDto>> GetActiveTermCoursesAsync(Guid studentId);

        /// <summary>
        /// Belirtilen ders açılışına ait aktif anketleri döner.
        /// Öğrencinin o derse kaydı ve aktif dönemde olması zorunludur.
        /// </summary>
        Task<List<StudentSurveyListDto>> GetActiveSurveysAsync(Guid offeringId, Guid studentId);

        /// <summary>
        /// Anketi sorularıyla birlikte döner (doldurmak için).
        /// Öğrencinin o derse kaydı ve aktif dönemde olması zorunludur.
        /// </summary>
        Task<StudentSurveyDetailDto> GetSurveyDetailAsync(Guid surveyId, Guid studentId);

        /// <summary>
        /// Öğrencinin anket cevaplarını kaydeder. Daha önce katılmışsa hata fırlatır.
        /// </summary>
        Task<StudentSubmissionResultDto> SubmitAsync(Guid surveyId, Guid studentId, SubmitSurveyDto dto);
    }
}
