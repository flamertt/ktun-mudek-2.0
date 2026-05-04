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

        public async Task<HashSet<int>> GetPassingStudentIdsAsync(int externalCourseOfferingId) =>
            await _context.StudentEvaluationResults
                .Where(r => r.ExternalCourseOfferingId == externalCourseOfferingId && r.IsPassed)
                .Select(r => r.ExternalStudentId)
                .ToHashSetAsync();
    }
}
