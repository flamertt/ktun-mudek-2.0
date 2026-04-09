using BitirmeApi.Core.EntityFramework;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.DataAccess.Concrete.EntityFramework.Context;
using BitirmeApi.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitirmeApi.DataAccess.Concrete.EntityFramework
{
    public class ProgramLetterGradeRuleDal : EfRepository<ProgramLetterGradeRule, ProjectDbContext>, IProgramLetterGradeRuleDal
    {
        public ProgramLetterGradeRuleDal(ProjectDbContext context) : base(context) { }

        public async Task<List<ProgramLetterGradeRule>> GetByProgramIdAsync(Guid programEntityId) =>
            await _context.ProgramLetterGradeRules
                .Where(r => r.ProgramEntityId == programEntityId)
                .OrderByDescending(r => r.MaxScore)
                .AsNoTracking()
                .ToListAsync();

        public async Task<bool> ExistsLetterAsync(Guid programEntityId, string letterGrade, Guid? excludeId = null) =>
            await _context.ProgramLetterGradeRules.AnyAsync(r =>
                r.ProgramEntityId == programEntityId &&
                r.LetterGrade == letterGrade &&
                (excludeId == null || r.Id != excludeId));
    }
}
