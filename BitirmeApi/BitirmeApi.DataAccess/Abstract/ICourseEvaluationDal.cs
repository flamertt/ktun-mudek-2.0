using BitirmeApi.Core;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.DataAccess.Abstract
{
    public interface ICourseEvaluationDal : IRepository<CourseEvaluation>
    {
        Task<List<CourseEvaluation>> GetAllWithDetailsAsync();
        Task<CourseEvaluation?> GetByIdWithDetailsAsync(Guid id);
        Task<CourseEvaluation?> GetByOfferingIdAsync(int externalCourseOfferingId);
        /// <summary>Öğretmene ait tüm değerlendirmeler</summary>
        Task<List<CourseEvaluation>> GetByTeacherIdAsync(int externalTeacherId);
    }
}
