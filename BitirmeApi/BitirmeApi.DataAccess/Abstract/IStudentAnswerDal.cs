using BitirmeApi.Core;
using BitirmeApi.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeApi.DataAccess.Abstract
{
    public interface IStudentAnswerDal:IRepository<StudentAnswer>
    {
        Task<List<StudentAnswer>> GetByQuestionIdWithDetailsAsync(Guid questionId);
        Task<StudentAnswer?> GetByIdWithOwnershipAsync(Guid id);
        Task<StudentAnswer?> GetByQuestionAndEnrollmentAsync(Guid questionId, Guid enrollmentId);
        Task<bool> ExistsAsync(Guid questionId, Guid enrollmentId);
    }
}
