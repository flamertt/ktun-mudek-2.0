using BitirmeApi.Core.EntityFramework;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.DataAccess.Concrete.EntityFramework.Context;
using BitirmeApi.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitirmeApi.DataAccess.Concrete.EntityFramework
{
    public class ExamQuestionDal : EfRepository<ExamQuestion, ProjectDbContext>, IExamQuestionDal
    {
        public ExamQuestionDal(ProjectDbContext context) : base(context) { }

        public async Task<List<ExamQuestion>> GetByExamIdWithDetailsAsync(Guid examId) =>
            await _context.ExamQuestions
                .Where(q => q.ExamId == examId)
                .Include(q => q.OutcomeMappings)
                    .ThenInclude(m => m.CourseLearningOutcome)
                .OrderBy(q => q.QuestionNumber)
                .AsNoTracking()
                .ToListAsync();

        public async Task<ExamQuestion?> GetByIdWithDetailsAsync(Guid id) =>
            await _context.ExamQuestions
                .Where(q => q.Id == id)
                .Include(q => q.OutcomeMappings)
                    .ThenInclude(m => m.CourseLearningOutcome)
                .AsNoTracking()
                .FirstOrDefaultAsync();

        public async Task<ExamQuestion?> GetByIdWithOwnershipAsync(Guid id) =>
            await _context.ExamQuestions
                .Where(q => q.Id == id)
                .Include(q => q.Exam)
                    .ThenInclude(e => e.CourseEvaluation)
                        .ThenInclude(ev => ev.CourseOffering)
                .AsNoTracking()
                .FirstOrDefaultAsync();

        public async Task<bool> ExistsAsync(Guid id) =>
            await _context.ExamQuestions.AnyAsync(q => q.Id == id);
    }
}
