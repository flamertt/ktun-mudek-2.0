using BitirmeApi.Core;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.DataAccess.Abstract
{
    public interface IAssessmentComponentDal : IRepository<AssessmentComponent>
    {
        Task<List<AssessmentComponent>> GetByExamIdWithDetailsAsync(Guid examId);
        Task<AssessmentComponent?> GetByIdWithDetailsAsync(Guid id);
        Task<AssessmentComponent?> GetByIdWithOwnershipAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
