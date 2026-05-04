using BitirmeApi.Core.EntityFramework;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.DataAccess.Concrete.EntityFramework.Context;
using BitirmeApi.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitirmeApi.DataAccess.Concrete.EntityFramework
{
    public class AssessmentComponentOutcomeMappingDal : EfRepository<AssessmentComponentOutcomeMapping, ProjectDbContext>, IAssessmentComponentOutcomeMappingDal
    {
        public AssessmentComponentOutcomeMappingDal(ProjectDbContext context) : base(context) { }

        public async Task<List<AssessmentComponentOutcomeMapping>> GetByComponentIdWithDetailsAsync(Guid componentId) =>
            await _context.AssessmentComponentOutcomeMappings
                .Where(m => m.AssessmentComponentId == componentId)
                .AsNoTracking()
                .ToListAsync();

        public async Task<AssessmentComponentOutcomeMapping?> GetByIdWithOwnershipAsync(Guid id) =>
            await _context.AssessmentComponentOutcomeMappings
                .Where(m => m.Id == id)
                .Include(m => m.AssessmentComponent)
                    .ThenInclude(c => c.Exam)
                        .ThenInclude(e => e.CourseEvaluation)
                .AsNoTracking()
                .FirstOrDefaultAsync();

        public async Task<AssessmentComponentOutcomeMapping?> GetByComponentAndCloAsync(Guid componentId, int externalCloId) =>
            await _context.AssessmentComponentOutcomeMappings
                .FirstOrDefaultAsync(m => m.AssessmentComponentId == componentId && m.ExternalCloId == externalCloId);

        public async Task<bool> ExistsAsync(Guid componentId, int externalCloId) =>
            await _context.AssessmentComponentOutcomeMappings
                .AnyAsync(m => m.AssessmentComponentId == componentId && m.ExternalCloId == externalCloId);
    }
}
