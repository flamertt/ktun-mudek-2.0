using BitirmeApi.Core;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.DataAccess.Abstract
{
    public interface ICloEvaluationResultDal : IRepository<CloEvaluationResult>
    {
        /// <summary>
        /// Belirtilen ders açılışının "Combined" tipindeki DÖÇ sonuç satırlarını getirir.
        /// Güncelleme yapılabilmesi için tracked (AsNoTracking yok) döner.
        /// </summary>
        Task<List<CloEvaluationResult>> GetCombinedByOfferingAsync(Guid courseOfferingId);
    }
}
