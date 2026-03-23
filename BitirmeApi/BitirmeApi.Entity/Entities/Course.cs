using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BitirmeApi.Core.Entity;

namespace BitirmeApi.Entity.Entities
{
    /// <summary>
    /// Ders kataloğu — dönemden, öğretmenden ve şubeden tamamen bağımsız.
    /// Öğretmen/dönem/şube bilgileri CourseOffering'de tutulur.
    /// Unique: (ProgramEntityId, Code)
    /// </summary>
    public class Course : IEntity
    {
        public Course()
        {
            Clos = new List<CourseLearningOutcome>();
            CourseOfferings = new List<CourseOffering>();
        }

        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid ProgramEntityId { get; set; }
        public ProgramEntity Program { get; set; } = default!;

        [Required, MaxLength(32)]
        public string Code { get; set; } = default!;

        [Required, MaxLength(256)]
        public string Name { get; set; } = default!;

        public int Credits { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Katalog seviyesi CLO'lar
        public ICollection<CourseLearningOutcome> Clos { get; set; }

        // Bu dersin tüm dönemlerdeki açılışları
        public ICollection<CourseOffering> CourseOfferings { get; set; }
    }
}
