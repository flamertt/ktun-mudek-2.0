using BitirmeApi.Core.EntityFramework;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.DataAccess.Concrete.EntityFramework.Context;
using BitirmeApi.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitirmeApi.DataAccess.Concrete.EntityFramework
{
    public class AssessmentComponentDal : EfRepository<AssessmentComponent, ProjectDbContext>, IAssessmentComponentDal
    {
        public AssessmentComponentDal(ProjectDbContext context) : base(context)
        {
        }

        public async Task<List<AssessmentComponent>> GetByExamIdWithDetailsAsync(Guid examId) =>
            await _context.AssessmentComponents
                .Where(c => c.ExamId == examId)
                .Include(c => c.OutcomeMappings)
                    .ThenInclude(m => m.CourseLearningOutcome)
                .AsNoTracking()
                .OrderBy(c => c.OrderIndex)
                .ToListAsync();

        public async Task<AssessmentComponent?> GetByIdWithDetailsAsync(Guid id) =>
            await _context.AssessmentComponents
                .Where(c => c.Id == id)
                .Include(c => c.OutcomeMappings)
                    .ThenInclude(m => m.CourseLearningOutcome)
                .AsNoTracking()
                .FirstOrDefaultAsync();

        public async Task<AssessmentComponent?> GetByIdWithOwnershipAsync(Guid id) =>
            await _context.AssessmentComponents
                .Where(c => c.Id == id)
                .Include(c => c.Exam)
                    .ThenInclude(e => e.CourseEvaluation)
                        .ThenInclude(ev => ev.CourseOffering)
                .AsNoTracking()
                .FirstOrDefaultAsync();

        public async Task<bool> ExistsAsync(Guid id) =>
            await _context.AssessmentComponents.AnyAsync(c => c.Id == id);
    }
}
