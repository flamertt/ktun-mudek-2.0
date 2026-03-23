using BitirmeApi.Core;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.DataAccess.Abstract
{
    public interface IAcademicTermDal : IRepository<AcademicTerm>
    {
        Task<AcademicTerm?> GetActiveTermAsync();
        Task DeactivateAllAsync();
    }
}
