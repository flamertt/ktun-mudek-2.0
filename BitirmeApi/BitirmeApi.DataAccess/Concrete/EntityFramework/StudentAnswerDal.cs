using BitirmeApi.Core.EntityFramework;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.DataAccess.Concrete.EntityFramework.Context;
using BitirmeApi.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitirmeApi.DataAccess.Concrete.EntityFramework
{
    public class StudentAnswerDal:EfRepository<StudentAnswer,ProjectDbContext>,IStudentAnswerDal
    {
        public StudentAnswerDal(ProjectDbContext context):base(context)
        {
            
        }

        public async Task<List<StudentAnswer>> GetByQuestionIdWithDetailsAsync(Guid questionId) =>
            await _context.StudentAnswers
                .Where(a => a.ExamQuestionId == questionId)
                .Include(a => a.Enrollment)
                    .ThenInclude(e => e.Student)
                .AsNoTracking()
                .ToListAsync();

        public async Task<StudentAnswer?> GetByIdWithOwnershipAsync(Guid id) =>
            await _context.StudentAnswers
                .Where(a => a.Id == id)
                .Include(a => a.ExamQuestion)
                    .ThenInclude(q => q.Exam)
                        .ThenInclude(e => e.CourseEvaluation)
                            .ThenInclude(ev => ev.CourseOffering)
                .Include(a => a.Enrollment)
                .AsNoTracking()
                .FirstOrDefaultAsync();

        public async Task<StudentAnswer?> GetByQuestionAndEnrollmentAsync(Guid questionId, Guid enrollmentId) =>
            await _context.StudentAnswers.FirstOrDefaultAsync(a =>
                a.ExamQuestionId == questionId && a.EnrollmentId == enrollmentId);

        public async Task<bool> ExistsAsync(Guid questionId, Guid enrollmentId) =>
            await _context.StudentAnswers.AnyAsync(a =>
                a.ExamQuestionId == questionId && a.EnrollmentId == enrollmentId);
    }
}
