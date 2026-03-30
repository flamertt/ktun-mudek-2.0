using BitirmeApi.Core.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitirmeApi.Entity.Entities
{
    /// <summary>
    /// Offering bazlı öğrenci nihai MÜDEK hesap satırı (tek güncel snapshot).
    /// Unique: (CourseOfferingId, EnrollmentId)
    /// </summary>
    public class StudentEvaluationResult : IEntity
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid CourseOfferingId { get; set; }
        public CourseOffering CourseOffering { get; set; } = default!;

        [Required]
        public Guid EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; } = default!;

        public decimal? MidtermScore { get; set; }
        public decimal? FinalScore { get; set; }
        public decimal? MakeupScore { get; set; }

        /// <summary>Bkz: <see cref="MudekUsedExamType"/></summary>
        [Required, MaxLength(32)]
        public string UsedExamType { get; set; } = MudekUsedExamType.None;

        public decimal? SuccessGrade { get; set; }

        [MaxLength(5)]
        public string? LetterGrade { get; set; }

        public bool IsPassed { get; set; }
        public bool IncludedInStatistics { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
