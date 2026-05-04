using BitirmeApi.Core.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitirmeApi.Entity.Entities
{
    /// <summary>
    /// Sınav bazlı özet istatistik (geçen öğrenci filtresine göre).
    /// Unique: (ExternalCourseOfferingId, ExamId)
    /// </summary>
    public class ExamEvaluationResult : IEntity
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>Üniversite API CourseOffering ID</summary>
        [Required]
        public int ExternalCourseOfferingId { get; set; }

        [Required]
        public Guid ExamId { get; set; }

        [ForeignKey("ExamId")]
        public Exam Exam { get; set; } = default!;

        public int ParticipantCount { get; set; }
        public int IncludedStudentCount { get; set; }
        public int PerfectScoreCount { get; set; }

        public decimal? MaxTotalScore { get; set; }
        public decimal? MinTotalScore { get; set; }
        public decimal? AverageTotalScore { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
