namespace BitirmeApi.Business.DTO
{
    /// <summary>Sınav liste görünümü</summary>
    public class ExamListDto
    {
        public Guid Id { get; set; }
        public Guid CourseEvaluationId { get; set; }
        public string ExamType { get; set; } = default!;
        public double WeightPercentage { get; set; }
        public int OrderIndex { get; set; }
        public int QuestionCount { get; set; }
    }

    /// <summary>Sınav detayı — sorular dahil</summary>
    public class ExamDetailDto
    {
        public Guid Id { get; set; }
        public Guid CourseEvaluationId { get; set; }
        public string ExamType { get; set; } = default!;
        public double WeightPercentage { get; set; }
        public int OrderIndex { get; set; }
        public List<ExamQuestionDto> Questions { get; set; } = new();
    }

    public class CreateExamDto
    {
        /// <summary>CourseEvaluationId — route'dan set edilir</summary>
        public Guid CourseEvaluationId { get; set; }

        /// <summary>Vize1, Vize2, Final, Quiz, Ödev, Proje, Lab</summary>
        public string ExamType { get; set; } = default!;

        /// <summary>Değerlendirme içindeki ağırlık yüzdesi (0-100)</summary>
        public double WeightPercentage { get; set; }

        public int OrderIndex { get; set; } = 1;
    }

    public class UpdateExamDto
    {
        public Guid Id { get; set; }
        public string ExamType { get; set; } = default!;
        public double WeightPercentage { get; set; }
        public int OrderIndex { get; set; }
    }

    // ── ExamQuestion DTOs ─────────────────────────────────────────────────────

    /// <summary>Soru — CLO eşlemeleriyle birlikte</summary>
    public class ExamQuestionDto
    {
        public Guid Id { get; set; }
        public Guid ExamId { get; set; }
        public int QuestionNumber { get; set; }
        public decimal MaxScore { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? QuestionType { get; set; }
        public List<ExamQuestionOutcomeMappingDto> OutcomeMappings { get; set; } = new();
    }

    public class CreateExamQuestionDto
    {
        /// <summary>ExamId — route'dan set edilir</summary>
        public Guid ExamId { get; set; }
        public int QuestionNumber { get; set; }
        public decimal MaxScore { get; set; } = 100;
        public string? Title { get; set; }
        public string? Description { get; set; }

        /// <summary>WrittenQuestion, ShortAnswer, MultipleChoice, Essay, Homework, Project, Lab, Other</summary>
        public string? QuestionType { get; set; }
    }

    public class UpdateExamQuestionDto
    {
        public Guid Id { get; set; }
        public int QuestionNumber { get; set; }
        public decimal MaxScore { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? QuestionType { get; set; }
    }

    // ── CLO Eşleme DTO'su (güncelleme) ───────────────────────────────────────

    public class UpdateQuestionOutcomeMappingWeightDto
    {
        public decimal Weight { get; set; }
    }
}
