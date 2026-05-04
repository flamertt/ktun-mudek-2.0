using BitirmeApi.Core.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitirmeApi.Entity.Entities
{
    /// <summary>
    /// Program bazında harf notu / geçme aralıkları (üniversite ExternalProgramId).
    /// </summary>
    public class LetterGradeRule : IEntity
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public int ExternalProgramId { get; set; }

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
