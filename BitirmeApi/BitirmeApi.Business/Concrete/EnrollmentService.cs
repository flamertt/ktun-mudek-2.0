using AutoMapper;
using BitirmeApi.Business.Abstract;
using BitirmeApi.Business.DTO;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.Entity.Constants;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.Business.Concrete
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentDal _enrollmentDal;
        private readonly ICourseOfferingDal _offeringDal;
        private readonly IAppUserDal _userDal;
        private readonly IMapper _mapper;

        public EnrollmentService(
            IEnrollmentDal enrollmentDal,
            ICourseOfferingDal offeringDal,
            IAppUserDal userDal,
            IMapper mapper)
        {
            _enrollmentDal = enrollmentDal;
            _offeringDal = offeringDal;
            _userDal = userDal;
            _mapper = mapper;
        }

        // ── Sorgular ──────────────────────────────────────────────────────────────

        public async Task<List<EnrollmentListDto>> GetByOfferingIdAsync(Guid courseOfferingId) =>
            _mapper.Map<List<EnrollmentListDto>>(
                await _enrollmentDal.GetByOfferingWithDetailsAsync(courseOfferingId));

        public async Task<List<StudentEnrollmentHistoryDto>> GetByStudentIdAsync(Guid studentId) =>
            _mapper.Map<List<StudentEnrollmentHistoryDto>>(
                await _enrollmentDal.GetByStudentIdWithDetailsAsync(studentId));

        public Task<bool> ExistsAsync(Guid courseOfferingId, Guid studentId) =>
            _enrollmentDal.IsEnrolledAsync(courseOfferingId, studentId);

        // ── Tek kayıt ─────────────────────────────────────────────────────────────

        public async Task<EnrollmentListDto> EnrollStudentAsync(Guid courseOfferingId, Guid studentId)
        {
            // 1) Açılış var mı ve aktif mi?
            var offering = await _offeringDal.GetAsync(o => o.Id == courseOfferingId)
                ?? throw new KeyNotFoundException("Belirtilen ders açılışı bulunamadı.");

            if (!offering.IsActive)
                throw new InvalidOperationException("Pasif ders açılışına öğrenci kaydı yapılamaz.");

            // 2) Öğrenci validasyonu
            await ValidateStudentAsync(studentId);

            // 3) Zaten kayıtlı mı?
            if (await _enrollmentDal.IsEnrolledAsync(courseOfferingId, studentId))
                throw new InvalidOperationException("Öğrenci bu ders açılışına zaten kayıtlı.");

            var enrollment = new Enrollment
            {
                Id = Guid.NewGuid(),
                CourseOfferingId = courseOfferingId,
                StudentId = studentId,
                Status = EnrollmentStatus.Enrolled,
                EnrolledAt = DateTime.UtcNow
            };

            _enrollmentDal.Add(enrollment);
            await _enrollmentDal.SaveChangesAsync();

            var list = await _enrollmentDal.GetByOfferingWithDetailsAsync(courseOfferingId);
            return _mapper.Map<EnrollmentListDto>(list.First(e => e.StudentId == studentId));
        }

        // ── Toplu kayıt (GUID listesi) ────────────────────────────────────────────

        public async Task<BulkEnrollResultDto> BulkEnrollStudentsAsync(Guid courseOfferingId, List<Guid> studentIds)
        {
            // Açılış kontrolü
            var offering = await _offeringDal.GetAsync(o => o.Id == courseOfferingId)
                ?? throw new KeyNotFoundException("Belirtilen ders açılışı bulunamadı.");

            if (!offering.IsActive)
                throw new InvalidOperationException("Pasif ders açılışına toplu kayıt yapılamaz.");

            var result = new BulkEnrollResultDto();
            var toAdd = new List<Enrollment>();

            foreach (var studentId in studentIds)
            {
                // Kullanıcı var mı?
                if (!await _userDal.ExistsAsync(studentId))
                {
                    result.NotFound.Add(studentId);
                    continue;
                }

                // Student rolünde mi?
                if (!await _userDal.IsInRoleAsync(studentId, UserRoles.Student))
                {
                    result.NotStudent.Add(studentId);
                    continue;
                }

                // Aktif mi?
                if (!await _userDal.IsActiveAsync(studentId))
                {
                    result.InactiveUsers.Add(studentId);
                    continue;
                }

                // Zaten kayıtlı mı?
                if (await _enrollmentDal.IsEnrolledAsync(courseOfferingId, studentId))
                {
                    result.AlreadyEnrolled.Add(studentId);
                    continue;
                }

                toAdd.Add(new Enrollment
                {
                    Id = Guid.NewGuid(),
                    CourseOfferingId = courseOfferingId,
                    StudentId = studentId,
                    Status = EnrollmentStatus.Enrolled,
                    EnrolledAt = DateTime.UtcNow
                });
            }

            foreach (var e in toAdd)
                _enrollmentDal.Add(e);

            if (toAdd.Any())
                await _enrollmentDal.SaveChangesAsync();

            result.Enrolled.AddRange(toAdd.Select(e => e.StudentId));
            return result;
        }

        // ── Toplu kayıt (öğrenci numarası / Excel) ────────────────────────────────

        public async Task<EnrollmentImportResultDto> BulkEnrollByStudentNumbersAsync(
            Guid courseOfferingId, List<string> studentNumbers)
        {
            // Açılış kontrolü
            var offering = await _offeringDal.GetAsync(o => o.Id == courseOfferingId)
                ?? throw new KeyNotFoundException("Belirtilen ders açılışı bulunamadı.");

            if (!offering.IsActive)
                throw new InvalidOperationException("Pasif ders açılışına toplu kayıt yapılamaz.");

            var result = new EnrollmentImportResultDto { TotalRows = studentNumbers.Count };
            var toAdd = new List<Enrollment>();

            foreach (var number in studentNumbers)
            {
                var trimmed = number.Trim();
                if (string.IsNullOrWhiteSpace(trimmed))
                {
                    result.FormatErrors.Add(trimmed);
                    continue;
                }

                // Öğrenci numarasıyla kullanıcıyı bul
                var student = await _userDal.GetByStudentNumberAsync(trimmed);
                if (student == null)
                {
                    result.NotFound.Add(trimmed);
                    continue;
                }

                // Student rolü kontrolü
                if (student.Role != UserRoles.Student)
                {
                    result.FormatErrors.Add($"{trimmed} (öğrenci rolünde değil)");
                    continue;
                }

                // Aktiflik kontrolü
                if (!student.IsActive)
                {
                    result.FormatErrors.Add($"{trimmed} (pasif kullanıcı)");
                    continue;
                }

                // Zaten kayıtlı mı?
                if (await _enrollmentDal.IsEnrolledAsync(courseOfferingId, student.Id))
                {
                    result.AlreadyEnrolled.Add(trimmed);
                    continue;
                }

                toAdd.Add(new Enrollment
                {
                    Id = Guid.NewGuid(),
                    CourseOfferingId = courseOfferingId,
                    StudentId = student.Id,
                    Status = EnrollmentStatus.Enrolled,
                    EnrolledAt = DateTime.UtcNow
                });
            }

            foreach (var e in toAdd)
                _enrollmentDal.Add(e);

            if (toAdd.Any())
                await _enrollmentDal.SaveChangesAsync();

            result.Enrolled = toAdd.Count;
            result.Message = $"{result.Enrolled} öğrenci kayıt edildi. " +
                             $"Zaten kayıtlı: {result.AlreadyEnrolled.Count}, " +
                             $"Bulunamadı: {result.NotFound.Count}, " +
                             $"Sorunlu kayıt: {result.FormatErrors.Count}.";
            return result;
        }

        // ── Durum güncelleme ──────────────────────────────────────────────────────

        public async Task<EnrollmentListDto> UpdateStatusAsync(
            Guid courseOfferingId, Guid studentId, string status)
        {
            // Geçerli durum değeri mi?
            var validStatuses = new[]
            {
                EnrollmentStatus.Enrolled, EnrollmentStatus.Passed,
                EnrollmentStatus.Failed, EnrollmentStatus.Withdrawn, EnrollmentStatus.Repeat
            };

            if (!validStatuses.Contains(status))
                throw new InvalidOperationException(
                    $"Geçersiz kayıt durumu: '{status}'. " +
                    $"Geçerli değerler: {string.Join(", ", validStatuses)}");

            var enrollment = await _enrollmentDal.GetByOfferingAndStudentAsync(courseOfferingId, studentId)
                ?? throw new KeyNotFoundException("Kayıt bulunamadı.");

            // İş kuralı: Withdrawn durumundan doğrudan Passed durumuna geçiş yapılamaz.
            if (enrollment.Status == EnrollmentStatus.Withdrawn && status == EnrollmentStatus.Passed)
                throw new InvalidOperationException(
                    "Dersten çekilen (Withdrawn) öğrencinin durumu doğrudan Passed yapılamaz. " +
                    "Önce Enrolled veya Repeat durumuna alın.");

            enrollment.Status = status;
            enrollment.UpdatedAt = DateTime.UtcNow;

            _enrollmentDal.Update(enrollment);
            await _enrollmentDal.SaveChangesAsync();

            var list = await _enrollmentDal.GetByOfferingWithDetailsAsync(courseOfferingId);
            return _mapper.Map<EnrollmentListDto>(list.First(e => e.StudentId == studentId));
        }

        // ── Silme ─────────────────────────────────────────────────────────────────

        public async Task RemoveEnrollmentAsync(Guid courseOfferingId, Guid studentId)
        {
            var enrollment = await _enrollmentDal.GetByOfferingAndStudentAsync(courseOfferingId, studentId)
                ?? throw new KeyNotFoundException("Kayıt bulunamadı.");

            // Veri bütünlüğü: öğrenciye ait sınav/puan kaydı varsa fiziksel silme yapılmaz.
            if (await _enrollmentDal.HasAssociatedDataAsync(enrollment.Id))
                throw new InvalidOperationException(
                    "Bu öğrenciye ait sınav cevabı veya puan kaydı mevcut. " +
                    "Fiziksel silme yerine durumu 'Withdrawn' olarak güncelleyin.");

            _enrollmentDal.Delete(enrollment);
            await _enrollmentDal.SaveChangesAsync();
        }

        // ── Özel validation yardımcısı ────────────────────────────────────────────

        /// <summary>
        /// Öğrenci ataması öncesi üç koşul:
        /// 1) Kullanıcı var mı?  2) Rolü Student mı?  3) Aktif mi?
        /// </summary>
        private async Task ValidateStudentAsync(Guid studentId)
        {
            if (!await _userDal.ExistsAsync(studentId))
                throw new KeyNotFoundException("Belirtilen öğrenci bulunamadı.");

            if (!await _userDal.IsInRoleAsync(studentId, UserRoles.Student))
                throw new InvalidOperationException("Belirtilen kullanıcı öğrenci rolünde değil.");

            if (!await _userDal.IsActiveAsync(studentId))
                throw new InvalidOperationException("Pasif kullanıcı ders açılışına kayıt edilemez.");
        }
    }
}
