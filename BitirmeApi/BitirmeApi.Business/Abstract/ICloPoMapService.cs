using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    public interface ICloPoMapService
    {
        /// <summary>Bir CLO'nun bağlı program çıktılarını getirir</summary>
        Task<List<CloPoMapDto>> GetByCloIdAsync(Guid cloId);

        /// <summary>Bir derse ait tüm CLO→PO eşlemelerini getirir</summary>
        Task<List<CloPoMapDto>> GetByCourseIdAsync(Guid courseId);

        /// <summary>CLO ile Program Çıktısını eşler — duplicate ve varlık kontrolleri yapılır</summary>
        Task<CloPoMapDto> MapAsync(CreateCloPoMapDto dto);

        /// <summary>Eşleme ağırlığını günceller</summary>
        Task<CloPoMapDto> UpdateWeightAsync(Guid cloId, Guid programOutcomeId, decimal weight);

        /// <summary>CLO → PO eşlemesini kaldırır</summary>
        Task UnmapAsync(Guid cloId, Guid programOutcomeId);
    }
}
