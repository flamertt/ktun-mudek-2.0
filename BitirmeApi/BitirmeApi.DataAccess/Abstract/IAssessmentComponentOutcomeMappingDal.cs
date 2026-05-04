using BitirmeApi.Core;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.DataAccess.Abstract
{
    public interface IAssessmentComponentOutcomeMappingDal : IRepository<AssessmentComponentOutcomeMapping>
    {
        Task<List<AssessmentComponentOutcomeMapping>> GetByComponentIdWithDetailsAsync(Guid componentId);
        Task<AssessmentComponentOutcomeMapping?> GetByIdWithOwnershipAsync(Guid id);
        Task<AssessmentComponentOutcomeMapping?> GetByComponentAndCloAsync(Guid componentId, int externalCloId);
        Task<bool> ExistsAsync(Guid componentId, int externalCloId);
    }
}
