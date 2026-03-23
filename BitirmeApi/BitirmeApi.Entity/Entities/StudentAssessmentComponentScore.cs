using BitirmeApi.Core.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitirmeApi.Entity.Entities
{
    /// <summary>
    /// Öğrencinin değerlendirme bileşeninden aldığı puan.
    /// StudentEnrollment yerine Enrollment kullanılır (tek öğrenci kayıt modeli).
    /// Unique: (AssessmentComponentId, EnrollmentId)
    /// </summary>
    public class StudentAssessmentComponentScore : IEntity
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid AssessmentComponentId { get; set; }

        [ForeignKey("AssessmentComponentId")]
        public AssessmentComponent AssessmentComponent { get; set; } = default!;

        /// <summary>Tek öğrenci kayıt modeli: Enrollment</summary>
        [Required]
        public Guid EnrollmentId { get; set; }

        [ForeignKey("EnrollmentId")]
        public Enrollment Enrollment { get; set; } = default!;

        public decimal? Score { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public string? EvaluatedBy { get; set; }
        public DateTime? EvaluatedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
