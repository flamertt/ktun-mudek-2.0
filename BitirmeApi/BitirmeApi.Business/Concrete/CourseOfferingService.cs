using AutoMapper;
using BitirmeApi.Business.Abstract;
using BitirmeApi.Business.DTO;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.Entity.Constants;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.Business.Concrete
{
    public class CourseOfferingService : ICourseOfferingService
    {
        private readonly ICourseOfferingDal _offeringDal;
        private readonly ICourseDal _courseDal;
        private readonly IAcademicTermDal _termDal;
        private readonly IAppUserDal _userDal;
        private readonly IMapper _mapper;

        public CourseOfferingService(
            ICourseOfferingDal offeringDal,
            ICourseDal courseDal,
            IAcademicTermDal termDal,
            IAppUserDal userDal,
            IMapper mapper)
        {
            _offeringDal = offeringDal;
            _courseDal = courseDal;
            _termDal = termDal;
            _userDal = userDal;
            _mapper = mapper;
        }

        // ── Sorgular ──────────────────────────────────────────────────────────────

        public async Task<List<CourseOfferingListDto>> GetAllAsync() =>
            _mapper.Map<List<CourseOfferingListDto>>(await _offeringDal.GetAllWithDetailsAsync());

        public async Task<List<CourseOfferingListDto>> GetByTermIdAsync(Guid termId) =>
            _mapper.Map<List<CourseOfferingListDto>>(await _offeringDal.GetByTermIdWithDetailsAsync(termId));

        public async Task<List<CourseOfferingListDto>> GetByActiveTermAsync() =>
            _mapper.Map<List<CourseOfferingListDto>>(await _offeringDal.GetByActiveTermWithDetailsAsync());

        public async Task<List<CourseOfferingListDto>> GetByTeacherIdAsync(Guid teacherId) =>
            _mapper.Map<List<CourseOfferingListDto>>(await _offeringDal.GetByTeacherIdWithDetailsAsync(teacherId));

        public async Task<List<CourseOfferingListDto>> GetByTeacherIdAndTermAsync(Guid teacherId, Guid? termId = null) =>
            _mapper.Map<List<CourseOfferingListDto>>(await _offeringDal.GetByTeacherIdAndTermAsync(teacherId, termId));

        public async Task<List<CourseOfferingListDto>> GetByCourseIdAsync(Guid courseId) =>
            _mapper.Map<List<CourseOfferingListDto>>(await _offeringDal.GetByCourseIdWithDetailsAsync(courseId));

        public async Task<CourseOfferingDto?> GetByIdAsync(Guid id)
        {
            var entity = await _offeringDal.GetByIdWithDetailsAsync(id);
            return entity != null ? _mapper.Map<CourseOfferingDto>(entity) : null;
        }

        public async Task<CourseOfferingDto?> GetByIdForTeacherAsync(Guid offeringId, Guid teacherId)
        {
            var entity = await _offeringDal.GetByIdAndTeacherIdWithDetailsAsync(offeringId, teacherId);
            return entity != null ? _mapper.Map<CourseOfferingDto>(entity) : null;
        }

        public Task<bool> ExistsAsync(Guid id) => _offeringDal.ExistsAsync(id);

        // ── Yazma ─────────────────────────────────────────────────────────────────

        public async Task<CourseOfferingDto> CreateAsync(CourseOfferingCreateDto dto)
        {
            // 1) Section zorunlu
            if (string.IsNullOrWhiteSpace(dto.Section))
                dto.Section = "A";

            // 2) Ders var mı?
            var course = await _courseDal.GetAsync(c => c.Id == dto.CourseId)
                ?? throw new KeyNotFoundException("Belirtilen ders kataloğu bulunamadı.");

            // 3) Ders aktif mi?
            if (!course.IsActive)
                throw new InvalidOperationException("Pasif bir ders için açılış oluşturulamaz.");

            // 4) Dönem var mı?
            var term = await _termDal.GetAsync(t => t.Id == dto.AcademicTermId)
                ?? throw new KeyNotFoundException("Belirtilen akademik dönem bulunamadı.");

            // 5) Dönem pasifse uyar ama engelleme (admin bilinçli karar verebilir)
            // İş kuralı: pasif döneme açılış oluşturulabilir ancak IsActive=false set edilmeli
            if (!term.IsActive && dto.IsActive)
                throw new InvalidOperationException(
                    "Pasif dönem için aktif açılış oluşturulamaz. " +
                    "Önce dönemi aktif edin ya da açılışı IsActive=false ile kaydedin.");

            // 6) Öğretmen atanıyorsa doğrula
            if (dto.TeacherId.HasValue)
                await ValidateTeacherAsync(dto.TeacherId.Value);

            // 7) Aynı Ders+Dönem+Şube zaten var mı?
            var duplicate = await _offeringDal.GetAsync(o =>
                o.CourseId == dto.CourseId &&
                o.AcademicTermId == dto.AcademicTermId &&
                o.Section == dto.Section);

            if (duplicate != null)
                throw new InvalidOperationException(
                    $"'{course.Code}' dersi bu dönemde '{dto.Section}' şubesiyle zaten açılmış.");

            var entity = new CourseOffering
            {
                Id = Guid.NewGuid(),
                CourseId = dto.CourseId,
                AcademicTermId = dto.AcademicTermId,
                TeacherId = dto.TeacherId,
                Section = dto.Section,
                PassingGrade = dto.PassingGrade,
                Quota = dto.Quota,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _offeringDal.Add(entity);
            await _offeringDal.SaveChangesAsync();

            return _mapper.Map<CourseOfferingDto>((await _offeringDal.GetByIdWithDetailsAsync(entity.Id))!);
        }

        public async Task<CourseOfferingDto> UpdateAsync(CourseOfferingUpdateDto dto)
        {
            // 1) Açılış var mı?
            var entity = await _offeringDal.GetAsync(o => o.Id == dto.Id)
                ?? throw new KeyNotFoundException("Ders açılışı bulunamadı.");

            // 2) Section zorunlu
            if (string.IsNullOrWhiteSpace(dto.Section))
                dto.Section = "A";

            // 3) Öğretmen değiştiriliyorsa doğrula
            if (dto.TeacherId.HasValue)
                await ValidateTeacherAsync(dto.TeacherId.Value);

            // 4) Section değiştiyse duplicate kontrolü (CourseId ve AcademicTermId sabit)
            if (dto.Section != entity.Section)
            {
                var duplicate = await _offeringDal.GetAsync(o =>
                    o.CourseId == entity.CourseId &&
                    o.AcademicTermId == entity.AcademicTermId &&
                    o.Section == dto.Section &&
                    o.Id != dto.Id);

                if (duplicate != null)
                    throw new InvalidOperationException(
                        $"Bu dönemde '{dto.Section}' şubesinde zaten bir açılış mevcut.");
            }

            // CourseId ve AcademicTermId güncellenmez — açılış kimliği değişmez
            entity.TeacherId = dto.TeacherId;
            entity.Section = dto.Section;
            entity.PassingGrade = dto.PassingGrade;
            entity.Quota = dto.Quota;
            entity.IsActive = dto.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            _offeringDal.Update(entity);
            await _offeringDal.SaveChangesAsync();

            return _mapper.Map<CourseOfferingDto>((await _offeringDal.GetByIdWithDetailsAsync(entity.Id))!);
        }

        public async Task<CourseOfferingDto> AssignTeacherAsync(Guid offeringId, Guid teacherId)
        {
            // 1) Açılış var mı?
            var entity = await _offeringDal.GetAsync(o => o.Id == offeringId)
                ?? throw new KeyNotFoundException("Ders açılışı bulunamadı.");

            // 2) Pasif açılışa öğretmen atanmasın
            if (!entity.IsActive)
                throw new InvalidOperationException("Pasif ders açılışına öğretmen atanamaz.");

            // 3) Öğretmen doğrulama (varlık + rol + aktiflik)
            await ValidateTeacherAsync(teacherId);

            // 4) Zaten aynı öğretmen atanmışsa işlem yapma
            if (entity.TeacherId == teacherId)
                return _mapper.Map<CourseOfferingDto>((await _offeringDal.GetByIdWithDetailsAsync(entity.Id))!);

            entity.TeacherId = teacherId;
            entity.UpdatedAt = DateTime.UtcNow;

            _offeringDal.Update(entity);
            await _offeringDal.SaveChangesAsync();

            return _mapper.Map<CourseOfferingDto>((await _offeringDal.GetByIdWithDetailsAsync(entity.Id))!);
        }

        public async Task<CourseOfferingDto> RemoveTeacherAsync(Guid offeringId)
        {
            // 1) Açılış var mı?
            var entity = await _offeringDal.GetAsync(o => o.Id == offeringId)
                ?? throw new KeyNotFoundException("Ders açılışı bulunamadı.");

            // 2) Zaten öğretmen yok mu?
            if (entity.TeacherId == null)
                throw new InvalidOperationException("Bu ders açılışına atanmış bir öğretmen bulunmuyor.");

            entity.TeacherId = null;
            entity.UpdatedAt = DateTime.UtcNow;

            _offeringDal.Update(entity);
            await _offeringDal.SaveChangesAsync();

            return _mapper.Map<CourseOfferingDto>((await _offeringDal.GetByIdWithDetailsAsync(entity.Id))!);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _offeringDal.GetAsync(o => o.Id == id)
                ?? throw new KeyNotFoundException("Ders açılışı bulunamadı.");

            _offeringDal.Delete(entity);
            await _offeringDal.SaveChangesAsync();
        }

        // ── Özel validation yardımcısı ────────────────────────────────────────────

        /// <summary>
        /// Öğretmen ataması öncesi üç koşul kontrol edilir:
        /// 1) Kullanıcı var mı?  2) Rolü Teacher mı?  3) Aktif mi?
        /// </summary>
        private async Task ValidateTeacherAsync(Guid teacherId)
        {
            if (!await _userDal.ExistsAsync(teacherId))
                throw new KeyNotFoundException("Atanacak öğretmen bulunamadı.");

            if (!await _userDal.IsInRoleAsync(teacherId, UserRoles.Teacher))
                throw new InvalidOperationException("Atanacak kullanıcı öğretmen rolünde değil.");

            if (!await _userDal.IsActiveAsync(teacherId))
                throw new InvalidOperationException("Pasif kullanıcı ders açılışına atanamaz.");
        }
    }
}
