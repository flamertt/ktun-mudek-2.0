using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BitirmeApi.Core.Entity;

namespace BitirmeApi.Entity.Entities
{
    public class CloPoMap:IEntity
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid CourseLearningOutcomeId { get; set; }
        [Required]
        public CourseLearningOutcome CLO { get; set; } = default!;

        [Required]
        public Guid ProgramOutcomeId { get; set; }
        [Required]
        public ProgramOutcome PO { get; set; } = default!;

        [Range(0, 1)]
        public decimal Weight { get; set; } = 1m;
    }
}
