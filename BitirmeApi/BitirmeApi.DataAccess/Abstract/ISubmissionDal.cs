using BitirmeApi.Core;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.DataAccess.Abstract
{
    public interface ISubmissionDal : IRepository<Submission>
    {
        Task<List<Submission>> GetBySurveyIdWithAnswersAsync(Guid surveyId);
        Task<int> CountBySurveyIdAsync(Guid surveyId);
        Task<bool> HasStudentSubmittedByExternalIdAsync(Guid surveyId, int externalStudentId);
    }
}
