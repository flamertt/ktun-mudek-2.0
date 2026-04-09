using AutoMapper;
using BitirmeApi.Business.Abstract;
using BitirmeApi.Business.DTO;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.Business.Concrete
{
    public class ProgramLetterGradeRuleService : IProgramLetterGradeRuleService
    {
        private readonly IProgramLetterGradeRuleDal _ruleDal;
        private readonly IProgramEntityDal _programDal;
        private readonly ICourseOfferingDal _offeringDal;
        private readonly ICourseEvaluationDal _evaluationDal;
        private readonly ICourseEvaluationLetterGradeRuleDal _evalRuleDal;
        private readonly IMapper _mapper;
        private readonly IMudekEvaluationCalculatorService _mudekStale;

        public ProgramLetterGradeRuleService(
            IProgramLetterGradeRuleDal ruleDal,
            IProgramEntityDal programDal,
            ICourseOfferingDal offeringDal,
            ICourseEvaluationDal evaluationDal,
            ICourseEvaluationLetterGradeRuleDal evalRuleDal,
            IMapper mapper,
            IMudekEvaluationCalculatorService mudekStale)
        {
            _ruleDal = ruleDal;
            _programDal = programDal;
            _offeringDal = offeringDal;
            _evaluationDal = evaluationDal;
            _evalRuleDal = evalRuleDal;
            _mapper = mapper;
            _mudekStale = mudekStale;
        }

        public async Task<List<ProgramLetterGradeRuleDto>> GetByProgramIdAsync(Guid programEntityId)
        {
            if (!await _programDal.ExistsAsync(programEntityId))
                throw new KeyNotFoundException("Program bulunamadı.");
            var list = await _ruleDal.GetByProgramIdAsync(programEntityId);
            return _mapper.Map<List<ProgramLetterGradeRuleDto>>(list);
        }

        public async Task<ProgramLetterGradeRuleDto> AddAsync(CreateProgramLetterGradeRuleDto dto)
        {
            if (!await _programDal.ExistsAsync(dto.ProgramEntityId))
                throw new KeyNotFoundException("Program bulunamadı.");
            ValidateRange(dto.MinScore, dto.MaxScore);
            if (await _ruleDal.ExistsLetterAsync(dto.ProgramEntityId, dto.LetterGrade))
                throw new InvalidOperationException("Bu harf notu bu program için zaten tanımlı.");
            await ValidateNoOverlapAsync(dto.ProgramEntityId, dto.MinScore, dto.MaxScore, null);

            var entity = _mapper.Map<ProgramLetterGradeRule>(dto);
            entity.CreatedAt = DateTime.UtcNow;
            var added = _ruleDal.Add(entity);
            await _ruleDal.SaveChangesAsync();
            await _mudekStale.MarkStaleByProgramEntityIdAsync(dto.ProgramEntityId);
            return _mapper.Map<ProgramLetterGradeRuleDto>(added);
        }

        public async Task<ProgramLetterGradeRuleDto> UpdateAsync(UpdateProgramLetterGradeRuleDto dto)
        {
            var existing = await _ruleDal.GetAsync(r => r.Id == dto.Id)
                ?? throw new KeyNotFoundException("Kural bulunamadı.");
            ValidateRange(dto.MinScore, dto.MaxScore);
            if (await _ruleDal.ExistsLetterAsync(existing.ProgramEntityId, dto.LetterGrade, dto.Id))
                throw new InvalidOperationException("Bu harf notu bu program için zaten tanımlı.");
            await ValidateNoOverlapAsync(existing.ProgramEntityId, dto.MinScore, dto.MaxScore, dto.Id);

            _mapper.Map(dto, existing);
            _ruleDal.Update(existing);
            await _ruleDal.SaveChangesAsync();
            await _mudekStale.MarkStaleByProgramEntityIdAsync(existing.ProgramEntityId);
            return _mapper.Map<ProgramLetterGradeRuleDto>(existing);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _ruleDal.GetAsync(r => r.Id == id)
                ?? throw new KeyNotFoundException("Kural bulunamadı.");
            var programId = entity.ProgramEntityId;
            _ruleDal.Delete(entity);
            await _ruleDal.SaveChangesAsync();
            await _mudekStale.MarkStaleByProgramEntityIdAsync(programId);
        }

        public async Task<EffectiveLetterGradeRulesResponseDto> GetEffectiveForTeacherOfferingAsync(Guid offeringId, Guid teacherId)
        {
            var offering = await _offeringDal.GetByIdAndTeacherIdWithDetailsAsync(offeringId, teacherId)
                ?? throw new UnauthorizedAccessException("Bu ders açılışı size ait değil veya bulunamadı.");

            var programId = offering.Course.ProgramEntityId;
            var programRules = await _ruleDal.GetByProgramIdAsync(programId);
            if (programRules.Count > 0)
            {
                return new EffectiveLetterGradeRulesResponseDto
                {
                    Source = "Program",
                    Rules = _mapper.Map<List<ProgramLetterGradeRuleDto>>(programRules)
                };
            }

            var evaluation = await _evaluationDal.GetByOfferingIdWithDetailsAsync(offeringId);
            if (evaluation == null)
            {
                return new EffectiveLetterGradeRulesResponseDto
                {
                    Source = "CourseEvaluation",
                    Rules = new List<ProgramLetterGradeRuleDto>()
                };
            }

            var evalRules = await _evalRuleDal.GetByEvaluationIdAsync(evaluation.Id);
            var mapped = evalRules.Select(r => new ProgramLetterGradeRuleDto
            {
                Id = r.Id,
                ProgramEntityId = Guid.Empty,
                LetterGrade = r.LetterGrade,
                MinScore = r.MinScore,
                MaxScore = r.MaxScore,
                IsPassing = r.IsPassing,
                MinimumFinalScore = r.MinimumFinalScore,
                Description = r.Description,
                CreatedAt = r.CreatedAt
            }).ToList();

            return new EffectiveLetterGradeRulesResponseDto
            {
                Source = "CourseEvaluation",
                Rules = mapped
            };
        }

        private static void ValidateRange(decimal min, decimal max)
        {
            if (min > max) throw new InvalidOperationException("MinScore MaxScore'dan büyük olamaz.");
        }

        private async Task ValidateNoOverlapAsync(Guid programEntityId, decimal min, decimal max, Guid? excludeId)
        {
            var rules = await _ruleDal.GetByProgramIdAsync(programEntityId);
            if (excludeId.HasValue) rules = rules.Where(r => r.Id != excludeId.Value).ToList();
            var overlap = rules.Any(r => min <= r.MaxScore && max >= r.MinScore);
            if (overlap) throw new InvalidOperationException("Puan aralığı mevcut kurallarla çakışıyor.");
        }
    }
}
