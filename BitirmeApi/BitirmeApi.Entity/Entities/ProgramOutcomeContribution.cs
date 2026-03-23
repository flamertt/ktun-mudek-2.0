using BitirmeApi.Core.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitirmeApi.Entity.Entities
{
    /// <summary>
    /// Bir ders değerlendirmesinde (CourseEvaluation) katalog CLO'nun
    /// belirli bir program çıktısına katkı düzeyini tutar.
    /// Unique: (CourseEvaluationId, CourseLearningOutcomeId, ProgramOutcomeCode)
    /// </summary>
    public class ProgramOutcomeContribution : IEntity
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>Hangi ders değerlendirmesine ait</summary>
        [Required]
        public Guid CourseEvaluationId { get; set; }

        [ForeignKey("CourseEvaluationId")]
        public CourseEvaluation CourseEvaluation { get; set; } = default!;

        /// <summary>Katalog CLO — tek CLO kaynağı</summary>
        [Required]
        public Guid CourseLearningOutcomeId { get; set; }

        [ForeignKey("CourseLearningOutcomeId")]
        public CourseLearningOutcome CourseLearningOutcome { get; set; } = default!;

        /// <summary>Program çıktısı kodu: PÇ1, PÇ2, ... PÇ11</summary>
        [Required, MaxLength(64)]
        public string ProgramOutcomeCode { get; set; } = default!;

        /// <summary>Katkı düzeyi: 0-5</summary>
        public int ContributionLevel { get; set; } = 0;
    }
}
