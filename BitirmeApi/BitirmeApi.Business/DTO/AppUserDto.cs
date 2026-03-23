using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeApi.Business.DTO
{
    /// <summary>
    /// Kullanıcı oluşturma için DTO
    /// </summary>
    public class CreateAppUserDto
    {
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!; // Plain text password (will be hashed)
        public string Role { get; set; } = default!; // Admin, Teacher, Student
        public string? StudentNumber { get; set; }
        public string? Title { get; set; }
        public string? PhoneNumber { get; set; }
        public Guid? ProgramEntityId { get; set; }
    }

    /// <summary>
    /// Kullanıcı güncelleme için DTO
    /// </summary>
    public class UpdateAppUserDto
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; } // Opsiyonel - sadece şifre değiştirilecekse
        public string? Role { get; set; }
        public string? StudentNumber { get; set; }
        public string? Title { get; set; }
        public string? PhoneNumber { get; set; }
        public Guid? ProgramEntityId { get; set; }
        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// Kullanıcı bilgisi döndürme için DTO
    /// </summary>
    public class AppUserDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Role { get; set; } = default!;
        public string? StudentNumber { get; set; }
        public string? Title { get; set; }
        public string? PhoneNumber { get; set; }
        public Guid? ProgramEntityId { get; set; }
        public string? ProgramName { get; set; } // Navigation property
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }

    /// <summary>
    /// Kullanıcı listesi için basitleştirilmiş DTO
    /// </summary>
    public class AppUserListDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Role { get; set; } = default!;
        public string? StudentNumber { get; set; }
        public string? ProgramName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Login için DTO
    /// </summary>
    public class LoginDto
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

    /// <summary>
    /// Login response DTO (JWT token ile birlikte)
    /// </summary>
    public class LoginResponseDto
    {
        public string Token { get; set; } = default!;
        public AppUserDto User { get; set; } = default!;
    }
}
