using BitirmeApi.Core;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.DataAccess.Abstract
{
    public interface IProgramLetterGradeRuleDal : IRepository<ProgramLetterGradeRule>
    {
        Task<List<ProgramLetterGradeRule>> GetByProgramIdAsync(Guid programEntityId);
        Task<bool> ExistsLetterAsync(Guid programEntityId, string letterGrade, Guid? excludeId = null);
    }
}
