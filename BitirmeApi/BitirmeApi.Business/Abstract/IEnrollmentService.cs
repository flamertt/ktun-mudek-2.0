using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    public interface IEnrollmentService
    {
        // ── Sorgular ──────────────────────────────────────────────────────────────
        Task<List<EnrollmentListDto>> GetByOfferingIdAsync(Guid courseOfferingId);
        Task<List<StudentEnrollmentHistoryDto>> GetByStudentIdAsync(Guid studentId);

        // ── Tek kayıt ─────────────────────────────────────────────────────────────
        Task<EnrollmentListDto> EnrollStudentAsync(Guid courseOfferingId, Guid studentId);

        // ── Toplu kayıt (GUID listesi) — ayrıntılı sonuç döner ───────────────────
        Task<BulkEnrollResultDto> BulkEnrollStudentsAsync(Guid courseOfferingId, List<Guid> studentIds);

        // ── Toplu kayıt (öğrenci numarası / Excel) ────────────────────────────────
        Task<EnrollmentImportResultDto> BulkEnrollByStudentNumbersAsync(Guid courseOfferingId, List<string> studentNumbers);

        // ── Durum güncelleme ──────────────────────────────────────────────────────
        Task<EnrollmentListDto> UpdateStatusAsync(Guid courseOfferingId, Guid studentId, string status);

        // ── Silme ─────────────────────────────────────────────────────────────────
        Task RemoveEnrollmentAsync(Guid courseOfferingId, Guid studentId);

        // ── Validation helper ─────────────────────────────────────────────────────
        Task<bool> ExistsAsync(Guid courseOfferingId, Guid studentId);
    }
}
