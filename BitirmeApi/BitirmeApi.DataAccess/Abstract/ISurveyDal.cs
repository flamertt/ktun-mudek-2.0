using BitirmeApi.Core;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.DataAccess.Abstract
{
    public interface ISurveyDal : IRepository<Survey>
    {
        /// <summary>Bir ders açılışına ait anketler (sorular ve gönderimler dahil).</summary>
        Task<List<Survey>> GetByOfferingIdAsync(Guid offeringId);

        /// <summary>Anket detayı — sorular, gönderimler ve CourseOffering dahil.</summary>
        Task<Survey?> GetByIdWithDetailsAsync(Guid id);

        /// <summary>Öğrenci görünümü: açılışa ait aktif anketler (sadece sorular dahil).</summary>
        Task<List<Survey>> GetActiveByOfferingIdAsync(Guid offeringId);

        /// <summary>Öğrenci görünümü: tek aktif anket detayı (sorular dahil, CourseOffering dahil).</summary>
        Task<Survey?> GetActiveByIdAsync(Guid id);
    }
}
