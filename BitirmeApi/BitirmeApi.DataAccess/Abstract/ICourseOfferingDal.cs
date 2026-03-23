using BitirmeApi.Core;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.DataAccess.Abstract
{
    public interface ICourseOfferingDal : IRepository<CourseOffering>
    {
        /// <summary>Tüm açılışlar — Course, AcademicTerm, Teacher include</summary>
        Task<List<CourseOffering>> GetAllWithDetailsAsync();

        /// <summary>Belirli dönemin açılışları</summary>
        Task<List<CourseOffering>> GetByTermIdWithDetailsAsync(Guid termId);

        /// <summary>Aktif dönemin açılışları</summary>
        Task<List<CourseOffering>> GetByActiveTermWithDetailsAsync();

        /// <summary>Öğretmene ait açılışlar (tüm dönemler)</summary>
        Task<List<CourseOffering>> GetByTeacherIdWithDetailsAsync(Guid teacherId);

        /// <summary>Öğretmene ait açılışlar — belirli dönem filtresi isteğe bağlı</summary>
        Task<List<CourseOffering>> GetByTeacherIdAndTermAsync(Guid teacherId, Guid? termId = null);

        /// <summary>Bir dersin tüm dönemlerdeki açılışları</summary>
        Task<List<CourseOffering>> GetByCourseIdWithDetailsAsync(Guid courseId);

        /// <summary>Tek açılış detayı</summary>
        Task<CourseOffering?> GetByIdWithDetailsAsync(Guid id);

        /// <summary>Öğretmenin belirli bir açılışına erişim (ownership check)</summary>
        Task<CourseOffering?> GetByIdAndTeacherIdWithDetailsAsync(Guid offeringId, Guid teacherId);

        Task<bool> ExistsAsync(Guid id);
    }
}
