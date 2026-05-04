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
                .AsNoTracking();

        public async Task<List<CourseEvaluation>> GetAllWithDetailsAsync() =>
            await WithDetails().ToListAsync();

        public async Task<CourseEvaluation?> GetByIdWithDetailsAsync(Guid id) =>
            await WithDetails().FirstOrDefaultAsync(e => e.Id == id);

        public async Task<CourseEvaluation?> GetByOfferingIdAsync(int externalCourseOfferingId) =>
            await _context.CourseEvaluations
                .FirstOrDefaultAsync(e => e.ExternalCourseOfferingId == externalCourseOfferingId);

        public async Task<List<CourseEvaluation>> GetByTeacherIdAsync(int externalTeacherId) =>
            await _context.CourseEvaluations
                .Where(e => e.ExternalTeacherId == externalTeacherId)
                .AsNoTracking()
                .ToListAsync();
    }
}
