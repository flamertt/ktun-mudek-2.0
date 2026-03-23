using AutoMapper;
using BitirmeApi.Business.Abstract;
using BitirmeApi.Business.DTO;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.Business.Concrete
{
    public class CourseLearningOutcomeService : ICourseLearningOutcomeService
    {
        private readonly ICourseLearningOutcomeDal _cloDal;
        private readonly ICourseDal _courseDal;
        private readonly IMapper _mapper;

        public CourseLearningOutcomeService(
            ICourseLearningOutcomeDal cloDal,
            ICourseDal courseDal,
            IMapper mapper)
        {
            _cloDal = cloDal;
            _courseDal = courseDal;
            _mapper = mapper;
        }

        public async Task<List<CourseLearningOutcomeDto>> GetByCourseIdAsync(Guid courseId) =>
            _mapper.Map<List<CourseLearningOutcomeDto>>(await _cloDal.GetByCourseIdAsync(courseId));

        public async Task<CourseLearningOutcomeDto?> GetByIdAsync(Guid id)
        {
            var entity = await _cloDal.GetByIdWithDetailsAsync(id);
            return entity != null ? _mapper.Map<CourseLearningOutcomeDto>(entity) : null;
        }

        public async Task<CourseLearningOutcomeDto> CreateAsync(CreateCourseLearningOutcomeDto dto)
        {
            // Ders var mı?
            if (await _courseDal.GetAsync(c => c.Id == dto.CourseId) == null)
                throw new KeyNotFoundException("Belirtilen ders bulunamadı.");

            // Aynı derse aynı kod ile CLO eklenmiş mi?
            if (await _cloDal.CodeExistsForCourseAsync(dto.CourseId, dto.Code))
                throw new InvalidOperationException(
                    $"Bu ders için '{dto.Code}' kodlu bir öğrenim çıktısı zaten mevcut.");

            var entity = _mapper.Map<CourseLearningOutcome>(dto);
            entity.Id = Guid.NewGuid();

            _cloDal.Add(entity);
            await _cloDal.SaveChangesAsync();

            return _mapper.Map<CourseLearningOutcomeDto>((await _cloDal.GetByIdWithDetailsAsync(entity.Id))!);
        }

        public async Task<CourseLearningOutcomeDto> UpdateAsync(UpdateCourseLearningOutcomeDto dto)
        {
            var entity = await _cloDal.GetAsync(c => c.Id == dto.Id)
                ?? throw new KeyNotFoundException("Öğrenim çıktısı bulunamadı.");

            // Code değiştiyse aynı derse ait başka CLO ile çakışıyor mu?
            if (entity.Code != dto.Code &&
                await _cloDal.CodeExistsForCourseAsync(entity.CourseId, dto.Code, dto.Id))
            {
                throw new InvalidOperationException(
                    $"Bu ders için '{dto.Code}' kodlu bir öğrenim çıktısı zaten mevcut.");
            }

            entity.Code = dto.Code;
            entity.Description = dto.Description;
            entity.OrderIndex = dto.OrderIndex;

            _cloDal.Update(entity);
            await _cloDal.SaveChangesAsync();

            return _mapper.Map<CourseLearningOutcomeDto>((await _cloDal.GetByIdWithDetailsAsync(entity.Id))!);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _cloDal.GetAsync(c => c.Id == id)
                ?? throw new KeyNotFoundException("Öğrenim çıktısı bulunamadı.");

            _cloDal.Delete(entity);
            await _cloDal.SaveChangesAsync();
        }

        public Task<bool> ExistsAsync(Guid id) => _cloDal.ExistsAsync(id);
    }
}
