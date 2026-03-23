using System;
using System.Collections.Generic;

namespace BitirmeApi.Business.DTO
{
    /// <summary>
    /// Değerlendirme bileşeni oluşturma DTO'su
    /// </summary>
    public class CreateAssessmentComponentDto
    {
        public Guid ExamId { get; set; }
        public string Name { get; set; } = default!;
        public string ComponentType { get; set; } = default!;
        public decimal MaxScore { get; set; }
        public decimal? WeightPercentage { get; set; }
        public int OrderIndex { get; set; }
        public string? Description { get; set; }
    }

    /// <summary>
    /// Değerlendirme bileşeni güncelleme DTO'su
    /// </summary>
    public class UpdateAssessmentComponentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string ComponentType { get; set; } = default!;
        public decimal MaxScore { get; set; }
        public decimal? WeightPercentage { get; set; }
        public int OrderIndex { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Değerlendirme bileşeni response DTO'su
    /// </summary>
    public class AssessmentComponentDto
    {
        public Guid Id { get; set; }
        public Guid ExamId { get; set; }
        public string Name { get; set; } = default!;
        public string ComponentType { get; set; } = default!;
        public decimal MaxScore { get; set; }
        public decimal? WeightPercentage { get; set; }
        public int OrderIndex { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public string? ExamType { get; set; }
        public List<AssessmentComponentOutcomeMappingDto>? OutcomeMappings { get; set; }
    }

    /// <summary>
    /// Değerlendirme bileşeni liste DTO'su (detaysız)
    /// </summary>
    public class AssessmentComponentListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string ComponentType { get; set; } = default!;
        public decimal MaxScore { get; set; }
        public decimal? WeightPercentage { get; set; }
        public int OrderIndex { get; set; }
        public bool IsActive { get; set; }
    }
}
