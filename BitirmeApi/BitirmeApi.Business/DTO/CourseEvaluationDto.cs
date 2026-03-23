namespace BitirmeApi.Business.DTO
{
    /// <summary>Ders değerlendirme özet (liste)</summary>
    public class CourseEvaluationListDto
    {
        public Guid Id { get; set; }
        public Guid CourseOfferingId { get; set; }
        public string CourseCode { get; set; } = default!;
        public string CourseName { get; set; } = default!;
        public string TermName { get; set; } = default!;
        public string? TeacherName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>Ders değerlendirme tam detay</summary>
    public class CourseEvaluationDetailDto
    {
        public Guid Id { get; set; }
        public Guid CourseOfferingId { get; set; }

        // CourseOffering üzerinden gelen bilgiler
        public string CourseCode { get; set; } = default!;
        public string CourseName { get; set; } = default!;
        public string TermName { get; set; } = default!;
        public string Section { get; set; } = default!;
        public string? TeacherName { get; set; }
        public string? ProgramName { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string? StudentFeedbackEvaluation { get; set; }
        public string? ProgramOutcomeEvaluation { get; set; }
        public string? GeneralEvaluation { get; set; }
        public string? ImprovementSuggestions { get; set; }

        public int StudentCount { get; set; }
    }

    public class CourseEvaluationCreateDto
    {
        public Guid CourseOfferingId { get; set; }
        public string? StudentFeedbackEvaluation { get; set; }
        public string? ProgramOutcomeEvaluation { get; set; }
        public string? GeneralEvaluation { get; set; }
        public string? ImprovementSuggestions { get; set; }
    }

    public class CourseEvaluationUpdateDto
    {
        public Guid Id { get; set; }
        public string? StudentFeedbackEvaluation { get; set; }
        public string? ProgramOutcomeEvaluation { get; set; }
        public string? GeneralEvaluation { get; set; }
        public string? ImprovementSuggestions { get; set; }
    }
}
