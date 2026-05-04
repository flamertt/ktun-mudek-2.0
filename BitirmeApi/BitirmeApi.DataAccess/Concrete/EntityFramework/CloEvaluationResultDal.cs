using BitirmeApi.Core.EntityFramework;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.DataAccess.Concrete.EntityFramework.Context;
using BitirmeApi.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitirmeApi.DataAccess.Concrete.EntityFramework
{
    public class CloEvaluationResultDal : EfRepository<CloEvaluationResult, ProjectDbContext>, ICloEvaluationResultDal
    {
        public CloEvaluationResultDal(ProjectDbContext context) : base(context) { }

        public async Task<List<CloEvaluationResult>> GetCombinedByOfferingAsync(int externalCourseOfferingId) =>
            await _context.CloEvaluationResults
                .Where(c => c.ExternalCourseOfferingId == externalCourseOfferingId
                         && c.ResultType == CloEvaluationResultType.Combined)
                .ToListAsync();
    }
}
