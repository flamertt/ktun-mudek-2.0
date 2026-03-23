using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BitirmeApi.Core.Entity;

namespace BitirmeApi.Entity.Entities
{
    /// <summary>
    /// Katalog seviyesinde ders öğrenim çıktısı (CLO).
    /// Dönemden bağımsız, Course'a bağlı.
    /// Sınav/değerlendirme eşlemeleri doğrudan bu entity üzerinden kurulur.
    /// Unique: (CourseId, Code)
    /// </summary>
    public class CourseLearningOutcome : IEntity
    {
        public CourseLearningOutcome()
        {
            Maps = new List<CloPoMap>();
            ExamQuestionMappings = new List<ExamQuestionOutcomeMapping>();
            AssessmentComponentMappings = new List<AssessmentComponentOutcomeMapping>();
        }

        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid CourseId { get; set; }
        public Course Course { get; set; } = default!;

        [Required, MaxLength(32)]
        public string Code { get; set; } = default!;

        [Required, MaxLength(2000)]
        public string Description { get; set; } = default!;

        /// <summary>Sıralama indeksi (ÖÇ1, ÖÇ2, ...)</summary>
        public int OrderIndex { get; set; } = 1;

        // CLO → Program Outcome ağırlık haritası (akademik katalog)
        public ICollection<CloPoMap> Maps { get; set; }

        // Dönemsel ölçme eşlemeleri (tek CLO kaynağı)
        public ICollection<ExamQuestionOutcomeMapping> ExamQuestionMappings { get; set; }
        public ICollection<AssessmentComponentOutcomeMapping> AssessmentComponentMappings { get; set; }
    }
}
