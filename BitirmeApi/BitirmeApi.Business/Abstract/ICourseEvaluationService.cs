using BitirmeApi.Business.DTO;

namespace BitirmeApi.Business.Abstract
{
    public interface ICourseEvaluationService
    {
        Task<List<CourseEvaluationListDto>> GetAllAsync();
        Task<CourseEvaluationDetailDto?> GetByIdAsync(Guid id);
        Task<CourseEvaluationDetailDto?> GetByOfferingIdAsync(int externalCourseOfferingId);
        Task<List<CourseEvaluationListDto>> GetByTeacherIdAsync(int externalTeacherId);

        /// <summary>
        /// Öğretmen kendi ders açılışı için değerlendirme oluşturur.
        /// ExternalTeacherId ile üniversite API'sinden doğrulama yapılır.
        /// </summary>
        Task<CourseEvaluationDetailDto> CreateForTeacherAsync(CourseEvaluationCreateDto dto, int externalTeacherId, string universityToken);

        Task<CourseEvaluationDetailDto> UpdateForTeacherAsync(CourseEvaluationUpdateDto dto, int externalTeacherId);

        Task DeleteForTeacherAsync(Guid id, int externalTeacherId);
    }
}
