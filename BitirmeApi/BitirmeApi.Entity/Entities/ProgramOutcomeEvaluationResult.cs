using BitirmeApi.Core.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitirmeApi.Entity.Entities
{
    /// <summary>
    /// PÇ / PO başarı skoru (offering bazlı, tek snapshot).
    /// Unique: (ExternalCourseOfferingId, ExternalProgramOutcomeId)
    /// </summary>
    public class ProgramOutcomeEvaluationResult : IEntity
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>Üniversite API CourseOffering ID</summary>
        [Required]
        public int ExternalCourseOfferingId { get; set; }

        /// <summary>Üniversite API ProgramOutcome ID</summary>
        [Required]
        public int ExternalProgramOutcomeId { get; set; }

        [MaxLength(64)]
        public string? ProgramOutcomeCode { get; set; }

        [MaxLength(256)]
        public string? ProgramOutcomeTitle { get; set; }

        public decimal? AchievementScore { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
