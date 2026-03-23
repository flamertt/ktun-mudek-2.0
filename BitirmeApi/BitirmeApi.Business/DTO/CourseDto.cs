namespace BitirmeApi.Business.DTO
{
    /// <summary>Ders kataloğu özet (liste)</summary>
    public class CourseListDto
    {
        public Guid Id { get; set; }
        public Guid ProgramEntityId { get; set; }
        public string? ProgramName { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public int Credits { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>Ders kataloğu tam detay</summary>
    public class CourseDto
    {
        public Guid Id { get; set; }
        public Guid ProgramEntityId { get; set; }
        public string? ProgramName { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public int Credits { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateCourseDto
    {
        public Guid ProgramEntityId { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public int Credits { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateCourseDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public int Credits { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
