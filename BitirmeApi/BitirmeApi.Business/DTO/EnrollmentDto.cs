namespace BitirmeApi.Business.DTO
{
    /// <summary>Öğrenci kayıt listesi (offering bazlı)</summary>
    public class EnrollmentListDto
    {
        public Guid Id { get; set; }
        public Guid CourseOfferingId { get; set; }
        public Guid StudentId { get; set; }
        public string StudentFullName { get; set; } = default!;
        public string? StudentNumber { get; set; }
        public string Status { get; set; } = default!;
        public DateTime EnrolledAt { get; set; }
    }

    /// <summary>Öğrencinin kendi kayıt geçmişi</summary>
    public class StudentEnrollmentHistoryDto
    {
        public Guid EnrollmentId { get; set; }
        public Guid CourseOfferingId { get; set; }
        public string CourseCode { get; set; } = default!;
        public string CourseName { get; set; } = default!;
        public string TermName { get; set; } = default!;
        public string? TeacherName { get; set; }
        public string Section { get; set; } = default!;
        public string Status { get; set; } = default!;
        public DateTime EnrolledAt { get; set; }
    }

    public class EnrollmentCreateDto
    {
        public Guid StudentId { get; set; }
    }

    public class EnrollmentBulkCreateDto
    {
        public List<Guid> StudentIds { get; set; } = new();
    }

    public class EnrollmentStatusUpdateDto
    {
        /// <summary>Enrolled, Passed, Failed, Withdrawn, Repeat</summary>
        public string Status { get; set; } = default!;
    }

    /// <summary>Excel / öğrenci numarası ile toplu kayıt sonucu</summary>
    public class EnrollmentImportResultDto
    {
        public int TotalRows { get; set; }
        public int Enrolled { get; set; }
        public List<string> AlreadyEnrolled { get; set; } = new();
        public List<string> NotFound { get; set; } = new();
        public List<string> FormatErrors { get; set; } = new();
        public string Message { get; set; } = default!;
    }

    /// <summary>
    /// GUID listesiyle toplu kayıt sonucu — her öğrencinin durumunu ayrı listede döner.
    /// </summary>
    public class BulkEnrollResultDto
    {
        /// <summary>Başarıyla kayıt edilen öğrenci Id'leri</summary>
        public List<Guid> Enrolled { get; set; } = new();

        /// <summary>Zaten kayıtlı olan öğrenci Id'leri</summary>
        public List<Guid> AlreadyEnrolled { get; set; } = new();

        /// <summary>Kullanıcı bulunamayan Id'ler</summary>
        public List<Guid> NotFound { get; set; } = new();

        /// <summary>Student rolünde olmayan kullanıcıların Id'leri</summary>
        public List<Guid> NotStudent { get; set; } = new();

        /// <summary>Pasif kullanıcıların Id'leri</summary>
        public List<Guid> InactiveUsers { get; set; } = new();

        public string Message =>
            $"{Enrolled.Count} kayıt eklendi. " +
            $"Zaten kayıtlı: {AlreadyEnrolled.Count}, " +
            $"Bulunamadı: {NotFound.Count}, " +
            $"Öğrenci rolünde değil: {NotStudent.Count}, " +
            $"Pasif kullanıcı: {InactiveUsers.Count}.";
    }
}
