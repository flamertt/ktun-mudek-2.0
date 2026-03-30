using AutoMapper;
using BitirmeApi.Business.Abstract;
using BitirmeApi.Business.DTO;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.Business.Concrete
{
    public class CloPoMapService : ICloPoMapService
    {
        private readonly ICloPoMapDal _mapDal;
        private readonly ICourseLearningOutcomeDal _cloDal;
        private readonly IProgramOutcomeDal _poDal;
        private readonly IMapper _mapper;
        private readonly IMudekEvaluationCalculatorService _mudekStale;

        public CloPoMapService(
            ICloPoMapDal mapDal,
            ICourseLearningOutcomeDal cloDal,
            IProgramOutcomeDal poDal,
            IMapper mapper,
            IMudekEvaluationCalculatorService mudekStale)
        {
            _mapDal = mapDal;
            _cloDal = cloDal;
            _poDal = poDal;
            _mapper = mapper;
            _mudekStale = mudekStale;
        }

        public async Task<List<CloPoMapDto>> GetByCloIdAsync(Guid cloId) =>
            _mapper.Map<List<CloPoMapDto>>(await _mapDal.GetByCloIdAsync(cloId));

        public async Task<List<CloPoMapDto>> GetByCourseIdAsync(Guid courseId) =>
            _mapper.Map<List<CloPoMapDto>>(await _mapDal.GetByCourseIdAsync(courseId));

        public async Task<CloPoMapDto> MapAsync(CreateCloPoMapDto dto)
        {
            // CLO var mı?
            if (!await _cloDal.ExistsAsync(dto.CourseLearningOutcomeId))
                throw new KeyNotFoundException("Belirtilen ders öğrenim çıktısı (CLO) bulunamadı.");

            // Program çıktısı var mı?
            if (await _poDal.GetAsync(p => p.Id == dto.ProgramOutcomeId) == null)
                throw new KeyNotFoundException("Belirtilen program çıktısı (PO) bulunamadı.");

            // Zaten eşlenmiş mi?
            if (await _mapDal.ExistsAsync(dto.CourseLearningOutcomeId, dto.ProgramOutcomeId))
                throw new InvalidOperationException(
                    "Bu CLO ile Program Çıktısı zaten eşleştirilmiş.");

            // Ağırlık 0-1 arasında olmalı
            if (dto.Weight < 0 || dto.Weight > 1)
                throw new InvalidOperationException("Ağırlık değeri 0 ile 1 arasında olmalıdır.");

            var entity = new CloPoMap
            {
                Id = Guid.NewGuid(),
                CourseLearningOutcomeId = dto.CourseLearningOutcomeId,
                ProgramOutcomeId = dto.ProgramOutcomeId,
                Weight = dto.Weight
            };

            _mapDal.Add(entity);
            await _mapDal.SaveChangesAsync();
            await _mudekStale.MarkStaleByCourseLearningOutcomeIdAsync(dto.CourseLearningOutcomeId);

            // Detaylarıyla çek (CLO ve PO include)
            var maps = await _mapDal.GetByCloIdAsync(dto.CourseLearningOutcomeId);
            return _mapper.Map<CloPoMapDto>(maps.First(m => m.ProgramOutcomeId == dto.ProgramOutcomeId));
        }

        public async Task<CloPoMapDto> UpdateWeightAsync(Guid cloId, Guid programOutcomeId, decimal weight)
        {
            var entity = await _mapDal.GetByIdsAsync(cloId, programOutcomeId)
                ?? throw new KeyNotFoundException("Belirtilen CLO → PO eşlemesi bulunamadı.");

            if (weight < 0 || weight > 1)
                throw new InvalidOperationException("Ağırlık değeri 0 ile 1 arasında olmalıdır.");

            entity.Weight = weight;
            _mapDal.Update(entity);
            await _mapDal.SaveChangesAsync();
            await _mudekStale.MarkStaleByCourseLearningOutcomeIdAsync(cloId);

            var maps = await _mapDal.GetByCloIdAsync(cloId);
            return _mapper.Map<CloPoMapDto>(maps.First(m => m.ProgramOutcomeId == programOutcomeId));
        }

        public async Task UnmapAsync(Guid cloId, Guid programOutcomeId)
        {
            var entity = await _mapDal.GetByIdsAsync(cloId, programOutcomeId)
                ?? throw new KeyNotFoundException("Belirtilen CLO → PO eşlemesi bulunamadı.");

            _mapDal.Delete(entity);
            await _mapDal.SaveChangesAsync();
            await _mudekStale.MarkStaleByCourseLearningOutcomeIdAsync(cloId);
        }
    }
}
