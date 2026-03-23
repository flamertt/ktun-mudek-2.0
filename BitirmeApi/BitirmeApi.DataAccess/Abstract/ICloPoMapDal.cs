using BitirmeApi.Core;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.DataAccess.Abstract
{
    public interface ICloPoMapDal : IRepository<CloPoMap>
    {
        /// <summary>Bir CLO'nun tüm PO eşlemeleri — CLO ve PO include</summary>
        Task<List<CloPoMap>> GetByCloIdAsync(Guid cloId);

        /// <summary>Bir dersteki tüm CLO→PO eşlemeleri</summary>
        Task<List<CloPoMap>> GetByCourseIdAsync(Guid courseId);

        Task<bool> ExistsAsync(Guid cloId, Guid programOutcomeId);
        Task<CloPoMap?> GetByIdsAsync(Guid cloId, Guid programOutcomeId);
    }
}
