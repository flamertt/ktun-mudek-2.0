using BitirmeApi.Core.EntityFramework;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.DataAccess.Concrete.EntityFramework.Context;
using BitirmeApi.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitirmeApi.DataAccess.Concrete.EntityFramework
{
    public class AppUserDal : EfRepository<AppUser, ProjectDbContext>, IAppUserDal
    {
        public AppUserDal(ProjectDbContext context) : base(context) { }

        public async Task<AppUser?> GetByIdWithProgramAsync(Guid id) =>
            await _context.Users
                .Where(u => u.Id == id)
                .Include(u => u.Program)
                .AsNoTracking()
                .FirstOrDefaultAsync();

        public async Task<List<AppUser>> GetByRoleWithProgramAsync(string role) =>
            await _context.Users
                .Where(u => u.Role == role && u.IsActive)
                .Include(u => u.Program)
                .AsNoTracking()
                .ToListAsync();

        public async Task<List<AppUser>> GetStudentsByProgramAsync(Guid programId) =>
            await _context.Users
                .Where(u => u.ProgramEntityId == programId && u.Role == "Student" && u.IsActive)
                .Include(u => u.Program)
                .AsNoTracking()
                .ToListAsync();

        public async Task<List<AppUser>> GetTeachersByProgramAsync(Guid programId) =>
            await _context.Users
                .Where(u => u.ProgramEntityId == programId && u.Role == "Teacher" && u.IsActive)
                .Include(u => u.Program)
                .AsNoTracking()
                .ToListAsync();

        // ── Validation helpers (tek sütun sorgular — verimli) ────────────────────

        public async Task<bool> ExistsAsync(Guid id) =>
            await _context.Users.AnyAsync(u => u.Id == id);

        public async Task<bool> IsInRoleAsync(Guid id, string role) =>
            await _context.Users.AnyAsync(u => u.Id == id && u.Role == role);

        public async Task<bool> IsActiveAsync(Guid id) =>
            await _context.Users.AnyAsync(u => u.Id == id && u.IsActive);

        public async Task<AppUser?> GetByStudentNumberAsync(string studentNumber) =>
            await _context.Users
                .Where(u => u.StudentNumber == studentNumber)
                .AsNoTracking()
                .FirstOrDefaultAsync();
    }
}
