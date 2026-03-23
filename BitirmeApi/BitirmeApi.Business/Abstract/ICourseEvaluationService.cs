using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    public interface ICourseEvaluationService
    {
        // ── Okuma (Admin + Teacher) ───────────────────────────────────────────────
        Task<List<CourseEvaluationListDto>> GetAllAsync();
        Task<CourseEvaluationDetailDto?> GetByIdAsync(Guid id);
        Task<CourseEvaluationDetailDto?> GetByOfferingIdAsync(Guid courseOfferingId);

        // ── Yazma — sahiplik serviste doğrulanır ──────────────────────────────────

        /// <summary>
        /// Öğretmen kendi ders açılışı için değerlendirme oluşturur.
        /// Servis: CourseOffering.TeacherId != teacherId ise UnauthorizedAccessException fırlatır.
        /// Servis: Zaten varsa InvalidOperationException fırlatır.
        /// </summary>
        Task<CourseEvaluationDetailDto> CreateForTeacherAsync(CourseEvaluationCreateDto dto, Guid teacherId);

        /// <summary>
        /// Öğretmen kendi değerlendirmesini günceller.
        /// Servis: CourseOffering.TeacherId != teacherId ise UnauthorizedAccessException fırlatır.
        /// </summary>
        Task<CourseEvaluationDetailDto> UpdateForTeacherAsync(CourseEvaluationUpdateDto dto, Guid teacherId);

        /// <summary>
        /// Öğretmen kendi değerlendirmesini siler.
        /// Servis: CourseOffering.TeacherId != teacherId ise UnauthorizedAccessException fırlatır.
        /// </summary>
        Task DeleteForTeacherAsync(Guid id, Guid teacherId);
    }
}
