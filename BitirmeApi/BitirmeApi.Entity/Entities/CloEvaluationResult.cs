using BitirmeApi.Core.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitirmeApi.Entity.Entities
{
    /// <summary>
    /// CLO / DÖÇ başarı skoru (sınav bazlı veya birleşik).
    /// Unique: (ExternalCourseOfferingId, ExternalCloId, ResultType)
    /// </summary>
    public class CloEvaluationResult : IEntity
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>Üniversite API CourseOffering ID</summary>
        [Required]
        public int ExternalCourseOfferingId { get; set; }

        /// <summary>Üniversite API CLO ID</summary>
        [Required]
        public int ExternalCloId { get; set; }

        [MaxLength(64)]
        public string? CloCode { get; set; }

        [MaxLength(2000)]
        public string? CloDescription { get; set; }

        [Required, MaxLength(32)]
        public string ResultType { get; set; } = CloEvaluationResultType.Combined;

        public Guid? ExamId { get; set; }

        [ForeignKey("ExamId")]
        public Exam? Exam { get; set; }

        public decimal? AchievementScore { get; set; }
        public decimal? CombinedAchievementScore { get; set; }

        public decimal? SurveyScore { get; set; }
        public decimal? SurveyDifference { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
