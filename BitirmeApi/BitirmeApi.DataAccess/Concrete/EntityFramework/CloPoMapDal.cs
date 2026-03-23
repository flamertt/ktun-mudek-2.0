using BitirmeApi.Core.EntityFramework;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.DataAccess.Concrete.EntityFramework.Context;
using BitirmeApi.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitirmeApi.DataAccess.Concrete.EntityFramework
{
    public class CloPoMapDal : EfRepository<CloPoMap, ProjectDbContext>, ICloPoMapDal
    {
        public CloPoMapDal(ProjectDbContext context) : base(context) { }

        public async Task<List<CloPoMap>> GetByCloIdAsync(Guid cloId) =>
            await _context.Set<CloPoMap>()
                .Where(m => m.CourseLearningOutcomeId == cloId)
                .Include(m => m.CLO)
                .Include(m => m.PO)
                .AsNoTracking()
                .ToListAsync();

        public async Task<List<CloPoMap>> GetByCourseIdAsync(Guid courseId) =>
            await _context.Set<CloPoMap>()
                .Where(m => m.CLO.CourseId == courseId)
                .Include(m => m.CLO)
                .Include(m => m.PO)
                .AsNoTracking()
                .ToListAsync();

        public async Task<bool> ExistsAsync(Guid cloId, Guid programOutcomeId) =>
            await _context.Set<CloPoMap>().AnyAsync(m =>
                m.CourseLearningOutcomeId == cloId && m.ProgramOutcomeId == programOutcomeId);

        public async Task<CloPoMap?> GetByIdsAsync(Guid cloId, Guid programOutcomeId) =>
            await _context.Set<CloPoMap>().FirstOrDefaultAsync(m =>
                m.CourseLearningOutcomeId == cloId && m.ProgramOutcomeId == programOutcomeId);
    }
}
