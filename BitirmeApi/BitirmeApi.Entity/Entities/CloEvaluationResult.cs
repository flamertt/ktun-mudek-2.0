using BitirmeApi.Core.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitirmeApi.Entity.Entities
{
    /// <summary>
    /// CLO / DÖÇ başarı skoru (sınav bazlı veya birleşik).
    /// Unique: (CourseOfferingId, CourseLearningOutcomeId, ResultType)
    /// </summary>
    public class CloEvaluationResult : IEntity
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid CourseOfferingId { get; set; }
        public CourseOffering CourseOffering { get; set; } = default!;

        [Required]
        public Guid CourseLearningOutcomeId { get; set; }
        public CourseLearningOutcome CourseLearningOutcome { get; set; } = default!;

        /// <summary>Bkz: <see cref="CloEvaluationResultType"/></summary>
        [Required, MaxLength(32)]
        public string ResultType { get; set; } = CloEvaluationResultType.Combined;

        public Guid? ExamId { get; set; }
        public Exam? Exam { get; set; }

        /// <summary>İlgili ResultType için hesaplanan başarı skoru (0–1).</summary>
        public decimal? AchievementScore { get; set; }

        /// <summary>İsteğe bağlı; birleşik raporlamada ek alan (şimdilik çoğu satırda null).</summary>
        public decimal? CombinedAchievementScore { get; set; }

        public decimal? SurveyScore { get; set; }
        public decimal? SurveyDifference { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
