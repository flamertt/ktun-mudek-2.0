using BitirmeApi.Core;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.DataAccess.Abstract
{
    public interface IAcademicTermDal : IRepository<AcademicTerm>
    {
        /// <summary>En büyük Id'ye sahip (en güncel) dönemi döner.</summary>
        Task<AcademicTerm?> GetActiveAsync();
    }
}
