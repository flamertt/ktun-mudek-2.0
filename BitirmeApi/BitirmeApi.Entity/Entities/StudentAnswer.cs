using BitirmeApi.Core.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitirmeApi.Entity.Entities
{
    /// <summary>
    /// Öğrencinin sınav sorusuna verdiği cevap / aldığı puan.
    /// StudentEnrollment yerine Enrollment kullanılır (tek öğrenci kayıt modeli).
    /// Unique: (ExamQuestionId, EnrollmentId)
    /// </summary>
    public class StudentAnswer : IEntity
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid ExamQuestionId { get; set; }

        [ForeignKey("ExamQuestionId")]
        public ExamQuestion ExamQuestion { get; set; } = default!;

        /// <summary>Tek öğrenci kayıt modeli: Enrollment</summary>
        [Required]
        public Guid EnrollmentId { get; set; }

        [ForeignKey("EnrollmentId")]
        public Enrollment Enrollment { get; set; } = default!;

        public decimal Score { get; set; }
    }
}
