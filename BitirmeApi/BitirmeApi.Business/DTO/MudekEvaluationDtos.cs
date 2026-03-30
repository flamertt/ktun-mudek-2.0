namespace BitirmeApi.Business.DTO
{
    public class MudekEvaluationSnapshotDto
    {
        public Guid CourseOfferingId { get; set; }
        public Guid? CourseEvaluationId { get; set; }
        public DateTime? LastCalculatedAt { get; set; }
        public bool IsCalculationDirty { get; set; }
        public List<StudentEvaluationResultDto> StudentResults { get; set; } = new();
        public List<ExamEvaluationResultDto> ExamSummaries { get; set; } = new();
        public List<ExamQuestionEvaluationResultDto> QuestionAndComponentResults { get; set; } = new();

        /// <summary>
        /// Her yazılı soru veya sınav bileşeni için CLO eşlemesi + o öğe için başarı oranı (geçen öğrenciler).
        /// DOC matrisindeki hücre: <c>MappingWeight × AchievementRate</c> (<see cref="ExamAssessmentItemCloAchievementDto.WeightedAchievement"/>).
        /// </summary>
        public List<ExamAssessmentItemCloAchievementDto> ItemCloAchievements { get; set; } = new();

        public List<CloEvaluationResultDto> CloResults { get; set; } = new();
        public List<ProgramOutcomeEvaluationResultDto> ProgramOutcomeResults { get; set; } = new();
    }

    /// <summary>Sınavdaki ölçme öğesi (soru veya bileşen) başına CLO katkısı.</summary>
    public class ExamAssessmentItemCloAchievementDto
    {
        /// <summary><c>WrittenQuestion</c> veya <c>AssessmentComponent</c>.</summary>
        public string ItemType { get; set; } = default!;

        public Guid ExamId { get; set; }

        public Guid? ExamQuestionId { get; set; }

        public Guid? AssessmentComponentId { get; set; }

        /// <summary>Soru numarası veya bileşen sıra değeri (<see cref="ExamQuestionEvaluationResultDto.QuestionNumber"/> ile uyumlu).</summary>
        public int ItemNumber { get; set; }

        public Guid CourseLearningOutcomeId { get; set; }

        /// <summary>Soru–CLO veya bileşen–CLO eşlemesindeki ağırlık.</summary>
        public decimal MappingWeight { get; set; }

        /// <summary>Öğe bazında başarı oranı (ortalama / max); yalnız geçen öğrenciler.</summary>
        public decimal? AchievementRate { get; set; }

        /// <summary>DOC SUMPRODUCT terimi: <c>AchievementRate × MappingWeight</c>.</summary>
        public decimal? WeightedAchievement { get; set; }

        public int IncludedStudentCount { get; set; }
    }

    public class StudentEvaluationResultDto
    {
        public Guid Id { get; set; }
        public Guid EnrollmentId { get; set; }
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
        public Guid CourseLearningOutcomeId { get; set; }
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
        public Guid ProgramOutcomeId { get; set; }
        public decimal? AchievementScore { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
