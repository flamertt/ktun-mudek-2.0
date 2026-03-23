using AutoMapper;
using BitirmeApi.Business.Abstract;
using BitirmeApi.Business.DTO;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.Business.Concrete
{
    public class ProgramOutcomeService : IProgramOutcomeService
    {
        private readonly IProgramOutcomeDal _programOutcomeDal;
        private readonly IMapper _mapper;

        public ProgramOutcomeService(IProgramOutcomeDal programOutcomeDal, IMapper mapper)
        {
            _programOutcomeDal = programOutcomeDal;
            _mapper = mapper;
        }

        public async Task<List<ProgramOutcomeDto>> GetAllAsync()
        {
            var entities = await _programOutcomeDal.GetListAsync();
            return _mapper.Map<List<ProgramOutcomeDto>>(entities.ToList());
        }

        public async Task<List<ProgramOutcomeDto>> GetByProgramIdAsync(Guid programId)
        {
            var entities = await _programOutcomeDal.GetListAsync(po => po.ProgramEntityId == programId);
            return _mapper.Map<List<ProgramOutcomeDto>>(entities.ToList());
        }

        public async Task<ProgramOutcomeDto?> GetByIdAsync(Guid id)
        {
            var entity = await _programOutcomeDal.GetAsync(po => po.Id == id);
            return entity != null ? _mapper.Map<ProgramOutcomeDto>(entity) : null;
        }

        public async Task<ProgramOutcomeDto> CreateAsync(CreateProgramOutcomeDto dto)
        {
            var entity = _mapper.Map<ProgramOutcome>(dto);
            var added = _programOutcomeDal.Add(entity);
            await _programOutcomeDal.SaveChangesAsync();
            return _mapper.Map<ProgramOutcomeDto>(added);
        }

        public async Task<ProgramOutcomeDto> UpdateAsync(UpdateProgramOutcomeDto dto)
        {
            var existing = await _programOutcomeDal.GetAsync(po => po.Id == dto.Id);
            if (existing == null)
                throw new KeyNotFoundException($"Program çıktısı bulunamadı: {dto.Id}");

            _mapper.Map(dto, existing);
            _programOutcomeDal.Update(existing);
            await _programOutcomeDal.SaveChangesAsync();
            return _mapper.Map<ProgramOutcomeDto>(existing);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _programOutcomeDal.GetAsync(po => po.Id == id);
            if (entity == null)
                throw new KeyNotFoundException($"Program çıktısı bulunamadı: {id}");

            _programOutcomeDal.Delete(entity);
            await _programOutcomeDal.SaveChangesAsync();
        }
    }
}
