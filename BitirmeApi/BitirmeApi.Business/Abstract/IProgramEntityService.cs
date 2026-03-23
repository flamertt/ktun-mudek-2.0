using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    public interface IProgramEntityService
    {
        Task<List<ProgramEntityDto>> GetAllAsync();
        Task<ProgramEntityDto?> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<ProgramEntityDto> CreateAsync(CreateProgramEntityDto dto);
        Task<ProgramEntityDto> UpdateAsync(UpdateProgramEntityDto dto);
        Task DeleteAsync(Guid id);
    }
}
