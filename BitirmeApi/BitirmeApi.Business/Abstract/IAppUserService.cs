using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    public interface IAppUserService
    {
        // ── CRUD ──────────────────────────────────────────────────────────────────
        Task<List<AppUserListDto>> GetAllAsync();
        Task<AppUserDto?> GetByIdAsync(Guid id);
        Task<AppUserDto?> GetByEmailAsync(string email);
        Task<AppUserDto?> GetByStudentNumberAsync(string studentNumber);
        Task<List<AppUserListDto>> GetByRoleAsync(string role);
        Task<List<AppUserListDto>> GetStudentsByProgramIdAsync(Guid programId);
        Task<List<AppUserListDto>> GetTeachersByProgramIdAsync(Guid programId);
        Task<AppUserDto> AddAsync(CreateAppUserDto createDto);
        Task<AppUserDto> UpdateAsync(UpdateAppUserDto updateDto);
        Task DeleteAsync(Guid id);

        // ── Authentication ────────────────────────────────────────────────────────
        Task<AppUserDto?> ValidateUserAsync(string email, string password);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> ChangePasswordAsync(Guid userId, string oldPassword, string newPassword);
        Task UpdateLastLoginAsync(Guid userId);

        // ── Role-based lists ──────────────────────────────────────────────────────
        Task<List<AppUserListDto>> GetStudentsAsync();
        Task<List<AppUserListDto>> GetTeachersAsync();
        Task<List<AppUserListDto>> GetAdminsAsync();

        // ── Validation helpers (Business katmanı doğrulamaları için) ─────────────
        Task<bool> ExistsAsync(Guid id);
        Task<bool> IsTeacherAsync(Guid id);
        Task<bool> IsStudentAsync(Guid id);
        Task<bool> IsActiveAsync(Guid id);
    }
}
