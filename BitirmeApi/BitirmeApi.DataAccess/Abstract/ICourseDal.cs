using BitirmeApi.Core;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.DataAccess.Abstract
{
    public interface ICourseDal : IRepository<Course>
    {
        Task<List<Course>> GetAllWithDetailsAsync();
        Task<List<Course>> GetByProgramIdWithDetailsAsync(Guid programId);
        Task<Course?> GetByIdWithDetailsAsync(Guid id);
    }
}
