using BitirmeApi.Core;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.DataAccess.Abstract
{
    public interface ICloEvaluationResultDal : IRepository<CloEvaluationResult>
    {
        Task<List<CloEvaluationResult>> GetCombinedByOfferingAsync(int externalCourseOfferingId);
    }
}
