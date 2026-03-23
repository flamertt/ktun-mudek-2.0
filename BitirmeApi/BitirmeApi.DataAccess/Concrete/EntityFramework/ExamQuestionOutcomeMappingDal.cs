using BitirmeApi.Core.EntityFramework;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.DataAccess.Concrete.EntityFramework.Context;
using BitirmeApi.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitirmeApi.DataAccess.Concrete.EntityFramework
{
    public class ExamQuestionOutcomeMappingDal : EfRepository<ExamQuestionOutcomeMapping, ProjectDbContext>, IExamQuestionOutcomeMappingDal
    {
        public ExamQuestionOutcomeMappingDal(ProjectDbContext context) : base(context) { }

        public async Task<List<ExamQuestionOutcomeMapping>> GetByQuestionIdWithDetailsAsync(Guid questionId) =>
            await _context.ExamQuestionOutcomeMappings
                .Where(m => m.ExamQuestionId == questionId)
                .Include(m => m.ExamQuestion)
                .Include(m => m.CourseLearningOutcome)
                .AsNoTracking()
                .ToListAsync();

        public async Task<ExamQuestionOutcomeMapping?> GetByIdWithDetailsAsync(Guid id) =>
            await _context.ExamQuestionOutcomeMappings
                .Where(m => m.Id == id)
                .Include(m => m.ExamQuestion)
                .Include(m => m.CourseLearningOutcome)
                .AsNoTracking()
                .FirstOrDefaultAsync();

        public async Task<ExamQuestionOutcomeMapping?> GetByIdWithOwnershipAsync(Guid id) =>
            await _context.ExamQuestionOutcomeMappings
                .Where(m => m.Id == id)
                .Include(m => m.ExamQuestion)
                    .ThenInclude(q => q.Exam)
                        .ThenInclude(e => e.CourseEvaluation)
                            .ThenInclude(ev => ev.CourseOffering)
                .AsNoTracking()
                .FirstOrDefaultAsync();

        public async Task<ExamQuestionOutcomeMapping?> GetByIdsAsync(Guid questionId, Guid cloId) =>
            await _context.ExamQuestionOutcomeMappings
                .FirstOrDefaultAsync(m => m.ExamQuestionId == questionId && m.CourseLearningOutcomeId == cloId);

        public async Task<bool> ExistsAsync(Guid questionId, Guid cloId) =>
            await _context.ExamQuestionOutcomeMappings
                .AnyAsync(m => m.ExamQuestionId == questionId && m.CourseLearningOutcomeId == cloId);
    }
}
