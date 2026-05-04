using BitirmeApi.Core;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.DataAccess.Abstract
{
    public interface IStudentAssessmentComponentScoreDal : IRepository<StudentAssessmentComponentScore>
    {
        Task<List<StudentAssessmentComponentScore>> GetByComponentIdWithDetailsAsync(Guid componentId);
        Task<StudentAssessmentComponentScore?> GetByIdWithOwnershipAsync(Guid id);
        Task<StudentAssessmentComponentScore?> GetByComponentAndStudentAsync(Guid componentId, int externalStudentId);
        Task<bool> ExistsAsync(Guid componentId, int externalStudentId);
    }
}
