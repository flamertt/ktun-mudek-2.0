using BitirmeApi.Core.EntityFramework;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.DataAccess.Concrete.EntityFramework.Context;
using BitirmeApi.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitirmeApi.DataAccess.Concrete.EntityFramework
{
    public class StudentAssessmentComponentScoreDal : EfRepository<StudentAssessmentComponentScore, ProjectDbContext>, IStudentAssessmentComponentScoreDal
    {
        public StudentAssessmentComponentScoreDal(ProjectDbContext context) : base(context) { }

        public async Task<List<StudentAssessmentComponentScore>> GetByComponentIdWithDetailsAsync(Guid componentId) =>
            await _context.StudentAssessmentComponentScores
                .Where(s => s.AssessmentComponentId == componentId)
                .AsNoTracking()
                .ToListAsync();

        public async Task<StudentAssessmentComponentScore?> GetByIdWithOwnershipAsync(Guid id) =>
            await _context.StudentAssessmentComponentScores
                .Where(s => s.Id == id)
                .Include(s => s.AssessmentComponent)
                    .ThenInclude(c => c.Exam)
                        .ThenInclude(e => e.CourseEvaluation)
                .AsNoTracking()
                .FirstOrDefaultAsync();

        public async Task<StudentAssessmentComponentScore?> GetByComponentAndStudentAsync(Guid componentId, int externalStudentId) =>
            await _context.StudentAssessmentComponentScores.FirstOrDefaultAsync(s =>
                s.AssessmentComponentId == componentId && s.ExternalStudentId == externalStudentId);

        public async Task<bool> ExistsAsync(Guid componentId, int externalStudentId) =>
            await _context.StudentAssessmentComponentScores.AnyAsync(s =>
                s.AssessmentComponentId == componentId && s.ExternalStudentId == externalStudentId);
    }
}
