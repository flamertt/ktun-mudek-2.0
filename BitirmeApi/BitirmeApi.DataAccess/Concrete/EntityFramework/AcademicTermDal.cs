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

        public async Task<AcademicTerm?> GetActiveTermAsync()
        {
            return await _context.Set<AcademicTerm>()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.IsActive);
        }

        public async Task DeactivateAllAsync()
        {
            await _context.Set<AcademicTerm>()
                .Where(t => t.IsActive)
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.IsActive, false));
        }
    }
}
