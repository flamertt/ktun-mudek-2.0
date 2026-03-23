using BitirmeApi.Core.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitirmeApi.Entity.Entities
{
    /// <summary>
    /// Bir dersin belirli akademik dönemde açılmış hali.
    /// Öğretmen, şube, geçme notu ve dönemsel tüm veriler burada tutulur.
    /// Unique: (CourseId, AcademicTermId, Section)
    /// </summary>
    public class CourseOffering : IEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid CourseId { get; set; }
        public Course Course { get; set; } = default!;

        [Required]
        public Guid AcademicTermId { get; set; }
        public AcademicTerm AcademicTerm { get; set; } = default!;

        public Guid? TeacherId { get; set; }

        [ForeignKey("TeacherId")]
        public AppUser? Teacher { get; set; }

        /// <summary>
        /// Şube kodu — zorunlu. Şube mantığı yoksa "A" kullan.
        /// Unique index içinde yer alır: (CourseId, AcademicTermId, Section)
        /// </summary>
        [Required, MaxLength(16)]
        public string Section { get; set; } = "A";

        public decimal? PassingGrade { get; set; }

        /// <summary>Maksimum öğrenci kapasitesi</summary>
        public int? Quota { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Survey> Surveys { get; set; } = new List<Survey>();

        /// <summary>1 Opening = 1 Evaluation (unique constraint ile garanti edilir)</summary>
        public CourseEvaluation? CourseEvaluation { get; set; }
    }
}
