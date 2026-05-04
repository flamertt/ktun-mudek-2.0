namespace BitirmeApi.Business.DTO
{
    public class CreateLetterGradeRuleDto
    {
        public int ExternalProgramId { get; set; }
        public string LetterGrade { get; set; } = default!;
        public decimal MinScore { get; set; }
        public decimal MaxScore { get; set; }
        public bool IsPassing { get; set; } = true;
        public decimal? MinimumFinalScore { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateLetterGradeRuleDto
    {
        public Guid Id { get; set; }
        public string LetterGrade { get; set; } = default!;
        public decimal MinScore { get; set; }
        public decimal MaxScore { get; set; }
        public bool IsPassing { get; set; } = true;
        public decimal? MinimumFinalScore { get; set; }
        public string? Description { get; set; }
    }

    public class LetterGradeRuleDto
    {
        public Guid Id { get; set; }
        public int ExternalProgramId { get; set; }
        public string LetterGrade { get; set; } = default!;
        public decimal MinScore { get; set; }
        public decimal MaxScore { get; set; }
        public bool IsPassing { get; set; }
        public decimal? MinimumFinalScore { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
