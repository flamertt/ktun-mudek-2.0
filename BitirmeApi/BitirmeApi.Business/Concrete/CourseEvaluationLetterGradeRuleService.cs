using AutoMapper;
using BitirmeApi.Business.Abstract;
using BitirmeApi.Business.DTO;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.Business.Concrete
{
    public class CourseEvaluationLetterGradeRuleService : ICourseEvaluationLetterGradeRuleService
    {
        private readonly ICourseEvaluationLetterGradeRuleDal _ruleDal;
        private readonly ICourseEvaluationDal _evaluationDal;
        private readonly IMapper _mapper;

        public CourseEvaluationLetterGradeRuleService(
            ICourseEvaluationLetterGradeRuleDal ruleDal,
            ICourseEvaluationDal evaluationDal,
            IMapper mapper)
        {
            _ruleDal = ruleDal;
            _evaluationDal = evaluationDal;
            _mapper = mapper;
        }

        public async Task<List<CourseEvaluationLetterGradeRuleDto>> GetAllAsync()
        {
            var entities = await _ruleDal.GetListAsync();
            return _mapper.Map<List<CourseEvaluationLetterGradeRuleDto>>(entities.ToList());
        }

        public async Task<CourseEvaluationLetterGradeRuleDto?> GetByIdAsync(Guid id)
        {
            var entity = await _ruleDal.GetAsync(r => r.Id == id);
            return entity != null ? _mapper.Map<CourseEvaluationLetterGradeRuleDto>(entity) : null;
        }

        public async Task<List<CourseEvaluationLetterGradeRuleDto>> GetByCourseEvaluationIdAsync(Guid courseEvaluationId)
        {
            var orderedList = await _ruleDal.GetByEvaluationIdAsync(courseEvaluationId);
            return _mapper.Map<List<CourseEvaluationLetterGradeRuleDto>>(orderedList);
        }

        public async Task<CourseEvaluationLetterGradeRuleDto> AddAsync(CreateCourseEvaluationLetterGradeRuleDto createDto)
        {
            var entity = _mapper.Map<CourseEvaluationLetterGradeRule>(createDto);
            entity.CreatedAt = DateTime.UtcNow;
            var addedEntity = _ruleDal.Add(entity);
            await _ruleDal.SaveChangesAsync();
            return await Task.FromResult(_mapper.Map<CourseEvaluationLetterGradeRuleDto>(addedEntity));
        }

        public async Task<CourseEvaluationLetterGradeRuleDto> UpdateAsync(UpdateCourseEvaluationLetterGradeRuleDto updateDto)
        {
            var existingEntity = await _ruleDal.GetAsync(r => r.Id == updateDto.Id);
            if (existingEntity == null)
                throw new KeyNotFoundException($"Letter grade rule with ID {updateDto.Id} not found");

            _mapper.Map(updateDto, existingEntity);
            var updatedEntity = _ruleDal.Update(existingEntity);
            await _ruleDal.SaveChangesAsync();
            return await Task.FromResult(_mapper.Map<CourseEvaluationLetterGradeRuleDto>(updatedEntity));
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _ruleDal.GetAsync(r => r.Id == id);
            if (entity == null) throw new KeyNotFoundException("Rule bulunamadı.");
            _ruleDal.Delete(entity);
            await _ruleDal.SaveChangesAsync();
        }

        public async Task<List<CourseEvaluationLetterGradeRuleDto>> GetByEvaluationForTeacherAsync(Guid evaluationId, Guid teacherId)
        {
            await EnsureEvaluationOwnershipAsync(evaluationId, teacherId);
            return await GetByCourseEvaluationIdAsync(evaluationId);
        }

        public async Task<CourseEvaluationLetterGradeRuleDto> AddForTeacherAsync(CreateCourseEvaluationLetterGradeRuleDto createDto, Guid teacherId)
        {
            await EnsureEvaluationOwnershipAsync(createDto.CourseEvaluationId, teacherId);
            ValidateRange(createDto.MinScore, createDto.MaxScore);
            if (await _ruleDal.ExistsLetterAsync(createDto.CourseEvaluationId, createDto.LetterGrade))
                throw new InvalidOperationException("Aynı harf notu bu evaluation için zaten tanımlı.");
            await ValidateNoOverlapAsync(createDto.CourseEvaluationId, createDto.MinScore, createDto.MaxScore, null);
            return await AddAsync(createDto);
        }

        public async Task<CourseEvaluationLetterGradeRuleDto> UpdateForTeacherAsync(UpdateCourseEvaluationLetterGradeRuleDto updateDto, Guid teacherId)
        {
            var snapshot = await _ruleDal.GetByIdWithOwnershipAsync(updateDto.Id)
                ?? throw new KeyNotFoundException("Rule bulunamadı.");
            if (snapshot.CourseEvaluation?.CourseOffering?.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bu rule sizin dersinize ait değil.");
            ValidateRange(updateDto.MinScore, updateDto.MaxScore);
            if (await _ruleDal.ExistsLetterAsync(snapshot.CourseEvaluationId, updateDto.LetterGrade, updateDto.Id))
                throw new InvalidOperationException("Aynı harf notu bu evaluation için zaten tanımlı.");
            await ValidateNoOverlapAsync(snapshot.CourseEvaluationId, updateDto.MinScore, updateDto.MaxScore, updateDto.Id);
            return await UpdateAsync(updateDto);
        }

        public async Task DeleteForTeacherAsync(Guid id, Guid teacherId)
        {
            var snapshot = await _ruleDal.GetByIdWithOwnershipAsync(id)
                ?? throw new KeyNotFoundException("Rule bulunamadı.");
            if (snapshot.CourseEvaluation?.CourseOffering?.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bu rule sizin dersinize ait değil.");
            await DeleteAsync(id);
        }

        private async Task EnsureEvaluationOwnershipAsync(Guid evaluationId, Guid teacherId)
        {
            var ev = await _evaluationDal.GetByIdWithDetailsAsync(evaluationId)
                ?? throw new KeyNotFoundException("Evaluation bulunamadı.");
            if (ev.CourseOffering?.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bu evaluation sizin dersinize ait değil.");
        }

        private static void ValidateRange(decimal min, decimal max)
        {
            if (min > max) throw new InvalidOperationException("MinScore MaxScore'dan büyük olamaz.");
        }

        private async Task ValidateNoOverlapAsync(Guid evaluationId, decimal min, decimal max, Guid? excludeId)
        {
            var rules = await _ruleDal.GetByEvaluationIdAsync(evaluationId);
            if (excludeId.HasValue) rules = rules.Where(r => r.Id != excludeId.Value).ToList();
            var overlap = rules.Any(r => min <= r.MaxScore && max >= r.MinScore);
            if (overlap) throw new InvalidOperationException("Puan aralığı mevcut kurallarla çakışıyor.");
        }
    }
}
