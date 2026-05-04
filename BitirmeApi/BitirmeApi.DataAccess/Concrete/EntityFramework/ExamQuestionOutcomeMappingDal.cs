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
                .AsNoTracking()
                .ToListAsync();

        public async Task<ExamQuestionOutcomeMapping?> GetByIdWithDetailsAsync(Guid id) =>
            await _context.ExamQuestionOutcomeMappings
                .Where(m => m.Id == id)
                .Include(m => m.ExamQuestion)
                .AsNoTracking()
                .FirstOrDefaultAsync();

        public async Task<ExamQuestionOutcomeMapping?> GetByIdWithOwnershipAsync(Guid id) =>
            await _context.ExamQuestionOutcomeMappings
                .Where(m => m.Id == id)
                .Include(m => m.ExamQuestion)
                    .ThenInclude(q => q.Exam)
                        .ThenInclude(e => e.CourseEvaluation)
                .AsNoTracking()
                .FirstOrDefaultAsync();

        public async Task<ExamQuestionOutcomeMapping?> GetByIdsAsync(Guid questionId, int externalCloId) =>
            await _context.ExamQuestionOutcomeMappings
                .FirstOrDefaultAsync(m => m.ExamQuestionId == questionId && m.ExternalCloId == externalCloId);

        public async Task<bool> ExistsAsync(Guid questionId, int externalCloId) =>
            await _context.ExamQuestionOutcomeMappings
                .AnyAsync(m => m.ExamQuestionId == questionId && m.ExternalCloId == externalCloId);
    }
}
