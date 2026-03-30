using BitirmeApi.Core.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitirmeApi.Entity.Entities
{
    /// <summary>
    /// PÇ / PO başarı skoru (offering bazlı, tek snapshot).
    /// Unique: (CourseOfferingId, ProgramOutcomeId)
    /// </summary>
    public class ProgramOutcomeEvaluationResult : IEntity
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid CourseOfferingId { get; set; }
        public CourseOffering CourseOffering { get; set; } = default!;

        [Required]
        public Guid ProgramOutcomeId { get; set; }
        public ProgramOutcome ProgramOutcome { get; set; } = default!;

        public decimal? AchievementScore { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
