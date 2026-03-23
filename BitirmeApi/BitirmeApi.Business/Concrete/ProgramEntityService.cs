using AutoMapper;
using BitirmeApi.Business.Abstract;
using BitirmeApi.Business.DTO;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.Business.Concrete
{
    public class ProgramEntityService : IProgramEntityService
    {
        private readonly IProgramEntityDal _programEntityDal;
        private readonly IMapper _mapper;

        public ProgramEntityService(IProgramEntityDal programEntityDal, IMapper mapper)
        {
            _programEntityDal = programEntityDal;
            _mapper = mapper;
        }

        public async Task<List<ProgramEntityDto>> GetAllAsync()
        {
            var entities = await _programEntityDal.GetListAsync();
            return _mapper.Map<List<ProgramEntityDto>>(entities.ToList());
        }

        public async Task<ProgramEntityDto?> GetByIdAsync(Guid id)
        {
            var entity = await _programEntityDal.GetAsync(p => p.Id == id);
            return entity != null ? _mapper.Map<ProgramEntityDto>(entity) : null;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _programEntityDal.ExistsAsync(id);
        }

        public async Task<ProgramEntityDto> CreateAsync(CreateProgramEntityDto dto)
        {
            var entity = _mapper.Map<ProgramEntity>(dto);
            var added = _programEntityDal.Add(entity);
            await _programEntityDal.SaveChangesAsync();
            return _mapper.Map<ProgramEntityDto>(added);
        }

        public async Task<ProgramEntityDto> UpdateAsync(UpdateProgramEntityDto dto)
        {
            var existing = await _programEntityDal.GetAsync(p => p.Id == dto.Id);
            if (existing == null)
                throw new KeyNotFoundException($"Program bulunamadı: {dto.Id}");

            _mapper.Map(dto, existing);
            _programEntityDal.Update(existing);
            await _programEntityDal.SaveChangesAsync();
            return _mapper.Map<ProgramEntityDto>(existing);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _programEntityDal.GetAsync(p => p.Id == id);
            if (entity == null)
                throw new KeyNotFoundException($"Program bulunamadı: {id}");

            _programEntityDal.Delete(entity);
            await _programEntityDal.SaveChangesAsync();
        }
    }
}
