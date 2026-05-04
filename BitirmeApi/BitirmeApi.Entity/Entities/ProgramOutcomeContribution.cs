using BitirmeApi.Core.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitirmeApi.Entity.Entities
{
    /// <summary>
    /// Bir ders değerlendirmesinde CLO'nun belirli bir PÇ'ye katkı düzeyi.
    /// Unique: (CourseEvaluationId, ExternalCloId, ProgramOutcomeCode)
    /// </summary>
    public class ProgramOutcomeContribution : IEntity
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid CourseEvaluationId { get; set; }

        [ForeignKey("CourseEvaluationId")]
        public CourseEvaluation CourseEvaluation { get; set; } = default!;

        /// <summary>Üniversite API CLO ID (int)</summary>
        [Required]
        public int ExternalCloId { get; set; }

        [MaxLength(64)]
        public string? CloCode { get; set; }

        /// <summary>Program çıktısı kodu: PÇ1, PÇ2, ... PÇ11</summary>
        [Required, MaxLength(64)]
        public string ProgramOutcomeCode { get; set; } = default!;

        /// <summary>Katkı düzeyi: 0-5</summary>
        public int ContributionLevel { get; set; } = 0;
    }
}
