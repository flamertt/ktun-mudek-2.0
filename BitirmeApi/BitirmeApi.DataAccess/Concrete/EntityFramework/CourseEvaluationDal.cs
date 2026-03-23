using BitirmeApi.Core.EntityFramework;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.DataAccess.Concrete.EntityFramework.Context;
using BitirmeApi.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitirmeApi.DataAccess.Concrete.EntityFramework
{
    public class CourseEvaluationDal : EfRepository<CourseEvaluation, ProjectDbContext>, ICourseEvaluationDal
    {
        public CourseEvaluationDal(ProjectDbContext context) : base(context) { }

        private IQueryable<CourseEvaluation> WithDetails() =>
            _context.CourseEvaluations
                .Include(e => e.CourseOffering)
                    .ThenInclude(o => o.Course)
                        .ThenInclude(c => c.Program)
                .Include(e => e.CourseOffering)
                    .ThenInclude(o => o.AcademicTerm)
                .Include(e => e.CourseOffering)
                    .ThenInclude(o => o.Teacher)
                .Include(e => e.CourseOffering)
                    .ThenInclude(o => o.Enrollments)
                .AsNoTracking();

        public async Task<List<CourseEvaluation>> GetAllWithDetailsAsync() =>
            await WithDetails().ToListAsync();

        public async Task<CourseEvaluation?> GetByIdWithDetailsAsync(Guid id) =>
            await WithDetails().FirstOrDefaultAsync(e => e.Id == id);

        public async Task<CourseEvaluation?> GetByOfferingIdWithDetailsAsync(Guid courseOfferingId) =>
            await WithDetails().FirstOrDefaultAsync(e => e.CourseOfferingId == courseOfferingId);
    }
}
