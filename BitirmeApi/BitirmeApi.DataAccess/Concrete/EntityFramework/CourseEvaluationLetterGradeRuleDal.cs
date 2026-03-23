using BitirmeApi.Core.EntityFramework;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.DataAccess.Concrete.EntityFramework.Context;
using BitirmeApi.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitirmeApi.DataAccess.Concrete.EntityFramework
{
    public class CourseEvaluationLetterGradeRuleDal : EfRepository<CourseEvaluationLetterGradeRule, ProjectDbContext>, ICourseEvaluationLetterGradeRuleDal
    {
        public CourseEvaluationLetterGradeRuleDal(ProjectDbContext context) : base(context)
        {
        }

        public async Task<List<CourseEvaluationLetterGradeRule>> GetByEvaluationIdAsync(Guid evaluationId) =>
            await _context.CourseEvaluationLetterGradeRules
                .Where(r => r.CourseEvaluationId == evaluationId)
                .OrderByDescending(r => r.MinScore)
                .AsNoTracking()
                .ToListAsync();

        public async Task<CourseEvaluationLetterGradeRule?> GetByIdWithOwnershipAsync(Guid id) =>
            await _context.CourseEvaluationLetterGradeRules
                .Where(r => r.Id == id)
                .Include(r => r.CourseEvaluation)
                    .ThenInclude(e => e.CourseOffering)
                .AsNoTracking()
                .FirstOrDefaultAsync();

        public async Task<bool> ExistsLetterAsync(Guid evaluationId, string letterGrade, Guid? excludeId = null) =>
            await _context.CourseEvaluationLetterGradeRules.AnyAsync(r =>
                r.CourseEvaluationId == evaluationId &&
                r.LetterGrade == letterGrade &&
                (excludeId == null || r.Id != excludeId));
    }
}
