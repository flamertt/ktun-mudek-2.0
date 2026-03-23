using BitirmeApi.Core.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitirmeApi.Entity.Entities
{
    public class ExamQuestion : IEntity
    {
        public ExamQuestion()
        {
            StudentAnswers = new List<StudentAnswer>();
            OutcomeMappings = new List<ExamQuestionOutcomeMapping>();
        }

        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid ExamId { get; set; }

        [ForeignKey("ExamId")]
        public Exam Exam { get; set; } = default!;

        public int QuestionNumber { get; set; }

        public decimal MaxScore { get; set; } = 100;

        /// <summary>
        /// Soru başlığı / kısa açıklama
        /// </summary>
        [MaxLength(500)]
        public string? Title { get; set; }

        /// <summary>
        /// Detaylı açıklama
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Soru tipi: WrittenQuestion, ShortAnswer, MultipleChoice, Essay, Homework, Project, Lab, Other
        /// </summary>
        [MaxLength(50)]
        public string? QuestionType { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public ICollection<StudentAnswer> StudentAnswers { get; set; }
        
        /// <summary>
        /// Bu sorunun ilişkili olduğu öğrenme çıktıları ve ağırlıkları
        /// </summary>
        public ICollection<ExamQuestionOutcomeMapping> OutcomeMappings { get; set; }
    }
}
