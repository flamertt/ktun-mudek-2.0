using BitirmeApi.Core;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.DataAccess.Abstract
{
    public interface IEnrollmentDal : IRepository<Enrollment>
    {
        /// <summary>Bir açılışın kayıtlı öğrencileri (Student include)</summary>
        Task<List<Enrollment>> GetByOfferingWithDetailsAsync(Guid courseOfferingId);

        /// <summary>Öğrencinin tüm kayıtları (CourseOffering → Course, AcademicTerm include)</summary>
        Task<List<Enrollment>> GetByStudentIdWithDetailsAsync(Guid studentId);

        Task<bool> IsEnrolledAsync(Guid courseOfferingId, Guid studentId);

        Task<Enrollment?> GetByOfferingAndStudentAsync(Guid courseOfferingId, Guid studentId);

        /// <summary>
        /// Enrollment'a bağlı öğrenci cevabı veya puan kaydı var mı?
        /// Silme öncesi veri bütünlüğü kontrolü için kullanılır.
        /// </summary>
        Task<bool> HasAssociatedDataAsync(Guid enrollmentId);
    }
}
