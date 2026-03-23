namespace BitirmeApi.Business.DTO
{
    public class CreateTeacherDto
    {
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Password { get; set; }
        public string? Title { get; set; }
        public string? PhoneNumber { get; set; }
        public Guid ProgramEntityId { get; set; }
    }

    public class CreateStudentDto
    {
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Password { get; set; }
        public string? StudentNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public Guid ProgramEntityId { get; set; }
    }

    public class UpdateTeacherDto
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Title { get; set; }
        public string? PhoneNumber { get; set; }
        public Guid? ProgramEntityId { get; set; }
        public bool? IsActive { get; set; }
    }

    public class UpdateStudentDto
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? StudentNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public Guid? ProgramEntityId { get; set; }
        public bool? IsActive { get; set; }
    }
}
