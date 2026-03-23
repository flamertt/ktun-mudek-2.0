using BitirmeApi.Core.EntityFramework;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.DataAccess.Concrete.EntityFramework.Context;
using BitirmeApi.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitirmeApi.DataAccess.Concrete.EntityFramework
{
    public class AssessmentComponentOutcomeMappingDal : EfRepository<AssessmentComponentOutcomeMapping, ProjectDbContext>, IAssessmentComponentOutcomeMappingDal
    {
        public AssessmentComponentOutcomeMappingDal(ProjectDbContext context) : base(context)
        {
        }

        public async Task<List<AssessmentComponentOutcomeMapping>> GetByComponentIdWithDetailsAsync(Guid componentId) =>
            await _context.AssessmentComponentOutcomeMappings
                .Where(m => m.AssessmentComponentId == componentId)
                .Include(m => m.CourseLearningOutcome)
                .AsNoTracking()
                .ToListAsync();

        public async Task<AssessmentComponentOutcomeMapping?> GetByIdWithOwnershipAsync(Guid id) =>
            await _context.AssessmentComponentOutcomeMappings
                .Where(m => m.Id == id)
                .Include(m => m.AssessmentComponent)
                    .ThenInclude(c => c.Exam)
                        .ThenInclude(e => e.CourseEvaluation)
                            .ThenInclude(ev => ev.CourseOffering)
                .AsNoTracking()
                .FirstOrDefaultAsync();

        public async Task<AssessmentComponentOutcomeMapping?> GetByComponentAndCloAsync(Guid componentId, Guid cloId) =>
            await _context.AssessmentComponentOutcomeMappings
                .FirstOrDefaultAsync(m => m.AssessmentComponentId == componentId && m.CourseLearningOutcomeId == cloId);

        public async Task<bool> ExistsAsync(Guid componentId, Guid cloId) =>
            await _context.AssessmentComponentOutcomeMappings
                .AnyAsync(m => m.AssessmentComponentId == componentId && m.CourseLearningOutcomeId == cloId);
    }
}
