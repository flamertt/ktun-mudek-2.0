using BitirmeApi.Core.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitirmeApi.Entity.Entities
{
    /// <summary>
    /// Quiz, ödev, proje, sunum, uygulama, lab gibi tüm değerlendirme bileşenlerini tek yapıda toplar
    /// </summary>
    public class AssessmentComponent : IEntity
    {
        public AssessmentComponent()
        {
            StudentScores = new List<StudentAssessmentComponentScore>();
            OutcomeMappings = new List<AssessmentComponentOutcomeMapping>();
        }

        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid ExamId { get; set; }

        [ForeignKey("ExamId")]
        public Exam Exam { get; set; } = default!;

        /// <summary>
        /// Bileşen adı (örn: "Ara Sınav Quiz 1", "Final Projesi", "Lab Çalışması 3")
        /// </summary>
        [Required, MaxLength(200)]
        public string Name { get; set; } = default!;

        /// <summary>
        /// Bileşen tipi: Quiz, Homework, Project, Presentation, Practice, Lab, ExamQuestion, Other
        /// </summary>
        [Required, MaxLength(50)]
        public string ComponentType { get; set; } = default!;

        /// <summary>
        /// Maksimum puan
        /// </summary>
        [Required]
        public decimal MaxScore { get; set; }

        /// <summary>
        /// Katkı yüzdesi (opsiyonel, örn: %20)
        /// </summary>
        public decimal? WeightPercentage { get; set; }

        /// <summary>
        /// Sıralama indeksi
        /// </summary>
        public int OrderIndex { get; set; }

        /// <summary>
        /// Açıklama (opsiyonel)
        /// </summary>
        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// Aktif mi?
        /// </summary>
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<StudentAssessmentComponentScore> StudentScores { get; set; }
        
        public ICollection<AssessmentComponentOutcomeMapping> OutcomeMappings { get; set; }
    }
}
