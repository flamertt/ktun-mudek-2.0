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

        public async Task<List<Survey>> GetByOfferingIdAsync(int externalCourseOfferingId) =>
            await _context.Surveys
                .Where(s => s.ExternalCourseOfferingId == externalCourseOfferingId)
                .Include(s => s.Questions)
                .Include(s => s.Submissions)
                .OrderByDescending(s => s.CreatedAt)
                .AsNoTracking()
                .ToListAsync();

        public async Task<Survey?> GetByIdWithDetailsAsync(Guid id) =>
            await _context.Surveys
                .Where(s => s.Id == id)
                .Include(s => s.Questions.OrderBy(q => q.OrderIndex))
                .Include(s => s.Submissions)
                .AsNoTracking()
                .FirstOrDefaultAsync();

        public async Task<List<Survey>> GetActiveByOfferingIdAsync(int externalCourseOfferingId) =>
            await _context.Surveys
                .Where(s => s.ExternalCourseOfferingId == externalCourseOfferingId && s.IsActive)
                .Include(s => s.Questions)
                .OrderByDescending(s => s.CreatedAt)
                .AsNoTracking()
                .ToListAsync();

        public async Task<Survey?> GetActiveByIdAsync(Guid id) =>
            await _context.Surveys
                .Where(s => s.Id == id && s.IsActive)
                .Include(s => s.Questions.OrderBy(q => q.OrderIndex))
                .AsNoTracking()
                .FirstOrDefaultAsync();
    }
}
