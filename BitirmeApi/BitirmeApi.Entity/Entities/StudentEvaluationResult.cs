using BitirmeApi.Core.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitirmeApi.Entity.Entities
{
    /// <summary>
    /// Öğrenci nihai MÜDEK hesap satırı (tek güncel snapshot).
    /// Unique: (ExternalCourseOfferingId, ExternalStudentId)
    /// </summary>
    public class StudentEvaluationResult : IEntity
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>Üniversite API CourseOffering ID</summary>
        [Required]
        public int ExternalCourseOfferingId { get; set; }

        /// <summary>Üniversite API Student ID</summary>
        [Required]
        public int ExternalStudentId { get; set; }

        [MaxLength(64)]
        public string? ExternalStudentNumber { get; set; }

        [MaxLength(256)]
        public string? ExternalStudentName { get; set; }

        public decimal? MidtermScore { get; set; }
        public decimal? FinalScore { get; set; }
        public decimal? MakeupScore { get; set; }

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
