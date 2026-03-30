using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    /// <summary>
    /// Offering bazlı MÜDEK hesap sonuçları: tek snapshot, delete + recompute + insert, transaction.
    /// </summary>
    public interface IMudekEvaluationCalculatorService
    {
        /// <summary>Tüm sonuçları siler, yeniden hesaplar ve kaydeder. CourseEvaluation güncellenir.</summary>
        Task<MudekEvaluationSnapshotDto> RecalculateForTeacherAsync(Guid courseOfferingId, Guid teacherId, CancellationToken ct = default);

        /// <summary>Veritabanındaki güncel snapshot (hesaplama yapmaz).</summary>
        Task<MudekEvaluationSnapshotDto?> GetSnapshotForTeacherAsync(Guid courseOfferingId, Guid teacherId, CancellationToken ct = default);

        Task MarkStaleByOfferingIdAsync(Guid courseOfferingId, CancellationToken ct = default);
        Task MarkStaleByCourseEvaluationIdAsync(Guid courseEvaluationId, CancellationToken ct = default);
        Task MarkStaleByEnrollmentIdAsync(Guid enrollmentId, CancellationToken ct = default);

        /// <summary>CLO–PÇ eşlemesi katalog CLO üzerinden değiştiğinde ilgili tüm offeringler.</summary>
        Task MarkStaleByCourseLearningOutcomeIdAsync(Guid courseLearningOutcomeId, CancellationToken ct = default);

        Task MarkStaleByExamIdAsync(Guid examId, CancellationToken ct = default);
    }
}
