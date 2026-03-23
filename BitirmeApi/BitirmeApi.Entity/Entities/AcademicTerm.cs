using BitirmeApi.Core.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitirmeApi.Entity.Entities
{
    /// <summary>
    /// Akademik dönem. Aynı anda yalnızca bir dönem IsActive olabilir.
    /// Unique: (StartYear, EndYear, TermType)
    /// </summary>
    public class AcademicTerm : IEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public int StartYear { get; set; }

        [Required]
        public int EndYear { get; set; }

        /// <summary>"Guz", "Bahar", "Yaz"</summary>
        [Required, MaxLength(16)]
        public string TermType { get; set; } = default!;

        /// <summary>İnsan okunabilir ad (örn: "2025-2026 Güz"). Servis tarafından üretilir.</summary>
        [Required, MaxLength(128)]
        public string Name { get; set; } = default!;

        /// <summary>Varsayılan çalışma dönemi. SetActive servisi ile yönetilir.</summary>
        public bool IsActive { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<CourseOffering> CourseOfferings { get; set; } = new List<CourseOffering>();
    }

    /// <summary>Dönem tipi sabitleri</summary>
    public static class TermType
    {
        public const string Guz = "Guz";
        public const string Bahar = "Bahar";
        public const string Yaz = "Yaz";
    }
}
