using BitirmeApi.Core;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.DataAccess.Abstract
{
    public interface IStudentEvaluationResultDal : IRepository<StudentEvaluationResult>
    {
        /// <summary>
        /// Belirtilen ders açılışında dersi geçen öğrencilerin UserId setini döner.
        /// StudentEvaluationResult → Enrollment → StudentId zinciri kullanılır.
        /// Hesaplama yapılmamışsa boş set döner.
        /// </summary>
        Task<HashSet<Guid>> GetPassingStudentIdsAsync(Guid courseOfferingId);
    }
}
