using System.ComponentModel.DataAnnotations;

namespace BitirmeApi.Business.DTO
{
    // ── Öğrencinin aktif dönem dersleri ──────────────────────────────────────────
    public class StudentCourseDto
    {
        public Guid CourseOfferingId { get; set; }
        public string CourseCode { get; set; } = default!;
        public string CourseName { get; set; } = default!;
        public string? TeacherName { get; set; }
        public string Section { get; set; } = default!;
        public string TermName { get; set; } = default!;

        /// <summary>Bu derse ait aktif anket sayısı.</summary>
        public int ActiveSurveyCount { get; set; }
    }

    // ── Öğrenci için anket listesi ────────────────────────────────────────────────
    public class StudentSurveyListDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public int QuestionCount { get; set; }

        /// <summary>Öğrenci bu ankete daha önce katıldı mı?</summary>
        public bool HasSubmitted { get; set; }
    }

    // ── Anketi doldurmak için detay ───────────────────────────────────────────────
    public class StudentSurveyDetailDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }

        /// <summary>Öğrenci bu ankete daha önce katıldı mı? True ise tekrar gönderim engellenecek.</summary>
        public bool HasSubmitted { get; set; }

        public List<StudentSurveyQuestionDto> Questions { get; set; } = new();
    }

    // ── Soru (öğrenci görünümü) ───────────────────────────────────────────────────
    public class StudentSurveyQuestionDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = default!;
        public int OrderIndex { get; set; }
        public bool IsRequired { get; set; }
        public int ScaleMin { get; set; }
        public int ScaleMax { get; set; }
    }

    // ── Anket gönderimi (input) ───────────────────────────────────────────────────
    public class SubmitSurveyDto
    {
        [Required, MinLength(1, ErrorMessage = "En az bir cevap gereklidir.")]
        public List<SurveyAnswerInputDto> Answers { get; set; } = new();
    }

    public class SurveyAnswerInputDto
    {
        [Required]
        public Guid QuestionId { get; set; }

        /// <summary>
        /// Likert değeri. Sorunun ScaleMin–ScaleMax aralığında olmalıdır.
        /// 0 gönderilebilir (cevapsız bırakma).
        /// </summary>
        [Required, Range(0, 10)]
        public decimal ValueNumeric { get; set; }
    }

    // ── Gönderim sonucu ───────────────────────────────────────────────────────────
    public class StudentSubmissionResultDto
    {
        public Guid SubmissionId { get; set; }
        public Guid SurveyId { get; set; }
        public int AnsweredQuestions { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
