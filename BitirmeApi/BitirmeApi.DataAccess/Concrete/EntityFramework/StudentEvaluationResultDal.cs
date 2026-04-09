using BitirmeApi.Core.EntityFramework;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.DataAccess.Concrete.EntityFramework.Context;
using BitirmeApi.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitirmeApi.DataAccess.Concrete.EntityFramework
{
    public class StudentEvaluationResultDal : EfRepository<StudentEvaluationResult, ProjectDbContext>, IStudentEvaluationResultDal
    {
        public StudentEvaluationResultDal(ProjectDbContext context) : base(context) { }

        public async Task<HashSet<Guid>> GetPassingStudentIdsAsync(Guid courseOfferingId)
        {
            // IsPassed = true olan kayıtların Enrollment'ı üzerinden StudentId topla
            var passingEnrollmentIds = await _context.StudentEvaluationResults
                .Where(r => r.CourseOfferingId == courseOfferingId && r.IsPassed)
                .Select(r => r.EnrollmentId)
                .ToListAsync();

            if (passingEnrollmentIds.Count == 0)
                return new HashSet<Guid>();

            return await _context.Enrollments
                .Where(e => passingEnrollmentIds.Contains(e.Id))
                .Select(e => e.StudentId)
                .ToHashSetAsync();
        }
    }
}
