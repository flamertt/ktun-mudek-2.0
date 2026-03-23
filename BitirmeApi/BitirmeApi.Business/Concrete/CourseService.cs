using AutoMapper;
using BitirmeApi.Business.Abstract;
using BitirmeApi.Business.DTO;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.Business.Concrete
{
    public class CourseService : ICourseService
    {
        private readonly ICourseDal _courseDal;
        private readonly IMapper _mapper;

        public CourseService(ICourseDal courseDal, IMapper mapper)
        {
            _courseDal = courseDal;
            _mapper = mapper;
        }

        public async Task<List<CourseDto>> GetAllAsync()
        {
            var entities = await _courseDal.GetAllWithDetailsAsync();
            return _mapper.Map<List<CourseDto>>(entities);
        }

        public async Task<List<CourseDto>> GetByProgramIdAsync(Guid programId)
        {
            var entities = await _courseDal.GetByProgramIdWithDetailsAsync(programId);
            return _mapper.Map<List<CourseDto>>(entities);
        }

        public async Task<CourseDto?> GetByIdAsync(Guid id)
        {
            var entity = await _courseDal.GetByIdWithDetailsAsync(id);
            return entity != null ? _mapper.Map<CourseDto>(entity) : null;
        }

        public async Task<CourseDto> CreateAsync(CreateCourseDto dto)
        {
            var entity = _mapper.Map<Course>(dto);
            var added = _courseDal.Add(entity);
            await _courseDal.SaveChangesAsync();
            var withDetails = await _courseDal.GetByIdWithDetailsAsync(added.Id);
            return _mapper.Map<CourseDto>(withDetails);
        }

        public async Task<CourseDto> UpdateAsync(UpdateCourseDto dto)
        {
            var existing = await _courseDal.GetAsync(c => c.Id == dto.Id);
            if (existing == null)
                throw new KeyNotFoundException($"Ders bulunamadı: {dto.Id}");

            _mapper.Map(dto, existing);
            _courseDal.Update(existing);
            await _courseDal.SaveChangesAsync();
            var updated = await _courseDal.GetByIdWithDetailsAsync(existing.Id);
            return _mapper.Map<CourseDto>(updated);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _courseDal.GetAsync(c => c.Id == id)
                ?? throw new KeyNotFoundException($"Ders bulunamadı: {id}");

            _courseDal.Delete(entity);
            await _courseDal.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid id) =>
            await _courseDal.GetAsync(c => c.Id == id) != null;

        public async Task<bool> IsActiveAsync(Guid id) =>
            await _courseDal.GetAsync(c => c.Id == id && c.IsActive) != null;
    }
}
