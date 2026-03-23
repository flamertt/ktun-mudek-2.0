using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    public interface ICourseOfferingService
    {
        // ── Sorgular ──────────────────────────────────────────────────────────────
        Task<List<CourseOfferingListDto>> GetAllAsync();
        Task<List<CourseOfferingListDto>> GetByTermIdAsync(Guid termId);
        Task<List<CourseOfferingListDto>> GetByActiveTermAsync();
        Task<List<CourseOfferingListDto>> GetByTeacherIdAsync(Guid teacherId);
        Task<List<CourseOfferingListDto>> GetByTeacherIdAndTermAsync(Guid teacherId, Guid? termId = null);
        Task<List<CourseOfferingListDto>> GetByCourseIdAsync(Guid courseId);
        Task<CourseOfferingDto?> GetByIdAsync(Guid id);
        Task<CourseOfferingDto?> GetByIdForTeacherAsync(Guid offeringId, Guid teacherId);

        // ── Yazma — tüm iş kuralı doğrulamaları servis içinde ────────────────────
        Task<CourseOfferingDto> CreateAsync(CourseOfferingCreateDto dto);
        Task<CourseOfferingDto> UpdateAsync(CourseOfferingUpdateDto dto);
        Task<CourseOfferingDto> AssignTeacherAsync(Guid offeringId, Guid teacherId);
        Task<CourseOfferingDto> RemoveTeacherAsync(Guid offeringId);
        Task DeleteAsync(Guid id);

        // ── Validation helper ─────────────────────────────────────────────────────
        Task<bool> ExistsAsync(Guid id);
    }
}
