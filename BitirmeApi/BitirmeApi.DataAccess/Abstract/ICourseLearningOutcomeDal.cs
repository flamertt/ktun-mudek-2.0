using BitirmeApi.Core;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.DataAccess.Abstract
{
    public interface ICourseLearningOutcomeDal : IRepository<CourseLearningOutcome>
    {
        /// <summary>Bir derse ait tüm CLO'lar — Course include</summary>
        Task<List<CourseLearningOutcome>> GetByCourseIdAsync(Guid courseId);

        /// <summary>Tek CLO detayı — Course ve Maps (CLO→PO) include</summary>
        Task<CourseLearningOutcome?> GetByIdWithDetailsAsync(Guid id);

        Task<bool> ExistsAsync(Guid id);
        Task<bool> CodeExistsForCourseAsync(Guid courseId, string code, Guid? excludeId = null);
    }
}
