using BitirmeApi.Core;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.DataAccess.Abstract
{
    public interface IStudentAssessmentComponentScoreDal : IRepository<StudentAssessmentComponentScore>
    {
        Task<List<StudentAssessmentComponentScore>> GetByComponentIdWithDetailsAsync(Guid componentId);
        Task<StudentAssessmentComponentScore?> GetByIdWithOwnershipAsync(Guid id);
        Task<StudentAssessmentComponentScore?> GetByComponentAndEnrollmentAsync(Guid componentId, Guid enrollmentId);
        Task<bool> ExistsAsync(Guid componentId, Guid enrollmentId);
    }
}
