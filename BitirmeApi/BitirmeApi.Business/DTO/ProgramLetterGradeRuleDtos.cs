namespace BitirmeApi.Business.DTO
{
    public class ProgramLetterGradeRuleDto
    {
        public Guid Id { get; set; }
        public Guid ProgramEntityId { get; set; }
        public string LetterGrade { get; set; } = default!;
        public decimal MinScore { get; set; }
        public decimal MaxScore { get; set; }
        public bool IsPassing { get; set; }
        public decimal? MinimumFinalScore { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateProgramLetterGradeRuleDto
    {
        public Guid ProgramEntityId { get; set; }
        public string LetterGrade { get; set; } = default!;
        public decimal MinScore { get; set; }
        public decimal MaxScore { get; set; }
        public bool IsPassing { get; set; } = true;
        public decimal? MinimumFinalScore { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateProgramLetterGradeRuleDto
    {
        public Guid Id { get; set; }
        public string LetterGrade { get; set; } = default!;
        public decimal MinScore { get; set; }
        public decimal MaxScore { get; set; }
        public bool IsPassing { get; set; }
        public decimal? MinimumFinalScore { get; set; }
        public string? Description { get; set; }
    }

    /// <summary>Öğretmen paneli: hangi kural setinin kullanıldığını belirtir.</summary>
    public class EffectiveLetterGradeRulesResponseDto
    {
        /// <summary>"Program" veya "CourseEvaluation" (yalnızca programda kural yoksa).</summary>
        public string Source { get; set; } = default!;
        public List<ProgramLetterGradeRuleDto> Rules { get; set; } = new();
    }
}
