using BitirmeApi.Core.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitirmeApi.Entity.Entities
{
    /// <summary>
    /// Harf notu kurallarını dönem/değerlendirme bazlı saklar
    /// Aynı dersin farklı dönemlerde farklı harf notu sistemi olabilir
    /// </summary>
    public class CourseEvaluationLetterGradeRule : IEntity
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid CourseEvaluationId { get; set; }

        [ForeignKey("CourseEvaluationId")]
        public CourseEvaluation CourseEvaluation { get; set; } = default!;

        /// <summary>
        /// Harf notu (örn: "AA", "BA", "BB", "CC", "FF")
        /// </summary>
        [Required, MaxLength(5)]
        public string LetterGrade { get; set; } = default!;

        /// <summary>
        /// Minimum puan
        /// </summary>
        [Required]
        public decimal MinScore { get; set; }

        /// <summary>
        /// Maksimum puan
        /// </summary>
        [Required]
        public decimal MaxScore { get; set; }

        /// <summary>
        /// Geçer not mu?
        /// </summary>
        public bool IsPassing { get; set; } = true;

        /// <summary>
        /// Final şartı varsa minimum final puanı (opsiyonel)
        /// </summary>
        public decimal? MinimumFinalScore { get; set; }

        /// <summary>
        /// Açıklama (opsiyonel)
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
