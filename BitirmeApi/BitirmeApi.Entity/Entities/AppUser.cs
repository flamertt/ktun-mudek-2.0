using BitirmeApi.Core.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitirmeApi.Entity.Entities
{
    /// <summary>
    /// Kullanıcı rolleri
    /// </summary>
    public enum UserRole
    {
        Admin,
        Teacher,
        Student
    }

    /// <summary>
    /// Tek kullanıcı tablosu - Identity'den bağımsız
    /// Öğrenci, öğretmen ve admin kullanıcıları bu tabloda tutulur
    /// </summary>
    public class AppUser : IEntity
    {
        public AppUser()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }

        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required, MaxLength(256)]
        public string FullName { get; set; } = default!;

        [Required, MaxLength(256)]
        public string Email { get; set; } = default!;

        [Required, MaxLength(512)]
        public string PasswordHash { get; set; } = default!;

        [Required, MaxLength(50)]
        public string Role { get; set; } = default!; // Admin, Teacher, Student

        /// <summary>
        /// Öğrenci numarası (sadece öğrenciler için dolu)
        /// </summary>
        [MaxLength(64)]
        public string? StudentNumber { get; set; }

        /// <summary>
        /// Öğretmen unvanı (sadece öğretmenler için dolu)
        /// </summary>
        [MaxLength(256)]
        public string? Title { get; set; }

        /// <summary>
        /// Telefon numarası (opsiyonel)
        /// </summary>
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Bağlı olduğu program/bölüm (öğrenci ve öğretmenler için)
        /// </summary>
        public Guid? ProgramEntityId { get; set; }

        [ForeignKey("ProgramEntityId")]
        public ProgramEntity? Program { get; set; }

        /// <summary>
        /// Aktif mi? (pasif kullanıcılar giriş yapamaz)
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Hesap oluşturulma tarihi
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Son güncelleme tarihi
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Son giriş tarihi
        /// </summary>
        public DateTime? LastLoginAt { get; set; }
    }
}
