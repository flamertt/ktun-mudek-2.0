namespace BitirmeApi.Business.DTO
{
    public class MudekEvaluationSnapshotDto
    {
        public int ExternalCourseOfferingId { get; set; }
        public Guid? CourseEvaluationId { get; set; }
        public DateTime? LastCalculatedAt { get; set; }
        public bool IsCalculationDirty { get; set; }
        public List<StudentEvaluationResultDto> StudentResults { get; set; } = new();
        public List<ExamEvaluationResultDto> ExamSummaries { get; set; } = new();
        public List<ExamQuestionEvaluationResultDto> QuestionAndComponentResults { get; set; } = new();
        public List<ExamAssessmentItemCloAchievementDto> ItemCloAchievements { get; set; } = new();
        public List<CloEvaluationResultDto> CloResults { get; set; } = new();
        public List<ProgramOutcomeEvaluationResultDto> ProgramOutcomeResults { get; set; } = new();
    }

    public class ExamAssessmentItemCloAchievementDto
    {
        public string ItemType { get; set; } = default!;
        public Guid ExamId { get; set; }
        public Guid? ExamQuestionId { get; set; }
        public Guid? AssessmentComponentId { get; set; }
        public int ItemNumber { get; set; }
        public int ExternalCloId { get; set; }
        public string? CloCode { get; set; }
        public decimal MappingWeight { get; set; }
        public decimal? AchievementRate { get; set; }
        public decimal? WeightedAchievement { get; set; }
        public int IncludedStudentCount { get; set; }
    }

    public class StudentEvaluationResultDto
    {
        public Guid Id { get; set; }
        public int ExternalStudentId { get; set; }
        public string? ExternalStudentNumber { get; set; }
        public string? ExternalStudentName { get; set; }
        public decimal? MidtermScore { get; set; }
        public decimal? FinalScore { get; set; }
        public decimal? MakeupScore { get; set; }
        public string UsedExamType { get; set; } = default!;
        public decimal? SuccessGrade { get; set; }
        public string? LetterGrade { get; set; }
        public bool IsPassed { get; set; }
        public bool IncludedInStatistics { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ExamEvaluationResultDto
    {
        public Guid Id { get; set; }
        public Guid ExamId { get; set; }
        public int ParticipantCount { get; set; }
        public int IncludedStudentCount { get; set; }
        public int PerfectScoreCount { get; set; }
        public decimal? MaxTotalScore { get; set; }
        public decimal? MinTotalScore { get; set; }
        public decimal? AverageTotalScore { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ExamQuestionEvaluationResultDto
    {
        public Guid Id { get; set; }
        public Guid ExamId { get; set; }
        public Guid? ExamQuestionId { get; set; }
        public Guid? AssessmentComponentId { get; set; }
        public string? ItemCaption { get; set; }
        public int QuestionNumber { get; set; }
        public decimal MaxScore { get; set; }
        public decimal? AverageScore { get; set; }
        public decimal? AchievementRate { get; set; }
        public int IncludedStudentCount { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CloEvaluationResultDto
    {
        public Guid Id { get; set; }
        public int ExternalCloId { get; set; }
        public string? CloCode { get; set; }
        public string? CloDescription { get; set; }
        public string ResultType { get; set; } = default!;
        public Guid? ExamId { get; set; }
        public decimal? AchievementScore { get; set; }
        public decimal? CombinedAchievementScore { get; set; }
        public decimal? SurveyScore { get; set; }
        public decimal? SurveyDifference { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ProgramOutcomeEvaluationResultDto
    {
        public Guid Id { get; set; }
        public int ExternalProgramOutcomeId { get; set; }
        public string? ProgramOutcomeCode { get; set; }
        public string? ProgramOutcomeCaption { get; set; }
        public decimal? AchievementScore { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
