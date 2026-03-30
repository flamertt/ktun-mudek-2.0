using AutoMapper;
using BitirmeApi.Business.Abstract;
using BitirmeApi.Business.DTO;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.Business.Concrete
{
    public class ExamService : IExamService
    {
        private readonly IExamDal _examDal;
        private readonly ICourseEvaluationDal _evaluationDal;
        private readonly IExamQuestionDal _questionDal;
        private readonly IAssessmentComponentDal _componentDal;
        private readonly IMapper _mapper;
        private readonly IMudekEvaluationCalculatorService _mudekStale;

        public ExamService(
            IExamDal examDal,
            ICourseEvaluationDal evaluationDal,
            IExamQuestionDal questionDal,
            IAssessmentComponentDal componentDal,
            IMapper mapper,
            IMudekEvaluationCalculatorService mudekStale)
        {
            _examDal = examDal;
            _evaluationDal = evaluationDal;
            _questionDal = questionDal;
            _componentDal = componentDal;
            _mapper = mapper;
            _mudekStale = mudekStale;
        }

        public async Task<List<ExamListDto>> GetByEvaluationIdAsync(Guid evaluationId) =>
            _mapper.Map<List<ExamListDto>>(await _examDal.GetByEvaluationIdAsync(evaluationId));

        public async Task<List<ExamListDto>> GetByEvaluationIdForTeacherAsync(Guid evaluationId, Guid teacherId)
        {
            var evaluation = await _evaluationDal.GetByIdWithDetailsAsync(evaluationId)
                ?? throw new KeyNotFoundException("Evaluation bulunamadı.");
            if (evaluation.CourseOffering?.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bu evaluation size ait değil.");
            return await GetByEvaluationIdAsync(evaluationId);
        }

        public async Task<ExamDetailDto?> GetByIdAsync(Guid id)
        {
            var entity = await _examDal.GetByIdWithDetailsAsync(id);
            return entity != null ? _mapper.Map<ExamDetailDto>(entity) : null;
        }

        public async Task<ExamDetailDto?> GetByIdForTeacherAsync(Guid id, Guid teacherId)
        {
            var snapshot = await _examDal.GetByIdWithOwnershipAsync(id);
            if (snapshot == null) return null;
            if (snapshot.CourseEvaluation?.CourseOffering?.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bu sınav size ait değil.");
            return await GetByIdAsync(id);
        }

        public async Task<ExamDetailDto> CreateAsync(CreateExamDto dto, Guid teacherId)
        {
            // CourseEvaluationDal'ın GetByIdWithDetailsAsync metodu CourseOffering'i include eder
            var evaluation = await _evaluationDal.GetByIdWithDetailsAsync(dto.CourseEvaluationId)
                ?? throw new KeyNotFoundException("Belirtilen ders değerlendirmesi bulunamadı.");

            if (evaluation.CourseOffering?.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bu değerlendirme sizin dersinize ait değil.");

            if (dto.WeightPercentage < 0 || dto.WeightPercentage > 100)
                throw new InvalidOperationException("Ağırlık yüzdesi 0 ile 100 arasında olmalıdır.");
            if ((await _examDal.GetListAsync(e => e.CourseEvaluationId == dto.CourseEvaluationId && e.OrderIndex == dto.OrderIndex)).Any())
                throw new InvalidOperationException("Aynı evaluation altında OrderIndex benzersiz olmalıdır.");

            var entity = new Exam
            {
                Id = Guid.NewGuid(),
                CourseEvaluationId = dto.CourseEvaluationId,
                ExamType = dto.ExamType,
                WeightPercentage = dto.WeightPercentage,
                OrderIndex = dto.OrderIndex
            };

            _examDal.Add(entity);
            await _examDal.SaveChangesAsync();
            await _mudekStale.MarkStaleByCourseEvaluationIdAsync(dto.CourseEvaluationId);

            return _mapper.Map<ExamDetailDto>((await _examDal.GetByIdWithDetailsAsync(entity.Id))!);
        }

        public async Task<ExamDetailDto> UpdateAsync(UpdateExamDto dto, Guid teacherId)
        {
            // Ownership zinciri: Exam → CourseEvaluation → CourseOffering
            var snapshot = await _examDal.GetByIdWithOwnershipAsync(dto.Id)
                ?? throw new KeyNotFoundException("Sınav bulunamadı.");

            if (snapshot.CourseEvaluation?.CourseOffering?.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bu sınav sizin dersinize ait değil.");

            if (dto.WeightPercentage < 0 || dto.WeightPercentage > 100)
                throw new InvalidOperationException("Ağırlık yüzdesi 0 ile 100 arasında olmalıdır.");
            if ((await _examDal.GetListAsync(e =>
                e.CourseEvaluationId == snapshot.CourseEvaluationId &&
                e.OrderIndex == dto.OrderIndex &&
                e.Id != dto.Id)).Any())
                throw new InvalidOperationException("Aynı evaluation altında OrderIndex benzersiz olmalıdır.");

            // AsNoTracking snapshot'ı varken tracked entity'yi ayrıca çekip güncelle
            var tracked = await _examDal.GetAsync(e => e.Id == dto.Id)
                ?? throw new KeyNotFoundException("Sınav bulunamadı.");

            tracked.ExamType = dto.ExamType;
            tracked.WeightPercentage = dto.WeightPercentage;
            tracked.OrderIndex = dto.OrderIndex;

            _examDal.Update(tracked);
            await _examDal.SaveChangesAsync();
            await _mudekStale.MarkStaleByCourseEvaluationIdAsync(snapshot.CourseEvaluationId);

            return _mapper.Map<ExamDetailDto>((await _examDal.GetByIdWithDetailsAsync(dto.Id))!);
        }

        public async Task DeleteAsync(Guid id, Guid teacherId)
        {
            var snapshot = await _examDal.GetByIdWithOwnershipAsync(id)
                ?? throw new KeyNotFoundException("Sınav bulunamadı.");

            if (snapshot.CourseEvaluation?.CourseOffering?.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bu sınav sizin dersinize ait değil.");
            if ((await _questionDal.GetListAsync(q => q.ExamId == id)).Any() ||
                (await _componentDal.GetListAsync(c => c.ExamId == id)).Any())
                throw new InvalidOperationException("Sınava bağlı soru/component olduğu için silinemez.");

            var tracked = await _examDal.GetAsync(e => e.Id == id)
                ?? throw new KeyNotFoundException("Sınav bulunamadı.");

            var evalId = snapshot.CourseEvaluationId;
            _examDal.Delete(tracked);
            await _examDal.SaveChangesAsync();
            await _mudekStale.MarkStaleByCourseEvaluationIdAsync(evalId);
        }
    }
}
