using BitirmeApi.Core;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.DataAccess.Abstract
{
    public interface IStudentAnswerDal : IRepository<StudentAnswer>
    {
        Task<List<StudentAnswer>> GetByQuestionIdWithDetailsAsync(Guid questionId);
        Task<StudentAnswer?> GetByIdWithOwnershipAsync(Guid id);
        Task<StudentAnswer?> GetByQuestionAndStudentAsync(Guid questionId, int externalStudentId);
        Task<bool> ExistsAsync(Guid questionId, int externalStudentId);
    }
}
