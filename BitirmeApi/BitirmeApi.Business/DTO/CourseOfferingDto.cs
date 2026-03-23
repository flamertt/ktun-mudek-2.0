namespace BitirmeApi.Business.DTO
{
    /// <summary>Dönemlik açılış özet (liste)</summary>
    public class CourseOfferingListDto
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public string CourseCode { get; set; } = default!;
        public string CourseName { get; set; } = default!;
        public string? ProgramName { get; set; }
        public Guid AcademicTermId { get; set; }
        public string TermName { get; set; } = default!;
        public Guid? TeacherId { get; set; }
        public string? TeacherName { get; set; }
        public string Section { get; set; } = "A";
        public bool IsActive { get; set; }
        public int EnrolledCount { get; set; }
    }

    /// <summary>Dönemlik açılış tam detay</summary>
    public class CourseOfferingDto
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public string CourseCode { get; set; } = default!;
        public string CourseName { get; set; } = default!;
        public int Credits { get; set; }
        public string? CourseDescription { get; set; }
        public Guid ProgramEntityId { get; set; }
        public string? ProgramName { get; set; }
        public Guid AcademicTermId { get; set; }
        public string TermName { get; set; } = default!;
        public Guid? TeacherId { get; set; }
        public string? TeacherName { get; set; }
        public string Section { get; set; } = "A";
        public decimal? PassingGrade { get; set; }
        public int? Quota { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int EnrolledCount { get; set; }
        public bool HasEvaluation { get; set; }
    }

    public class CourseOfferingCreateDto
    {
        public Guid CourseId { get; set; }
        public Guid AcademicTermId { get; set; }
        public Guid? TeacherId { get; set; }

        /// <summary>Şube kodu. Yoksa "A" bırak.</summary>
        public string Section { get; set; } = "A";
        public decimal? PassingGrade { get; set; }
        public int? Quota { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class CourseOfferingUpdateDto
    {
        public Guid Id { get; set; }
        public Guid? TeacherId { get; set; }
        public string Section { get; set; } = "A";
        public decimal? PassingGrade { get; set; }
        public int? Quota { get; set; }
        public bool IsActive { get; set; }
    }

    public class AssignTeacherDto
    {
        public Guid TeacherId { get; set; }
    }
}
