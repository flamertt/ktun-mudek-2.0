using BitirmeApi.Core.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitirmeApi.Entity.Entities
{
    /// <summary>
    /// Öğrencinin bir ders açılışına (CourseOffering) kaydı.
    /// Sistemdeki TEK öğrenci kayıt modeli. StudentEnrollment kaldırıldı.
    /// Unique: (CourseOfferingId, StudentId)
    /// </summary>
    public class Enrollment : IEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid CourseOfferingId { get; set; }
        public CourseOffering CourseOffering { get; set; } = default!;

        [Required]
        public Guid StudentId { get; set; }

        [ForeignKey("StudentId")]
        public AppUser Student { get; set; } = default!;

        /// <summary>Kayıt durumu. Bkz: EnrollmentStatus sabitleri.</summary>
        [Required, MaxLength(32)]
        public string Status { get; set; } = EnrollmentStatus.Enrolled;

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Öğrencinin bu ders açılışındaki sınav puanları
        public ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
        public ICollection<StudentAssessmentComponentScore> ComponentScores { get; set; } = new List<StudentAssessmentComponentScore>();

        public ICollection<StudentEvaluationResult> EvaluationResults { get; set; } = new List<StudentEvaluationResult>();
    }

    /// <summary>Kayıt durumu sabitleri</summary>
    public static class EnrollmentStatus
    {
        public const string Enrolled = "Enrolled";
        public const string Passed = "Passed";
        public const string Failed = "Failed";
        public const string Withdrawn = "Withdrawn";
        public const string Repeat = "Repeat";
    }
}
