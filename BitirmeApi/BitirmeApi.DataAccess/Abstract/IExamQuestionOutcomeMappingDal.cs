using BitirmeApi.Core;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.DataAccess.Abstract
{
    public interface IExamQuestionOutcomeMappingDal : IRepository<ExamQuestionOutcomeMapping>
    {
        Task<List<ExamQuestionOutcomeMapping>> GetByQuestionIdWithDetailsAsync(Guid questionId);
        Task<ExamQuestionOutcomeMapping?> GetByIdWithDetailsAsync(Guid id);
        Task<ExamQuestionOutcomeMapping?> GetByIdWithOwnershipAsync(Guid id);
        Task<ExamQuestionOutcomeMapping?> GetByIdsAsync(Guid questionId, Guid cloId);
        Task<bool> ExistsAsync(Guid questionId, Guid cloId);
    }
}
