using System.ComponentModel.DataAnnotations;

namespace BitirmeApi.Business.DTO
{
    // ── Anket Listeleme ───────────────────────────────────────────────────────────
    public class SurveyListDto
    {
        public Guid Id { get; set; }
        public Guid CourseOfferingId { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int QuestionCount { get; set; }
        public int SubmissionCount { get; set; }
    }

    // ── Anket Detayı (sorular dahil) ──────────────────────────────────────────────
    public class SurveyDetailDto
    {
        public Guid Id { get; set; }
        public Guid CourseOfferingId { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int SubmissionCount { get; set; }
        public List<SurveyQuestionDto> Questions { get; set; } = new();
    }

    // ── Anket Oluşturma ───────────────────────────────────────────────────────────
    public class CreateSurveyDto
    {
        [Required]
        public Guid CourseOfferingId { get; set; }

        [Required, MaxLength(256)]
        public string Title { get; set; } = default!;

        [MaxLength(2000)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }

    // ── Anket Güncelleme ──────────────────────────────────────────────────────────
    public class UpdateSurveyDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required, MaxLength(256)]
        public string Title { get; set; } = default!;

        [MaxLength(2000)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }

    // ── Soru DTO (Likert + DÖÇ) ──────────────────────────────────────────────────
    public class SurveyQuestionDto
    {
        public Guid Id { get; set; }
        public Guid SurveyId { get; set; }
        public string Text { get; set; } = default!;
        public int OrderIndex { get; set; }
        public bool IsRequired { get; set; }
        public int ScaleMin { get; set; }
        public int ScaleMax { get; set; }

        /// <summary>Eşlenmiş DÖÇ (opsiyonel).</summary>
        public Guid? CourseLearningOutcomeId { get; set; }

        /// <summary>DÖÇ kodu (ör. "ÖÇ1"). Eşleme yoksa null.</summary>
        public string? CloCode { get; set; }

        /// <summary>DÖÇ açıklaması. Eşleme yoksa null.</summary>
        public string? CloDescription { get; set; }
    }

    // ── Soru Oluşturma (Likert) ───────────────────────────────────────────────────
    public class CreateSurveyQuestionDto
    {
        [Required]
        public Guid SurveyId { get; set; }

        [Required, MaxLength(1000)]
        public string Text { get; set; } = default!;

        public int OrderIndex { get; set; } = 1;
        public bool IsRequired { get; set; } = true;

        /// <summary>Likert ölçeği başlangıç değeri (varsayılan 0).</summary>
        [Range(0, 9)]
        public int ScaleMin { get; set; } = 0;

        /// <summary>Likert ölçeği bitiş değeri (varsayılan 5).</summary>
        [Range(1, 10)]
        public int ScaleMax { get; set; } = 5;

        /// <summary>İsteğe bağlı DÖÇ eşlemesi. Anketin dersiyle aynı kursa ait olmalıdır.</summary>
        public Guid? CourseLearningOutcomeId { get; set; }
    }

    // ── Soru Güncelleme ───────────────────────────────────────────────────────────
    public class UpdateSurveyQuestionDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required, MaxLength(1000)]
        public string Text { get; set; } = default!;

        public int OrderIndex { get; set; }
        public bool IsRequired { get; set; }

        [Range(0, 9)]
        public int ScaleMin { get; set; }

        [Range(1, 10)]
        public int ScaleMax { get; set; }

        /// <summary>
        /// İsteğe bağlı DÖÇ eşlemesi. Eşlemeyi kaldırmak için null gönderin.
        /// </summary>
        public Guid? CourseLearningOutcomeId { get; set; }
    }

    // ── DÖÇ Bazlı Anket Skoru + MÜDEK Karşılaştırması ────────────────────────────
    public class CloSurveyResultDto
    {
        public Guid CloId { get; set; }
        public string CloCode { get; set; } = default!;
        public string CloDescription { get; set; } = default!;

        /// <summary>Bu DÖÇ'e bağlı soru sayısı.</summary>
        public int QuestionCount { get; set; }

        /// <summary>Anket skoru (0-100). Bağlı soruların yüzdelerinin ortalaması.</summary>
        public decimal? SurveyScore { get; set; }

        /// <summary>MÜDEK sınav tabanlı DÖÇ başarısı (0-100). Combined ResultType'tan.</summary>
        public decimal? MudekScore { get; set; }

        /// <summary>Fark: SurveyScore - MudekScore. Her ikisi de mevcutsa dolu.</summary>
        public decimal? Difference { get; set; }

        /// <summary>
        /// Değerlendirme metni:
        /// |Fark| ≤ 10 → "Uyumlu"
        /// Fark > 10  → "Anket Yüksek"
        /// Fark &lt; -10 → "Anket Düşük"
        /// </summary>
        public string? Evaluation { get; set; }
    }

    // ── Soru Bazlı Sonuç ──────────────────────────────────────────────────────────
    public class SurveyQuestionResultDto
    {
        public Guid QuestionId { get; set; }
        public string Text { get; set; } = default!;
        public int OrderIndex { get; set; }
        public Guid? CourseLearningOutcomeId { get; set; }
        public string? CloCode { get; set; }

        /// <summary>0 hariç yanıt sayısı (ortalama hesabına giren).</summary>
        public int ResponseCount { get; set; }

        /// <summary>Soru ortalaması (0 hariç). null → hiç yanıt yok.</summary>
        public decimal? AverageScore { get; set; }

        /// <summary>Soru yüzdesi = (Ortalama / ScaleMax) * 100.</summary>
        public decimal? ScorePercentage { get; set; }

        /// <summary>Her puan değeri için kaç yanıt (0→n, 1→n, …).</summary>
        public Dictionary<int, int> ScoreDistribution { get; set; } = new();
    }

    // ── Anket Sonuçları ───────────────────────────────────────────────────────────
    public class SurveyResultsDto
    {
        public Guid SurveyId { get; set; }
        public string Title { get; set; } = default!;

        // ── Katılım istatistikleri ─────────────────────────────────────────────────

        /// <summary>Ders açılışına kayıtlı toplam öğrenci sayısı.</summary>
        public int EnrolledStudentCount { get; set; }

        /// <summary>Anketi dolduran öğrenci sayısı (MÜDEK filtresi uygulanmadan önce).</summary>
        public int TotalSubmissions { get; set; }

        /// <summary>
        /// Hesaplamalara dahil edilen gönderi sayısı.
        /// IsPassingFilterApplied = true ise sadece dersten geçenlerin yanıtları sayılır.
        /// </summary>
        public int EvaluatedSubmissions { get; set; }

        /// <summary>Kayıtlı olmasına rağmen ankete katılmayan öğrenci sayısı.</summary>
        public int NotParticipatedCount { get; set; }

        /// <summary>
        /// Anketi doldurduğu hâlde dersten geçemediği için hesaba katılmayan öğrenci sayısı.
        /// Yalnızca IsPassingFilterApplied = true ise anlamlıdır.
        /// </summary>
        public int SubmittedButExcludedCount { get; set; }

        /// <summary>
        /// True → sadece dersten geçen öğrencilerin yanıtları hesaba katıldı.
        /// False → MÜDEK hesabı henüz yapılmamış; tüm yanıtlar kullanıldı.
        /// </summary>
        public bool IsPassingFilterApplied { get; set; }

        public List<SurveyQuestionResultDto> Questions { get; set; } = new();

        /// <summary>DÖÇ bazlı skor ve MÜDEK karşılaştırması (sadece DÖÇ eşlenmiş sorular için).</summary>
        public List<CloSurveyResultDto> CloResults { get; set; } = new();
    }
}
