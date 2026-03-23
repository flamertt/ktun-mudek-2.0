using AutoMapper;
using BitirmeApi.Business.Abstract;
using BitirmeApi.Business.DTO;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.Entity.Constants;
using BitirmeApi.Entity.Entities;
using System.Security.Cryptography;
using System.Text;

namespace BitirmeApi.Business.Concrete
{
    public class AppUserService : IAppUserService
    {
        private readonly IAppUserDal _userDal;
        private readonly IMapper _mapper;

        public AppUserService(IAppUserDal userDal, IMapper mapper)
        {
            _userDal = userDal;
            _mapper = mapper;
        }

        // ── CRUD ──────────────────────────────────────────────────────────────────

        public async Task<List<AppUserListDto>> GetAllAsync()
        {
            var entities = await _userDal.GetListAsync();
            return _mapper.Map<List<AppUserListDto>>(entities.ToList());
        }

        public async Task<AppUserDto?> GetByIdAsync(Guid id)
        {
            var entity = await _userDal.GetByIdWithProgramAsync(id);
            return entity != null ? _mapper.Map<AppUserDto>(entity) : null;
        }

        public async Task<AppUserDto?> GetByEmailAsync(string email)
        {
            var entity = await _userDal.GetAsync(u => u.Email.ToLower() == email.ToLower());
            return entity != null ? _mapper.Map<AppUserDto>(entity) : null;
        }

        public async Task<AppUserDto?> GetByStudentNumberAsync(string studentNumber)
        {
            var entity = await _userDal.GetByStudentNumberAsync(studentNumber);
            return entity != null ? _mapper.Map<AppUserDto>(entity) : null;
        }

        public async Task<List<AppUserListDto>> GetByRoleAsync(string role)
        {
            var entities = await _userDal.GetByRoleWithProgramAsync(role);
            return _mapper.Map<List<AppUserListDto>>(entities);
        }

        public async Task<List<AppUserListDto>> GetStudentsByProgramIdAsync(Guid programId)
        {
            var entities = await _userDal.GetStudentsByProgramAsync(programId);
            return _mapper.Map<List<AppUserListDto>>(entities);
        }

        public async Task<List<AppUserListDto>> GetTeachersByProgramIdAsync(Guid programId)
        {
            var entities = await _userDal.GetTeachersByProgramAsync(programId);
            return _mapper.Map<List<AppUserListDto>>(entities);
        }

        public async Task<AppUserDto> AddAsync(CreateAppUserDto createDto)
        {
            if (await _userDal.GetAsync(u => u.Email.ToLower() == createDto.Email.ToLower()) != null)
                throw new InvalidOperationException($"'{createDto.Email}' e-posta adresi zaten kullanımda.");

            var entity = _mapper.Map<AppUser>(createDto);
            entity.PasswordHash = HashPassword(createDto.Password);
            entity.CreatedAt = DateTime.UtcNow;
            entity.IsActive = true;

            var added = _userDal.Add(entity);
            await _userDal.SaveChangesAsync();
            return _mapper.Map<AppUserDto>(added);
        }

        public async Task<AppUserDto> UpdateAsync(UpdateAppUserDto updateDto)
        {
            var existing = await _userDal.GetAsync(u => u.Id == updateDto.Id)
                ?? throw new KeyNotFoundException($"Kullanıcı bulunamadı: {updateDto.Id}");

            if (!string.IsNullOrEmpty(updateDto.Email) &&
                updateDto.Email.ToLower() != existing.Email.ToLower())
            {
                if (await _userDal.GetAsync(u => u.Email.ToLower() == updateDto.Email.ToLower() && u.Id != updateDto.Id) != null)
                    throw new InvalidOperationException($"'{updateDto.Email}' e-posta adresi zaten kullanımda.");
            }

            _mapper.Map(updateDto, existing);

            if (!string.IsNullOrEmpty(updateDto.Password))
                existing.PasswordHash = HashPassword(updateDto.Password);

            existing.UpdatedAt = DateTime.UtcNow;
            var updated = _userDal.Update(existing);
            await _userDal.SaveChangesAsync();
            return _mapper.Map<AppUserDto>(updated);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _userDal.GetAsync(u => u.Id == id)
                ?? throw new KeyNotFoundException($"Kullanıcı bulunamadı: {id}");

            _userDal.Delete(entity);
            await _userDal.SaveChangesAsync();
        }

        // ── Authentication ────────────────────────────────────────────────────────

        public async Task<AppUserDto?> ValidateUserAsync(string email, string password)
        {
            var entity = await _userDal.GetAsync(u => u.Email.ToLower() == email.ToLower() && u.IsActive);
            if (entity == null || !VerifyPassword(password, entity.PasswordHash))
                return null;
            return _mapper.Map<AppUserDto>(entity);
        }

        public async Task<bool> EmailExistsAsync(string email) =>
            await _userDal.GetAsync(u => u.Email.ToLower() == email.ToLower()) != null;

        public async Task<bool> ChangePasswordAsync(Guid userId, string oldPassword, string newPassword)
        {
            var entity = await _userDal.GetAsync(u => u.Id == userId);
            if (entity == null || !VerifyPassword(oldPassword, entity.PasswordHash))
                return false;

            entity.PasswordHash = HashPassword(newPassword);
            entity.UpdatedAt = DateTime.UtcNow;
            _userDal.Update(entity);
            await _userDal.SaveChangesAsync();
            return true;
        }

        public async Task UpdateLastLoginAsync(Guid userId)
        {
            var entity = await _userDal.GetAsync(u => u.Id == userId);
            if (entity != null)
            {
                entity.LastLoginAt = DateTime.UtcNow;
                _userDal.Update(entity);
                await _userDal.SaveChangesAsync();
            }
        }

        // ── Role-based lists ──────────────────────────────────────────────────────

        public Task<List<AppUserListDto>> GetStudentsAsync() => GetByRoleAsync(UserRoles.Student);
        public Task<List<AppUserListDto>> GetTeachersAsync() => GetByRoleAsync(UserRoles.Teacher);
        public Task<List<AppUserListDto>> GetAdminsAsync() => GetByRoleAsync(UserRoles.Admin);

        // ── Validation helpers ────────────────────────────────────────────────────

        public Task<bool> ExistsAsync(Guid id) => _userDal.ExistsAsync(id);

        public Task<bool> IsTeacherAsync(Guid id) => _userDal.IsInRoleAsync(id, UserRoles.Teacher);

        public Task<bool> IsStudentAsync(Guid id) => _userDal.IsInRoleAsync(id, UserRoles.Student);

        public Task<bool> IsActiveAsync(Guid id) => _userDal.IsActiveAsync(id);

        // ── Private password helpers ──────────────────────────────────────────────

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private static bool VerifyPassword(string password, string hash) =>
            HashPassword(password) == hash;
    }
}
