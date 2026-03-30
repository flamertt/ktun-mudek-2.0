using BitirmeApi.Core.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitirmeApi.Entity.Entities
{
    /// <summary>
    /// Soru veya assessment component bazlı başarı metrikleri.
    /// Soru satırı: ExamQuestionId dolu; bileşen satırı: AssessmentComponentId dolu (tersine unique filtreli indeksler).
    /// </summary>
    public class ExamQuestionEvaluationResult : IEntity
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid CourseOfferingId { get; set; }
        public CourseOffering CourseOffering { get; set; } = default!;

        [Required]
        public Guid ExamId { get; set; }
        public Exam Exam { get; set; } = default!;

        public Guid? ExamQuestionId { get; set; }
        public ExamQuestion? ExamQuestion { get; set; }

        public Guid? AssessmentComponentId { get; set; }
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
