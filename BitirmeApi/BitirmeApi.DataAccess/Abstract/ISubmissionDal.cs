using BitirmeApi.Core;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.DataAccess.Abstract
{
    public interface ISubmissionDal : IRepository<Submission>
    {
        /// <summary>Ankete ait gönderimler ve yanıtlar (sonuç hesaplaması için).</summary>
        Task<List<Submission>> GetBySurveyIdWithAnswersAsync(Guid surveyId);

        /// <summary>Ankete yapılan toplam gönderim sayısı.</summary>
        Task<int> CountBySurveyIdAsync(Guid surveyId);

        /// <summary>Öğrencinin belirtilen ankete daha önce katılıp katılmadığını kontrol eder.</summary>
        Task<bool> HasStudentSubmittedAsync(Guid surveyId, Guid userId);
    }
}
