using BitirmeApi.Core;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.DataAccess.Abstract
{
    public interface ILetterGradeRuleDal : IRepository<LetterGradeRule>
    {
        Task<List<LetterGradeRule>> GetByProgramIdAsync(int externalProgramId);
        Task<bool> ExistsLetterAsync(int externalProgramId, string letterGrade, Guid? excludeId = null);
    }
}
