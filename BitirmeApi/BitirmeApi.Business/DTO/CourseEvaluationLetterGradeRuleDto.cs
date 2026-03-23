using System;

namespace BitirmeApi.Business.DTO
{
    /// <summary>
    /// Harf notu kuralı oluşturma DTO'su
    /// </summary>
    public class CreateCourseEvaluationLetterGradeRuleDto
    {
        public Guid CourseEvaluationId { get; set; }
        public string LetterGrade { get; set; } = default!;
        public decimal MinScore { get; set; }
        public decimal MaxScore { get; set; }
        public bool IsPassing { get; set; } = true;
        public decimal? MinimumFinalScore { get; set; }
        public string? Description { get; set; }
    }

    /// <summary>
    /// Harf notu kuralı güncelleme DTO'su
    /// </summary>
    public class UpdateCourseEvaluationLetterGradeRuleDto
    {
        public Guid Id { get; set; }
        public string LetterGrade { get; set; } = default!;
        public decimal MinScore { get; set; }
        public decimal MaxScore { get; set; }
        public bool IsPassing { get; set; }
        public decimal? MinimumFinalScore { get; set; }
        public string? Description { get; set; }
    }

    /// <summary>
    /// Harf notu kuralı response DTO'su
    /// </summary>
    public class CourseEvaluationLetterGradeRuleDto
    {
        public Guid Id { get; set; }
        public Guid CourseEvaluationId { get; set; }
        public string LetterGrade { get; set; } = default!;
        public decimal MinScore { get; set; }
        public decimal MaxScore { get; set; }
        public bool IsPassing { get; set; }
        public decimal? MinimumFinalScore { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Toplu harf notu kuralları tanımlama DTO'su
    /// </summary>
    public class BulkLetterGradeRulesDto
    {
        public Guid CourseEvaluationId { get; set; }
        public List<LetterGradeRuleItem> Rules { get; set; } = new();
    }

    public class LetterGradeRuleItem
    {
        public string LetterGrade { get; set; } = default!;
        public decimal MinScore { get; set; }
        public decimal MaxScore { get; set; }
        public bool IsPassing { get; set; } = true;
        public decimal? MinimumFinalScore { get; set; }
        public string? Description { get; set; }
    }
}
