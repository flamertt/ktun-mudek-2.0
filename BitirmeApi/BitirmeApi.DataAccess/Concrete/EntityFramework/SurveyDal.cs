using BitirmeApi.Core.EntityFramework;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.DataAccess.Concrete.EntityFramework.Context;
using BitirmeApi.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitirmeApi.DataAccess.Concrete.EntityFramework
{
    public class SurveyDal : EfRepository<Survey, ProjectDbContext>, ISurveyDal
    {
        public SurveyDal(ProjectDbContext context) : base(context) { }

        public async Task<List<Survey>> GetByOfferingIdAsync(Guid offeringId) =>
            await _context.Surveys
                .Where(s => s.CourseOfferingId == offeringId)
                .Include(s => s.Questions)
                .Include(s => s.Submissions)
                .OrderByDescending(s => s.CreatedAt)
                .AsNoTracking()
                .ToListAsync();

        public async Task<Survey?> GetByIdWithDetailsAsync(Guid id) =>
            await _context.Surveys
                .Where(s => s.Id == id)
                .Include(s => s.Questions.OrderBy(q => q.OrderIndex))
                    .ThenInclude(q => q.CourseLearningOutcome)
                .Include(s => s.Submissions)
                .Include(s => s.CourseOffering)
                .AsNoTracking()
                .FirstOrDefaultAsync();

        public async Task<List<Survey>> GetActiveByOfferingIdAsync(Guid offeringId) =>
            await _context.Surveys
                .Where(s => s.CourseOfferingId == offeringId && s.IsActive)
                .Include(s => s.Questions)
                .OrderByDescending(s => s.CreatedAt)
                .AsNoTracking()
                .ToListAsync();

        public async Task<Survey?> GetActiveByIdAsync(Guid id) =>
            await _context.Surveys
                .Where(s => s.Id == id && s.IsActive)
                .Include(s => s.Questions.OrderBy(q => q.OrderIndex))
                .Include(s => s.CourseOffering)
                .AsNoTracking()
                .FirstOrDefaultAsync();
    }
}
