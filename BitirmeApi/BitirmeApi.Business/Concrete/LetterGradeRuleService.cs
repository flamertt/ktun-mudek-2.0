using AutoMapper;
using BitirmeApi.Business.Abstract;
using BitirmeApi.Business.DTO;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.Business.Concrete
{
    public class LetterGradeRuleService : ILetterGradeRuleService
    {
        private readonly ILetterGradeRuleDal _dal;
        private readonly IMapper _mapper;

        public LetterGradeRuleService(ILetterGradeRuleDal dal, IMapper mapper)
        {
            _dal = dal;
            _mapper = mapper;
        }

        public async Task<List<LetterGradeRuleDto>> GetAllAsync()
        {
            var list = await _dal.GetListAsync();
            return _mapper.Map<List<LetterGradeRuleDto>>(list.OrderByDescending(r => r.MaxScore).ToList());
        }

        public async Task<List<LetterGradeRuleDto>> GetByProgramIdAsync(int externalProgramId)
        {
            var list = await _dal.GetByProgramIdAsync(externalProgramId);
            return _mapper.Map<List<LetterGradeRuleDto>>(list);
        }

        public async Task<LetterGradeRuleDto?> GetByIdAsync(Guid id)
        {
            var entity = await _dal.GetAsync(r => r.Id == id);
            return entity != null ? _mapper.Map<LetterGradeRuleDto>(entity) : null;
        }

        public async Task<LetterGradeRuleDto> AddAsync(CreateLetterGradeRuleDto dto)
        {
            if (dto.MinScore > dto.MaxScore)
                throw new InvalidOperationException("MinScore, MaxScore'dan büyük olamaz.");

            if (await _dal.ExistsLetterAsync(dto.ExternalProgramId, dto.LetterGrade))
                throw new InvalidOperationException($"Bu program için '{dto.LetterGrade}' harf notu zaten tanımlı.");

            var entity = _mapper.Map<LetterGradeRule>(dto);
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = DateTime.UtcNow;
            _dal.Add(entity);
            await _dal.SaveChangesAsync();
            return _mapper.Map<LetterGradeRuleDto>(entity);
        }

        public async Task<LetterGradeRuleDto> UpdateAsync(UpdateLetterGradeRuleDto dto)
        {
            if (dto.MinScore > dto.MaxScore)
                throw new InvalidOperationException("MinScore, MaxScore'dan büyük olamaz.");

            var entity = await _dal.GetAsync(r => r.Id == dto.Id)
                ?? throw new KeyNotFoundException("Harf notu kuralı bulunamadı.");

            if (await _dal.ExistsLetterAsync(entity.ExternalProgramId, dto.LetterGrade, dto.Id))
                throw new InvalidOperationException($"Bu program için '{dto.LetterGrade}' harf notu zaten tanımlı.");

            entity.LetterGrade = dto.LetterGrade;
            entity.MinScore = dto.MinScore;
            entity.MaxScore = dto.MaxScore;
            entity.IsPassing = dto.IsPassing;
            entity.MinimumFinalScore = dto.MinimumFinalScore;
            entity.Description = dto.Description;
            _dal.Update(entity);
            await _dal.SaveChangesAsync();
            return _mapper.Map<LetterGradeRuleDto>(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _dal.GetAsync(r => r.Id == id)
                ?? throw new KeyNotFoundException("Harf notu kuralı bulunamadı.");
            _dal.Delete(entity);
            await _dal.SaveChangesAsync();
        }
    }
}
