using BitirmeApi.Core.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitirmeApi.Entity.Entities
{
    /// <summary>
    /// Anket — üniversite API CourseOffering'e bağlıdır (ExternalCourseOfferingId).
    /// </summary>
    public class Survey : IEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>Üniversite API CourseOffering ID</summary>
        [Required]
        public int ExternalCourseOfferingId { get; set; }

        [Required, MaxLength(256)]
        public string Title { get; set; } = default!;

        [MaxLength(2000)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
    }
}
