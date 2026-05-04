using BitirmeApi.Entity.Entities;

namespace BitirmeApi.Business.Abstract
{
    public interface IAcademicTermService
    {
        /// <summary>DB'deki en güncel (en büyük Id) dönemi döner. Yoksa null.</summary>
        Task<AcademicTerm?> GetActiveAsync();

        /// <summary>
        /// Üniversite API'sinden tüm dönemleri çeker, en büyük Id'liyi bulur
        /// ve DB'ye ekler (yoksa) veya günceller (varsa).
        /// </summary>
        Task<AcademicTerm> SyncActiveAsync(string universityToken);
    }
}
