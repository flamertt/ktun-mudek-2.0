using BitirmeApi.Core.EntityFramework;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.DataAccess.Concrete.EntityFramework.Context;
using BitirmeApi.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitirmeApi.DataAccess.Concrete.EntityFramework
{
    public class AcademicTermDal : EfRepository<AcademicTerm, ProjectDbContext>, IAcademicTermDal
    {
        public AcademicTermDal(ProjectDbContext context) : base(context) { }

        public async Task<AcademicTerm?> GetActiveAsync() =>
            await _context.AcademicTerms
                .OrderByDescending(t => t.Id)
                .FirstOrDefaultAsync();
    }
}
