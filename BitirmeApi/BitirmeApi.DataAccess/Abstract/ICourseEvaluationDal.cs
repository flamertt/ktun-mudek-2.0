using BitirmeApi.Core;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.DataAccess.Abstract
{
    public interface ICourseEvaluationDal : IRepository<CourseEvaluation>
    {
        /// <summary>Tüm değerlendirmeler — CourseOffering.Course, AcademicTerm, Teacher include</summary>
        Task<List<CourseEvaluation>> GetAllWithDetailsAsync();

        /// <summary>Tek değerlendirme tam detay</summary>
        Task<CourseEvaluation?> GetByIdWithDetailsAsync(Guid id);

        /// <summary>Bir offering için değerlendirme (unique ilişki)</summary>
        Task<CourseEvaluation?> GetByOfferingIdWithDetailsAsync(Guid courseOfferingId);
    }
}
