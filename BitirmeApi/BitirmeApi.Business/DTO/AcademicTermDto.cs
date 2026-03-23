namespace BitirmeApi.Business.DTO
{
    public class AcademicTermListDto
    {
        public Guid Id { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public string TermType { get; set; } = default!;
        public string Name { get; set; } = default!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AcademicTermDto : AcademicTermListDto
    {
        public DateTime? UpdatedAt { get; set; }
        public int OfferingCount { get; set; }
    }

    public class AcademicTermCreateDto
    {
        public int StartYear { get; set; }
        public int EndYear { get; set; }

        /// <summary>"Guz", "Bahar", "Yaz"</summary>
        public string TermType { get; set; } = default!;

        /// <summary>Boş bırakılırsa otomatik üretilir: "{StartYear}-{EndYear} Güz/Bahar/Yaz"</summary>
        public string? Name { get; set; }
    }

    public class AcademicTermUpdateDto
    {
        public Guid Id { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public string TermType { get; set; } = default!;
        public string? Name { get; set; }
    }
}
