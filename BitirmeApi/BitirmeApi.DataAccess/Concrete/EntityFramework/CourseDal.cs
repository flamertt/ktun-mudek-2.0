using BitirmeApi.Core.EntityFramework;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.DataAccess.Concrete.EntityFramework.Context;
using BitirmeApi.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitirmeApi.DataAccess.Concrete.EntityFramework
{
    public class CourseDal : EfRepository<Course, ProjectDbContext>, ICourseDal
    {
        public CourseDal(ProjectDbContext context) : base(context) { }

        public async Task<List<Course>> GetAllWithDetailsAsync()
        {
            return await _context.Set<Course>()
                .Include(c => c.Program)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Course>> GetByProgramIdWithDetailsAsync(Guid programId)
        {
            return await _context.Set<Course>()
                .Where(c => c.ProgramEntityId == programId)
                .Include(c => c.Program)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Course?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.Set<Course>()
                .Where(c => c.Id == id)
                .Include(c => c.Program)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}
