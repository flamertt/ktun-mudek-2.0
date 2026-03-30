using BitirmeApi.Core.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitirmeApi.Entity.Entities
{
    /// <summary>
    /// Sınav bazlı özet istatistik (geçen öğrenci filtresine göre).
    /// Unique: (CourseOfferingId, ExamId)
    /// </summary>
    public class ExamEvaluationResult : IEntity
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid CourseOfferingId { get; set; }
        public CourseOffering CourseOffering { get; set; } = default!;

        [Required]
        public Guid ExamId { get; set; }
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
