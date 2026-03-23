using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    public interface IProgramOutcomeService
    {
        Task<List<ProgramOutcomeDto>> GetAllAsync();
        Task<List<ProgramOutcomeDto>> GetByProgramIdAsync(Guid programId);
        Task<ProgramOutcomeDto?> GetByIdAsync(Guid id);
        Task<ProgramOutcomeDto> CreateAsync(CreateProgramOutcomeDto dto);
        Task<ProgramOutcomeDto> UpdateAsync(UpdateProgramOutcomeDto dto);
        Task DeleteAsync(Guid id);
    }
}
