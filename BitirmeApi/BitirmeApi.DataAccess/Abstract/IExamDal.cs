using BitirmeApi.Core;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.DataAccess.Abstract
{
    public interface IExamDal : IRepository<Exam>
    {
        /// <summary>Bir değerlendirmeye ait sınavlar (soru sayısıyla)</summary>
        Task<List<Exam>> GetByEvaluationIdAsync(Guid evaluationId);

        /// <summary>Sınav detayı — sorular ve CLO eşlemeleri dahil</summary>
        Task<Exam?> GetByIdWithDetailsAsync(Guid id);

        /// <summary>Ownership zinciri için: sınav + CourseEvaluation + CourseOffering</summary>
        Task<Exam?> GetByIdWithOwnershipAsync(Guid id);

        Task<bool> ExistsAsync(Guid id);
    }
}
