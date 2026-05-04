using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BitirmeApi.Core.Entity;

namespace BitirmeApi.Entity.Entities
{
    /// <summary>
    /// Anket gönderimi.
    /// Öğrenci üniversite API ile kimlik doğrular; yerel kullanıcı tablosu yoktur.
    /// Unique: (SurveyId, ExternalStudentId)
    /// </summary>
    public class Submission : IEntity
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid SurveyId { get; set; }

        [Required]
        public Survey Survey { get; set; } = default!;

        /// <summary>Üniversite sistemindeki öğrenci ID'si.</summary>
        [Required]
        public int ExternalStudentId { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        /// <summary>İstatistiklere dahil edilsin mi? (Dersten geçemeyen öğrenciler için false)</summary>
        public bool IncludeInStatistics { get; set; } = true;

        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}
