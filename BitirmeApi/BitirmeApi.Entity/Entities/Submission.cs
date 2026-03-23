using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BitirmeApi.Core.Entity;

namespace BitirmeApi.Entity.Entities
{
    public class Submission : IEntity
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid SurveyId { get; set; }
        [Required]
        public Survey Survey { get; set; } = default!;

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public AppUser User { get; set; } = default!; 

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    // İstatistiklere dahil edilsin mi? (Dersten geçemeyen öğrenciler için false)
    public bool IncludeInStatistics { get; set; } = true;

    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}
