using AutoMapper;
using BitirmeApi.Business.Abstract;
using BitirmeApi.Business.DTO;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.Business.Concrete
{
    public class CourseEvaluationService : ICourseEvaluationService
    {
        private readonly ICourseEvaluationDal _evalDal;
        private readonly ICourseOfferingDal _offeringDal;
        private readonly IMapper _mapper;

        public CourseEvaluationService(
            ICourseEvaluationDal evalDal,
            ICourseOfferingDal offeringDal,
            IMapper mapper)
        {
            _evalDal = evalDal;
            _offeringDal = offeringDal;
            _mapper = mapper;
        }

        // ── Okuma ────────────────────────────────────────────────────────────────

        public async Task<List<CourseEvaluationListDto>> GetAllAsync()
        {
            var list = await _evalDal.GetAllWithDetailsAsync();
            return _mapper.Map<List<CourseEvaluationListDto>>(list);
        }

        public async Task<CourseEvaluationDetailDto?> GetByIdAsync(Guid id)
        {
            var entity = await _evalDal.GetByIdWithDetailsAsync(id);
            return entity != null ? _mapper.Map<CourseEvaluationDetailDto>(entity) : null;
        }

        public async Task<CourseEvaluationDetailDto?> GetByOfferingIdAsync(Guid courseOfferingId)
        {
            var entity = await _evalDal.GetByOfferingIdWithDetailsAsync(courseOfferingId);
            return entity != null ? _mapper.Map<CourseEvaluationDetailDto>(entity) : null;
        }

        // ── Yazma — sahiplik serviste doğrulanır ─────────────────────────────────

        public async Task<CourseEvaluationDetailDto> CreateForTeacherAsync(CourseEvaluationCreateDto dto, Guid teacherId)
        {
            // 1) Offering var mı ve bu öğretmene mi atanmış?
            var offering = await _offeringDal.GetAsync(o => o.Id == dto.CourseOfferingId)
                ?? throw new KeyNotFoundException("Ders açılışı bulunamadı.");

            if (offering.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bu ders açılışı size atanmamış; değerlendirme oluşturamazsınız.");

            // 2) Zaten bir değerlendirme var mı?
            var existing = await _evalDal.GetAsync(e => e.CourseOfferingId == dto.CourseOfferingId);
            if (existing != null)
                throw new InvalidOperationException("Bu ders açılışı için zaten bir değerlendirme mevcut.");

            var entity = new CourseEvaluation
            {
                Id = Guid.NewGuid(),
                CourseOfferingId = dto.CourseOfferingId,
                CreatedDate = DateTime.UtcNow,
                StudentFeedbackEvaluation = dto.StudentFeedbackEvaluation,
                ProgramOutcomeEvaluation = dto.ProgramOutcomeEvaluation,
                GeneralEvaluation = dto.GeneralEvaluation,
                ImprovementSuggestions = dto.ImprovementSuggestions
            };

            _evalDal.Add(entity);
            await _evalDal.SaveChangesAsync();

            var result = await _evalDal.GetByIdWithDetailsAsync(entity.Id);
            return _mapper.Map<CourseEvaluationDetailDto>(result!);
        }

        public async Task<CourseEvaluationDetailDto> UpdateForTeacherAsync(CourseEvaluationUpdateDto dto, Guid teacherId)
        {
            // 1) Değerlendirme var mı ve offering detayları include edilmiş mi?
            var entity = await _evalDal.GetByIdWithDetailsAsync(dto.Id)
                ?? throw new KeyNotFoundException("Değerlendirme bulunamadı.");

            // 2) Bu offering bu öğretmene mi atanmış?
            if (entity.CourseOffering?.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bu değerlendirmeyi güncelleme yetkiniz yok.");

            entity.StudentFeedbackEvaluation = dto.StudentFeedbackEvaluation;
            entity.ProgramOutcomeEvaluation = dto.ProgramOutcomeEvaluation;
            entity.GeneralEvaluation = dto.GeneralEvaluation;
            entity.ImprovementSuggestions = dto.ImprovementSuggestions;
            entity.UpdatedAt = DateTime.UtcNow;

            _evalDal.Update(entity);
            await _evalDal.SaveChangesAsync();

            var result = await _evalDal.GetByIdWithDetailsAsync(entity.Id);
            return _mapper.Map<CourseEvaluationDetailDto>(result!);
        }

        public async Task DeleteForTeacherAsync(Guid id, Guid teacherId)
        {
            // 1) Değerlendirme var mı?
            var entity = await _evalDal.GetByIdWithDetailsAsync(id)
                ?? throw new KeyNotFoundException("Değerlendirme bulunamadı.");

            // 2) Bu offering bu öğretmene mi atanmış?
            if (entity.CourseOffering?.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bu değerlendirmeyi silme yetkiniz yok.");

            _evalDal.Delete(entity);
            await _evalDal.SaveChangesAsync();
        }
    }
}
