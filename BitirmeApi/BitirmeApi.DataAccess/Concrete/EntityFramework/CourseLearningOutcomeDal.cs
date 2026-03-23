using BitirmeApi.Core.EntityFramework;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.DataAccess.Concrete.EntityFramework.Context;
using BitirmeApi.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitirmeApi.DataAccess.Concrete.EntityFramework
{
    public class CourseLearningOutcomeDal : EfRepository<CourseLearningOutcome, ProjectDbContext>, ICourseLearningOutcomeDal
    {
        public CourseLearningOutcomeDal(ProjectDbContext context) : base(context) { }

        public async Task<List<CourseLearningOutcome>> GetByCourseIdAsync(Guid courseId) =>
            await _context.Set<CourseLearningOutcome>()
                .Where(c => c.CourseId == courseId)
                .Include(c => c.Course)
                .Include(c => c.Maps)
                    .ThenInclude(m => m.PO)
                .OrderBy(c => c.OrderIndex)
                .AsNoTracking()
                .ToListAsync();

        public async Task<CourseLearningOutcome?> GetByIdWithDetailsAsync(Guid id) =>
            await _context.Set<CourseLearningOutcome>()
                .Where(c => c.Id == id)
                .Include(c => c.Course)
                .Include(c => c.Maps)
                    .ThenInclude(m => m.PO)
                .AsNoTracking()
                .FirstOrDefaultAsync();

        public async Task<bool> ExistsAsync(Guid id) =>
            await _context.Set<CourseLearningOutcome>().AnyAsync(c => c.Id == id);

        public async Task<bool> CodeExistsForCourseAsync(Guid courseId, string code, Guid? excludeId = null) =>
            await _context.Set<CourseLearningOutcome>().AnyAsync(c =>
                c.CourseId == courseId &&
                c.Code == code &&
                (excludeId == null || c.Id != excludeId));
    }
}
