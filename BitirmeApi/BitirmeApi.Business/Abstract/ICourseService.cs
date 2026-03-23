using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    public interface ICourseService
    {
        Task<List<CourseDto>> GetAllAsync();
        Task<List<CourseDto>> GetByProgramIdAsync(Guid programId);
        Task<CourseDto?> GetByIdAsync(Guid id);
        Task<CourseDto> CreateAsync(CreateCourseDto dto);
        Task<CourseDto> UpdateAsync(UpdateCourseDto dto);
        Task DeleteAsync(Guid id);

        // ── Validation helper ─────────────────────────────────────────────────────
        Task<bool> ExistsAsync(Guid id);
        Task<bool> IsActiveAsync(Guid id);
    }
}
