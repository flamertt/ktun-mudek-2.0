using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    public interface ICourseLearningOutcomeService
    {
        /// <summary>Bir derse ait tüm CLO'ları getirir</summary>
        Task<List<CourseLearningOutcomeDto>> GetByCourseIdAsync(Guid courseId);

        Task<CourseLearningOutcomeDto?> GetByIdAsync(Guid id);

        /// <summary>Derse yeni CLO ekler — Code benzersizliği kontrol edilir</summary>
        Task<CourseLearningOutcomeDto> CreateAsync(CreateCourseLearningOutcomeDto dto);

        Task<CourseLearningOutcomeDto> UpdateAsync(UpdateCourseLearningOutcomeDto dto);

        Task DeleteAsync(Guid id);

        Task<bool> ExistsAsync(Guid id);
    }
}
