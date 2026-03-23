using BitirmeApi.Core.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitirmeApi.Entity.Entities
{
    /// <summary>
    /// MÜDEK ders değerlendirme belgesi.
    /// Tek CourseOffering'e bağlıdır (unique).
    /// Tüm ders/dönem/hoca bilgisi CourseOffering üzerinden gelir — denormalized string alanlar yok.
    /// Unique: (CourseOfferingId)
    /// </summary>
    public class CourseEvaluation : IEntity
    {
        public CourseEvaluation()
        {
            Exams = new List<Exam>();
            LetterGradeRules = new List<CourseEvaluationLetterGradeRule>();
            ProgramOutcomeContributions = new List<ProgramOutcomeContribution>();
        }

        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>Zorunlu — 1 offering = 1 evaluation</summary>
        [Required]
        public Guid CourseOfferingId { get; set; }
        public CourseOffering CourseOffering { get; set; } = default!;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public string? StudentFeedbackEvaluation { get; set; }
        public string? ProgramOutcomeEvaluation { get; set; }
        public string? GeneralEvaluation { get; set; }
        public string? ImprovementSuggestions { get; set; }

        // Navigation — sınav ve puan yapısı
        public ICollection<Exam> Exams { get; set; }
        public ICollection<CourseEvaluationLetterGradeRule> LetterGradeRules { get; set; }

        /// <summary>Bu değerlendirme için CLO → PO katkı düzeyleri (catalog CLO kullanılır)</summary>
        public ICollection<ProgramOutcomeContribution> ProgramOutcomeContributions { get; set; }
    }
}
