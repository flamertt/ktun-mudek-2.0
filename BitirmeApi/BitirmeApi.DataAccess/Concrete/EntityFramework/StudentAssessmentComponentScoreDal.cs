using BitirmeApi.Core.EntityFramework;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.DataAccess.Concrete.EntityFramework.Context;
using BitirmeApi.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitirmeApi.DataAccess.Concrete.EntityFramework
{
    public class StudentAssessmentComponentScoreDal : EfRepository<StudentAssessmentComponentScore, ProjectDbContext>, IStudentAssessmentComponentScoreDal
    {
        public StudentAssessmentComponentScoreDal(ProjectDbContext context) : base(context)
        {
        }

        public async Task<List<StudentAssessmentComponentScore>> GetByComponentIdWithDetailsAsync(Guid componentId) =>
            await _context.StudentAssessmentComponentScores
                .Where(s => s.AssessmentComponentId == componentId)
                .Include(s => s.Enrollment)
                    .ThenInclude(e => e.Student)
                .AsNoTracking()
                .ToListAsync();

        public async Task<StudentAssessmentComponentScore?> GetByIdWithOwnershipAsync(Guid id) =>
            await _context.StudentAssessmentComponentScores
                .Where(s => s.Id == id)
                .Include(s => s.AssessmentComponent)
                    .ThenInclude(c => c.Exam)
                        .ThenInclude(e => e.CourseEvaluation)
                            .ThenInclude(ev => ev.CourseOffering)
                .Include(s => s.Enrollment)
                .AsNoTracking()
                .FirstOrDefaultAsync();

        public async Task<StudentAssessmentComponentScore?> GetByComponentAndEnrollmentAsync(Guid componentId, Guid enrollmentId) =>
            await _context.StudentAssessmentComponentScores.FirstOrDefaultAsync(s =>
                s.AssessmentComponentId == componentId && s.EnrollmentId == enrollmentId);

        public async Task<bool> ExistsAsync(Guid componentId, Guid enrollmentId) =>
            await _context.StudentAssessmentComponentScores.AnyAsync(s =>
                s.AssessmentComponentId == componentId && s.EnrollmentId == enrollmentId);
    }
}
