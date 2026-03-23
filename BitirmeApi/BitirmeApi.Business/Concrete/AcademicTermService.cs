using AutoMapper;
using BitirmeApi.Business.Abstract;
using BitirmeApi.Business.DTO;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.Business.Concrete
{
    public class AcademicTermService : IAcademicTermService
    {
        private readonly IAcademicTermDal _termDal;
        private readonly IMapper _mapper;

        public AcademicTermService(IAcademicTermDal termDal, IMapper mapper)
        {
            _termDal = termDal;
            _mapper = mapper;
        }

        public async Task<List<AcademicTermListDto>> GetAllAsync()
        {
            var list = await _termDal.GetListAsync(_ => true);
            return _mapper.Map<List<AcademicTermListDto>>(list);
        }

        public async Task<AcademicTermDto?> GetActiveAsync()
        {
            var term = await _termDal.GetActiveTermAsync();
            return term != null ? _mapper.Map<AcademicTermDto>(term) : null;
        }

        public async Task<AcademicTermDto?> GetByIdAsync(Guid id)
        {
            var term = await _termDal.GetAsync(t => t.Id == id);
            return term != null ? _mapper.Map<AcademicTermDto>(term) : null;
        }

        public async Task<AcademicTermDto> CreateAsync(AcademicTermCreateDto dto)
        {
            var entity = new AcademicTerm
            {
                Id = Guid.NewGuid(),
                StartYear = dto.StartYear,
                EndYear = dto.EndYear,
                TermType = dto.TermType,
                CreatedAt = DateTime.UtcNow
            };
            entity.Name = string.IsNullOrWhiteSpace(dto.Name)
                ? BuildTermName(dto.StartYear, dto.EndYear, dto.TermType)
                : dto.Name;

            _termDal.Add(entity);
            await _termDal.SaveChangesAsync();
            return _mapper.Map<AcademicTermDto>(entity);
        }

        public async Task<AcademicTermDto> UpdateAsync(AcademicTermUpdateDto dto)
        {
            var entity = await _termDal.GetAsync(t => t.Id == dto.Id)
                ?? throw new KeyNotFoundException($"Dönem bulunamadı: {dto.Id}");

            entity.StartYear = dto.StartYear;
            entity.EndYear = dto.EndYear;
            entity.TermType = dto.TermType;
            entity.Name = string.IsNullOrWhiteSpace(dto.Name)
                ? BuildTermName(dto.StartYear, dto.EndYear, dto.TermType)
                : dto.Name;
            entity.UpdatedAt = DateTime.UtcNow;

            _termDal.Update(entity);
            await _termDal.SaveChangesAsync();
            return _mapper.Map<AcademicTermDto>(entity);
        }

        public async Task<AcademicTermDto> SetActiveAsync(Guid id)
        {
            var entity = await _termDal.GetAsync(t => t.Id == id)
                ?? throw new KeyNotFoundException($"Dönem bulunamadı: {id}");

            // Diğer tüm dönemleri pasif yap
            await _termDal.DeactivateAllAsync();

            entity.IsActive = true;
            entity.UpdatedAt = DateTime.UtcNow;
            _termDal.Update(entity);
            await _termDal.SaveChangesAsync();

            return _mapper.Map<AcademicTermDto>(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _termDal.GetAsync(t => t.Id == id)
                ?? throw new KeyNotFoundException($"Dönem bulunamadı: {id}");

            if (entity.IsActive)
                throw new InvalidOperationException("Aktif dönem silinemez. Önce başka bir dönemi aktif yapın.");

            _termDal.Delete(entity);
            await _termDal.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid id) =>
            await _termDal.GetAsync(t => t.Id == id) != null;

        private static string BuildTermName(int startYear, int endYear, string termType)
        {
            var typeName = termType switch
            {
                TermType.Guz => "Güz",
                TermType.Bahar => "Bahar",
                TermType.Yaz => "Yaz",
                _ => termType
            };
            return $"{startYear}-{endYear} {typeName}";
        }
    }
}
