using System;

namespace BitirmeApi.Business.DTO
{
    /// <summary>
    /// Sınav sorusu - öğrenme çıktısı eşleştirmesi oluşturma DTO'su
    /// </summary>
    public class CreateExamQuestionOutcomeMappingDto
    {
        public Guid ExamQuestionId { get; set; }
        public Guid CourseLearningOutcomeId { get; set; }
        public decimal Weight { get; set; }
    }

    /// <summary>
    /// Sınav sorusu - öğrenme çıktısı eşleştirmesi güncelleme DTO'su
    /// </summary>
    public class UpdateExamQuestionOutcomeMappingDto
    {
        public Guid Id { get; set; }
        public Guid ExamQuestionId { get; set; }
        public Guid CourseLearningOutcomeId { get; set; }
        public decimal Weight { get; set; }
    }

    /// <summary>
    /// Sınav sorusu - öğrenme çıktısı eşleştirmesi response DTO'su
    /// </summary>
    public class ExamQuestionOutcomeMappingDto
    {
        public Guid Id { get; set; }
        public Guid ExamQuestionId { get; set; }
        public Guid CourseLearningOutcomeId { get; set; }
        public decimal Weight { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Navigation properties için basit bilgiler
        public string? QuestionTitle { get; set; }
        public string? OutcomeCode { get; set; }
        public string? OutcomeDescription { get; set; }
    }
}
