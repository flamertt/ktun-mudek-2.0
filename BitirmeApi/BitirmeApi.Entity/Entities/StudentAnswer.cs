using BitirmeApi.Core.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitirmeApi.Entity.Entities
{
    /// <summary>
    /// Öğrencinin sınav sorusuna verdiği cevap / aldığı puan.
    /// Unique: (ExamQuestionId, ExternalStudentId)
    /// </summary>
    public class StudentAnswer : IEntity
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid ExamQuestionId { get; set; }

        [ForeignKey("ExamQuestionId")]
        public ExamQuestion ExamQuestion { get; set; } = default!;

        /// <summary>Üniversite API Student ID (int)</summary>
        [Required]
        public int ExternalStudentId { get; set; }

        public decimal Score { get; set; }
    }
}
