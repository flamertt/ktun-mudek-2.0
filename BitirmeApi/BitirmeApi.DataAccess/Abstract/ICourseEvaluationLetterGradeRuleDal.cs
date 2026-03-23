using BitirmeApi.Core;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.DataAccess.Abstract
{
    public interface ICourseEvaluationLetterGradeRuleDal : IRepository<CourseEvaluationLetterGradeRule>
    {
        Task<List<CourseEvaluationLetterGradeRule>> GetByEvaluationIdAsync(Guid evaluationId);
        Task<CourseEvaluationLetterGradeRule?> GetByIdWithOwnershipAsync(Guid id);
        Task<bool> ExistsLetterAsync(Guid evaluationId, string letterGrade, Guid? excludeId = null);
    }
}
