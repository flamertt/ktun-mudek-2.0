namespace BitirmeApi.Business.DTO
{
    public class ProgramEntityDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public int AccreditationCycleYears { get; set; }
    }

    public class CreateProgramEntityDto
    {
        public string Name { get; set; } = default!;
        public int AccreditationCycleYears { get; set; } = 5;
    }

    public class UpdateProgramEntityDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public int AccreditationCycleYears { get; set; }
    }
}
