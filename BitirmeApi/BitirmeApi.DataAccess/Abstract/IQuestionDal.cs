using BitirmeApi.Core;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.DataAccess.Abstract
{
    public interface IQuestionDal : IRepository<Question>
    {
        /// <summary>Ankete ait sorular, sıralı.</summary>
        Task<List<Question>> GetBySurveyIdAsync(Guid surveyId);

        /// <summary>Soru + Survey + CourseOffering (ownership zinciri).</summary>
        Task<Question?> GetByIdWithSurveyAsync(Guid id);
    }
}
