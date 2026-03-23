using System;

namespace BitirmeApi.Business.DTO
{
    /// <summary>
    /// Değerlendirme bileşeni - öğrenme çıktısı eşleştirmesi oluşturma DTO'su
    /// </summary>
    public class CreateAssessmentComponentOutcomeMappingDto
    {
        public Guid AssessmentComponentId { get; set; }
        public Guid CourseLearningOutcomeId { get; set; }
        public decimal Weight { get; set; }
    }

    /// <summary>
    /// Değerlendirme bileşeni - öğrenme çıktısı eşleştirmesi güncelleme DTO'su
    /// </summary>
    public class UpdateAssessmentComponentOutcomeMappingDto
    {
        public Guid Id { get; set; }
        public Guid AssessmentComponentId { get; set; }
        public Guid CourseLearningOutcomeId { get; set; }
        public decimal Weight { get; set; }
    }

    /// <summary>
    /// Değerlendirme bileşeni - öğrenme çıktısı eşleştirmesi response DTO'su
    /// </summary>
    public class AssessmentComponentOutcomeMappingDto
    {
        public Guid Id { get; set; }
        public Guid AssessmentComponentId { get; set; }
        public Guid CourseLearningOutcomeId { get; set; }
        public decimal Weight { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Navigation properties için basit bilgiler
        public string? ComponentName { get; set; }
        public string? OutcomeCode { get; set; }
        public string? OutcomeDescription { get; set; }
    }
}
