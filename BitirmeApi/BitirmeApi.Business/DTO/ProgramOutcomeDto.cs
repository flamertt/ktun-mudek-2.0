namespace BitirmeApi.Business.DTO
{
    public class ProgramOutcomeDto
    {
        public Guid Id { get; set; }
        public Guid ProgramEntityId { get; set; }
        public string Code { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
    }

    public class CreateProgramOutcomeDto
    {
        public Guid ProgramEntityId { get; set; }
        public string Code { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
    }

    public class UpdateProgramOutcomeDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
    }
}
