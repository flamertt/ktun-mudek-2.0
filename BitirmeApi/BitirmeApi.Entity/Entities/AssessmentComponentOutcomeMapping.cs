using BitirmeApi.Core.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitirmeApi.Entity.Entities
{
    /// <summary>
    /// Değerlendirme bileşeninin katalog CLO'larına ağırlıklı eşlemesi.
    /// CourseLearningOutcome = katalog seviyesi (dönemsel değil).
    /// Unique: (AssessmentComponentId, CourseLearningOutcomeId)
    /// </summary>
    public class AssessmentComponentOutcomeMapping : IEntity
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid AssessmentComponentId { get; set; }

        [ForeignKey("AssessmentComponentId")]
        public AssessmentComponent AssessmentComponent { get; set; } = default!;

        /// <summary>Katalog CLO — tek CLO kaynağı</summary>
        [Required]
        public Guid CourseLearningOutcomeId { get; set; }

        [ForeignKey("CourseLearningOutcomeId")]
        public CourseLearningOutcome CourseLearningOutcome { get; set; } = default!;

        [Required]
        public decimal Weight { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
