namespace BitirmeApi.Business.DTO
{
    /// <summary>Katalog CLO — dönemsel değil, Course'a bağlı</summary>
    public class CourseLearningOutcomeDto
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public string? CourseCode { get; set; }
        public string Code { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int OrderIndex { get; set; }
    }

    public class CreateCourseLearningOutcomeDto
    {
        public Guid CourseId { get; set; }
        public string Code { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int OrderIndex { get; set; } = 1;
    }

    public class UpdateCourseLearningOutcomeDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int OrderIndex { get; set; }
    }

    public class CloPoMapDto
    {
        public Guid Id { get; set; }
        public Guid CourseLearningOutcomeId { get; set; }
        public string? CloCode { get; set; }
        public Guid ProgramOutcomeId { get; set; }
        public string? PoCode { get; set; }
        public decimal Weight { get; set; }
    }

    public class CreateCloPoMapDto
    {
        public Guid CourseLearningOutcomeId { get; set; }
        public Guid ProgramOutcomeId { get; set; }
        public decimal Weight { get; set; }
    }

    public class UpdateCloPoWeightDto
    {
        public decimal Weight { get; set; }
    }
}
