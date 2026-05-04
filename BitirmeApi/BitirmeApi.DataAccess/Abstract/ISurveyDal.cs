using BitirmeApi.Core;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.DataAccess.Abstract
{
    public interface ISurveyDal : IRepository<Survey>
    {
        Task<List<Survey>> GetByOfferingIdAsync(int externalCourseOfferingId);
        Task<Survey?> GetByIdWithDetailsAsync(Guid id);
        Task<List<Survey>> GetActiveByOfferingIdAsync(int externalCourseOfferingId);
        Task<Survey?> GetActiveByIdAsync(Guid id);
    }
}
