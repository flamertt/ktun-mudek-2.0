using BitirmeApi.Core;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.DataAccess.Abstract
{
    public interface IExamQuestionDal : IRepository<ExamQuestion>
    {
        /// <summary>Bir sınavın soruları — CLO eşlemeleriyle</summary>
        Task<List<ExamQuestion>> GetByExamIdWithDetailsAsync(Guid examId);

        /// <summary>Tek soru detayı — CLO eşlemeleriyle</summary>
        Task<ExamQuestion?> GetByIdWithDetailsAsync(Guid id);

        /// <summary>Ownership için: soru + Exam + CourseEvaluation + CourseOffering</summary>
        Task<ExamQuestion?> GetByIdWithOwnershipAsync(Guid id);

        Task<bool> ExistsAsync(Guid id);
    }
}
