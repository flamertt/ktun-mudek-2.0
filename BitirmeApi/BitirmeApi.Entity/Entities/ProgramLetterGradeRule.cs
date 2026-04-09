using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BitirmeApi.Core.Entity;

namespace BitirmeApi.Entity.Entities
{
    /// <summary>
    /// Harf notu eşikleri program (lisans programı) düzeyinde; bu programa ait tüm ders açılışlarında MÜDEK hesabında kullanılır.
    /// Kurallar yoksa geriye dönük olarak ders değerlendirmesine özel kurallara düşülür.
    /// </summary>
    public class ProgramLetterGradeRule : IEntity
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid ProgramEntityId { get; set; }

        [ForeignKey(nameof(ProgramEntityId))]
        public ProgramEntity Program { get; set; } = default!;

        [Required, MaxLength(5)]
        public string LetterGrade { get; set; } = default!;

        [Required]
        public decimal MinScore { get; set; }

        [Required]
        public decimal MaxScore { get; set; }

        public bool IsPassing { get; set; } = true;

        public decimal? MinimumFinalScore { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
