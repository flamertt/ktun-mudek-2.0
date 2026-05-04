using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    /// <summary>
    /// Offering bazlı MÜDEK hesap sonuçları: tek snapshot, delete + recompute + insert, transaction.
    /// </summary>
    public interface IMudekEvaluationCalculatorService
    {
        /// <summary>Tüm sonuçları siler, yeniden hesaplar ve kaydeder. Üniversite API token gerekli.</summary>
        Task<MudekEvaluationSnapshotDto> RecalculateAsync(int externalCourseOfferingId, int externalTeacherId, string universityToken, CancellationToken ct = default);

        /// <summary>Veritabanındaki güncel snapshot (hesaplama yapmaz).</summary>
        Task<MudekEvaluationSnapshotDto?> GetSnapshotAsync(int externalCourseOfferingId, int externalTeacherId, CancellationToken ct = default);

        Task MarkStaleByOfferingIdAsync(int externalCourseOfferingId, CancellationToken ct = default);
        Task MarkStaleByCourseEvaluationIdAsync(Guid courseEvaluationId, CancellationToken ct = default);
        Task MarkStaleByExamIdAsync(Guid examId, CancellationToken ct = default);
    }
}
