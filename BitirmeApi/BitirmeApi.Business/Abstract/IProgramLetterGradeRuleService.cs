using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    public interface IProgramLetterGradeRuleService
    {
        Task<List<ProgramLetterGradeRuleDto>> GetByProgramIdAsync(Guid programEntityId);
        Task<ProgramLetterGradeRuleDto> AddAsync(CreateProgramLetterGradeRuleDto dto);
        Task<ProgramLetterGradeRuleDto> UpdateAsync(UpdateProgramLetterGradeRuleDto dto);
        Task DeleteAsync(Guid id);

        /// <summary>Öğretmenin ders açılışı için geçerli kurallar (önce program, yoksa değerlendirme).</summary>
        Task<EffectiveLetterGradeRulesResponseDto> GetEffectiveForTeacherOfferingAsync(Guid offeringId, Guid teacherId);
    }
}
