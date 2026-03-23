using BitirmeApi.Core.EntityFramework;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.DataAccess.Concrete.EntityFramework.Context;
using BitirmeApi.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitirmeApi.DataAccess.Concrete.EntityFramework
{
    public class CourseOfferingDal : EfRepository<CourseOffering, ProjectDbContext>, ICourseOfferingDal
    {
        public CourseOfferingDal(ProjectDbContext context) : base(context) { }

        private IQueryable<CourseOffering> WithDetails() =>
            _context.CourseOfferings
                .Include(o => o.Course)
                    .ThenInclude(c => c.Program)
                .Include(o => o.AcademicTerm)
                .Include(o => o.Teacher)
                .Include(o => o.Enrollments)
                .AsNoTracking();

        public async Task<List<CourseOffering>> GetAllWithDetailsAsync() =>
            await WithDetails().ToListAsync();

        public async Task<List<CourseOffering>> GetByTermIdWithDetailsAsync(Guid termId) =>
            await WithDetails().Where(o => o.AcademicTermId == termId).ToListAsync();

        public async Task<List<CourseOffering>> GetByActiveTermWithDetailsAsync() =>
            await WithDetails().Where(o => o.AcademicTerm.IsActive).ToListAsync();

        public async Task<List<CourseOffering>> GetByTeacherIdWithDetailsAsync(Guid teacherId) =>
            await WithDetails().Where(o => o.TeacherId == teacherId).ToListAsync();

        public async Task<List<CourseOffering>> GetByTeacherIdAndTermAsync(Guid teacherId, Guid? termId = null)
        {
            var q = WithDetails().Where(o => o.TeacherId == teacherId);
            if (termId.HasValue)
                q = q.Where(o => o.AcademicTermId == termId.Value);
            return await q.ToListAsync();
        }

        public async Task<List<CourseOffering>> GetByCourseIdWithDetailsAsync(Guid courseId) =>
            await WithDetails().Where(o => o.CourseId == courseId).ToListAsync();

        public async Task<CourseOffering?> GetByIdWithDetailsAsync(Guid id) =>
            await WithDetails().FirstOrDefaultAsync(o => o.Id == id);

        public async Task<CourseOffering?> GetByIdAndTeacherIdWithDetailsAsync(Guid offeringId, Guid teacherId) =>
            await WithDetails().FirstOrDefaultAsync(o => o.Id == offeringId && o.TeacherId == teacherId);

        public async Task<bool> ExistsAsync(Guid id) =>
            await _context.CourseOfferings.AnyAsync(o => o.Id == id);
    }
}
