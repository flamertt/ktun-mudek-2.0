using BitirmeApi.Core;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.DataAccess.Abstract
{
    public interface IAppUserDal : IRepository<AppUser>
    {
        Task<AppUser?> GetByIdWithProgramAsync(Guid id);
        Task<List<AppUser>> GetByRoleWithProgramAsync(string role);
        Task<List<AppUser>> GetStudentsByProgramAsync(Guid programId);
        Task<List<AppUser>> GetTeachersByProgramAsync(Guid programId);

        // ── Validation helpers ────────────────────────────────────────────────────
        Task<bool> ExistsAsync(Guid id);
        Task<bool> IsInRoleAsync(Guid id, string role);
        Task<bool> IsActiveAsync(Guid id);
        Task<AppUser?> GetByStudentNumberAsync(string studentNumber);
    }
}
