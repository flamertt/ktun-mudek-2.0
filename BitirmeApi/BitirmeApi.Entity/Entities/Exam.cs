using BitirmeApi.Core.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitirmeApi.Entity.Entities
{
    public class Exam : IEntity
    {
        public Exam()
        {
            Questions = new List<ExamQuestion>();
        }

        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid CourseEvaluationId { get; set; }

        [ForeignKey("CourseEvaluationId")]
        public CourseEvaluation CourseEvaluation { get; set; } = default!;

        [Required, MaxLength(128)]
        public string ExamType { get; set; } = default!; // Vize1, Vize2, Ödev, Quiz, Proje, Final

        public double WeightPercentage { get; set; } // 0-100 arası yüzde

        public int OrderIndex { get; set; }

        public ICollection<ExamQuestion> Questions { get; set; }
    }
}
