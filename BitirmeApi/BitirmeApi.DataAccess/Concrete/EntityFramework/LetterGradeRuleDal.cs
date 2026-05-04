using BitirmeApi.Core.EntityFramework;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.DataAccess.Concrete.EntityFramework.Context;
using BitirmeApi.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitirmeApi.DataAccess.Concrete.EntityFramework
{
    public class LetterGradeRuleDal : EfRepository<LetterGradeRule, ProjectDbContext>, ILetterGradeRuleDal
    {
        public LetterGradeRuleDal(ProjectDbContext context) : base(context) { }

        public async Task<List<LetterGradeRule>> GetByProgramIdAsync(int externalProgramId) =>
            await _context.LetterGradeRules
                .AsNoTracking()
                .Where(r => r.ExternalProgramId == externalProgramId)
                .OrderByDescending(r => r.MaxScore)
                .ToListAsync();

        public async Task<bool> ExistsLetterAsync(int externalProgramId, string letterGrade, Guid? excludeId = null)
        {
            var q = _context.LetterGradeRules.Where(r =>
                r.ExternalProgramId == externalProgramId &&
                r.LetterGrade == letterGrade);
            if (excludeId.HasValue)
                q = q.Where(r => r.Id != excludeId.Value);
            return await q.AnyAsync();
        }
    }
}
