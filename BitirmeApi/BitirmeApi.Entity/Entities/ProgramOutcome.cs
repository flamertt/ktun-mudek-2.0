using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BitirmeApi.Core.Entity;

namespace BitirmeApi.Entity.Entities
{
    public class ProgramOutcome:IEntity
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid ProgramEntityId { get; set; }
        [Required]
        public ProgramEntity Program { get; set; } = default!;

        [Required, MaxLength(32)]
        public string Code { get; set; } = default!;

        [Required, MaxLength(256)]
        public string Title { get; set; } = default!;

        [Required, MaxLength(2000)]
        public string Description { get; set; } = default!;
    }
}
