using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    public interface IAcademicTermService
    {
        Task<List<AcademicTermListDto>> GetAllAsync();
        Task<AcademicTermDto?> GetActiveAsync();
        Task<AcademicTermDto?> GetByIdAsync(Guid id);
        Task<AcademicTermDto> CreateAsync(AcademicTermCreateDto dto);
        Task<AcademicTermDto> UpdateAsync(AcademicTermUpdateDto dto);

        /// <summary>Belirtilen dönemi aktif yap, diğerlerini pasif et</summary>
        Task<AcademicTermDto> SetActiveAsync(Guid id);
        Task DeleteAsync(Guid id);

        // ── Validation helper ─────────────────────────────────────────────────────
        Task<bool> ExistsAsync(Guid id);
    }
}
