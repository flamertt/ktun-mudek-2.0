using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    public interface ILetterGradeRuleService
    {
        Task<List<LetterGradeRuleDto>> GetAllAsync();
        Task<List<LetterGradeRuleDto>> GetByProgramIdAsync(int externalProgramId);
        Task<LetterGradeRuleDto?> GetByIdAsync(Guid id);
        Task<LetterGradeRuleDto> AddAsync(CreateLetterGradeRuleDto dto);
        Task<LetterGradeRuleDto> UpdateAsync(UpdateLetterGradeRuleDto dto);
        Task DeleteAsync(Guid id);
    }
}
