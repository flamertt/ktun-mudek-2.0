using BitirmeApi.Core.EntityFramework;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.DataAccess.Concrete.EntityFramework.Context;
using BitirmeApi.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitirmeApi.DataAccess.Concrete.EntityFramework
{
    public class ProgramEntityDal : EfRepository<ProgramEntity, ProjectDbContext>, IProgramEntityDal
    {
        public ProgramEntityDal(ProjectDbContext context) : base(context)
        {
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Set<ProgramEntity>().AnyAsync(p => p.Id == id);
        }
    }
}
