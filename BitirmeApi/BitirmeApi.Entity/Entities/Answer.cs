using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BitirmeApi.Core.Entity;

namespace BitirmeApi.Entity.Entities
{
    public class Answer : IEntity
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid SubmissionId { get; set; }
        [Required]
        public Submission Submission { get; set; } = default!;

        [Required]
        public Guid QuestionId { get; set; }
        [Required]
        public Question Question { get; set; } = default!;
        public decimal? ValueNumeric { get; set; }

        [MaxLength(2000)]
        public string? ValueText { get; set; }
    }
}
