using BitirmeApi.Core.EntityFramework;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.DataAccess.Concrete.EntityFramework.Context;
using BitirmeApi.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitirmeApi.DataAccess.Concrete.EntityFramework
{
    public class StudentAnswerDal : EfRepository<StudentAnswer, ProjectDbContext>, IStudentAnswerDal
    {
        public StudentAnswerDal(ProjectDbContext context) : base(context) { }

        public async Task<List<StudentAnswer>> GetByQuestionIdWithDetailsAsync(Guid questionId) =>
            await _context.StudentAnswers
                .Where(a => a.ExamQuestionId == questionId)
                .AsNoTracking()
                .ToListAsync();

        public async Task<StudentAnswer?> GetByIdWithOwnershipAsync(Guid id) =>
            await _context.StudentAnswers
                .Where(a => a.Id == id)
                .Include(a => a.ExamQuestion)
                    .ThenInclude(q => q.Exam)
                        .ThenInclude(e => e.CourseEvaluation)
                .AsNoTracking()
                .FirstOrDefaultAsync();

        public async Task<StudentAnswer?> GetByQuestionAndStudentAsync(Guid questionId, int externalStudentId) =>
            await _context.StudentAnswers.FirstOrDefaultAsync(a =>
                a.ExamQuestionId == questionId && a.ExternalStudentId == externalStudentId);

        public async Task<bool> ExistsAsync(Guid questionId, int externalStudentId) =>
            await _context.StudentAnswers.AnyAsync(a =>
                a.ExamQuestionId == questionId && a.ExternalStudentId == externalStudentId);
    }
}
