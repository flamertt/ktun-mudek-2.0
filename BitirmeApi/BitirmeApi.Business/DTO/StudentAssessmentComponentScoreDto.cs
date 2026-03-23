namespace BitirmeApi.Business.DTO
{
    /// <summary>StudentEnrollmentId → EnrollmentId (tek öğrenci kayıt modeli)</summary>
    public class CreateStudentAssessmentComponentScoreDto
    {
        public Guid AssessmentComponentId { get; set; }
        public Guid EnrollmentId { get; set; }
        public decimal? Score { get; set; }
        public string? Notes { get; set; }
        public string? EvaluatedBy { get; set; }
    }

    public class UpdateStudentAssessmentComponentScoreDto
    {
        public Guid Id { get; set; }
        public decimal? Score { get; set; }
        public string? Notes { get; set; }
        public string? EvaluatedBy { get; set; }
        public DateTime? EvaluatedAt { get; set; }
    }

    public class StudentAssessmentComponentScoreDto
    {
        public Guid Id { get; set; }
        public Guid AssessmentComponentId { get; set; }
        public Guid EnrollmentId { get; set; }
        public decimal? Score { get; set; }
        public string? Notes { get; set; }
        public string? EvaluatedBy { get; set; }
        public DateTime? EvaluatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? ComponentName { get; set; }
        public string? ComponentType { get; set; }
        public decimal? MaxScore { get; set; }
        public string? StudentNumber { get; set; }
        public string? StudentName { get; set; }
    }

    public class BulkStudentScoreDto
    {
        public Guid AssessmentComponentId { get; set; }
        public List<StudentScoreItem> Scores { get; set; } = new();
    }

    public class StudentScoreItem
    {
        public Guid EnrollmentId { get; set; }
        public decimal? Score { get; set; }
        public string? Notes { get; set; }
    }
}
