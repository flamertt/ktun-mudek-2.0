using BitirmeApi.Core.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitirmeApi.Entity.Entities
{
    /// <summary>
    /// Soru veya assessment component bazlı başarı metrikleri.
    /// Soru satırı: ExamQuestionId dolu; bileşen satırı: AssessmentComponentId dolu.
    /// </summary>
    public class ExamQuestionEvaluationResult : IEntity
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

        public Guid? ExamQuestionId { get; set; }

        [ForeignKey("ExamQuestionId")]
        public ExamQuestion? ExamQuestion { get; set; }

        public Guid? AssessmentComponentId { get; set; }

        [ForeignKey("AssessmentComponentId")]
        public AssessmentComponent? AssessmentComponent { get; set; }

        public int QuestionNumber { get; set; }

        [Required]
        public decimal MaxScore { get; set; }

        public decimal? AverageScore { get; set; }
        public decimal? AchievementRate { get; set; }
        public int IncludedStudentCount { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
