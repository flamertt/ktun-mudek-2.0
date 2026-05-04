using BitirmeApi.Core;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.DataAccess.Abstract
{
    public interface IStudentEvaluationResultDal : IRepository<StudentEvaluationResult>
    {
        /// <summary>Belirtilen ders açılışında dersi geçen öğrencilerin ExternalStudentId setini döner.</summary>
        Task<HashSet<int>> GetPassingStudentIdsAsync(int externalCourseOfferingId);
    }
}
