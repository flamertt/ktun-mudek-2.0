using BitirmeApi.Core.EntityFramework;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.DataAccess.Concrete.EntityFramework.Context;
using BitirmeApi.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitirmeApi.DataAccess.Concrete.EntityFramework
{
    public class ExamDal : EfRepository<Exam, ProjectDbContext>, IExamDal
    {
        public ExamDal(ProjectDbContext context) : base(context) { }

        public async Task<List<Exam>> GetByEvaluationIdAsync(Guid evaluationId) =>
            await _context.Exams
                .Where(e => e.CourseEvaluationId == evaluationId)
                .Include(e => e.Questions)
                .OrderBy(e => e.OrderIndex)
                .AsNoTracking()
                .ToListAsync();

        public async Task<Exam?> GetByIdWithDetailsAsync(Guid id) =>
            await _context.Exams
                .Where(e => e.Id == id)
                .Include(e => e.Questions)
                    .ThenInclude(q => q.OutcomeMappings)
                .AsNoTracking()
                .FirstOrDefaultAsync();

        public async Task<Exam?> GetByIdWithOwnershipAsync(Guid id) =>
            await _context.Exams
                .Where(e => e.Id == id)
                .Include(e => e.CourseEvaluation)
                .AsNoTracking()
                .FirstOrDefaultAsync();

        public async Task<bool> ExistsAsync(Guid id) =>
            await _context.Exams.AnyAsync(e => e.Id == id);
    }
}
