using BitirmeApi.Core.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitirmeApi.Entity.Entities
{
    /// <summary>
    /// Öğrencinin değerlendirme bileşeninden aldığı puan.
    /// Unique: (AssessmentComponentId, ExternalStudentId)
    /// </summary>
    public class StudentAssessmentComponentScore : IEntity
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid AssessmentComponentId { get; set; }

        [ForeignKey("AssessmentComponentId")]
        public AssessmentComponent AssessmentComponent { get; set; } = default!;

        /// <summary>Üniversite API Student ID (int)</summary>
        [Required]
        public int ExternalStudentId { get; set; }

        public decimal? Score { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public string? EvaluatedBy { get; set; }
        public DateTime? EvaluatedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
