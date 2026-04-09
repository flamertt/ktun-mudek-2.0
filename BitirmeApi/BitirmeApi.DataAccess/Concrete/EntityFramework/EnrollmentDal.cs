using BitirmeApi.Core.EntityFramework;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.DataAccess.Concrete.EntityFramework.Context;
using BitirmeApi.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitirmeApi.DataAccess.Concrete.EntityFramework
{
    public class EnrollmentDal : EfRepository<Enrollment, ProjectDbContext>, IEnrollmentDal
    {
        public EnrollmentDal(ProjectDbContext context) : base(context) { }

        public async Task<List<Enrollment>> GetByOfferingWithDetailsAsync(Guid courseOfferingId) =>
            await _context.Enrollments
                .Include(e => e.Student)
                .Where(e => e.CourseOfferingId == courseOfferingId)
                .AsNoTracking()
                .ToListAsync();

        public async Task<List<Enrollment>> GetByStudentIdWithDetailsAsync(Guid studentId) =>
            await _context.Enrollments
                .Include(e => e.CourseOffering)
                    .ThenInclude(o => o.Course)
                .Include(e => e.CourseOffering)
                    .ThenInclude(o => o.AcademicTerm)
                .Include(e => e.CourseOffering)
                    .ThenInclude(o => o.Teacher)
                .Where(e => e.StudentId == studentId)
                .AsNoTracking()
                .ToListAsync();

        public async Task<bool> IsEnrolledAsync(Guid courseOfferingId, Guid studentId) =>
            await _context.Enrollments.AnyAsync(e => e.CourseOfferingId == courseOfferingId && e.StudentId == studentId);

        public async Task<Enrollment?> GetByOfferingAndStudentAsync(Guid courseOfferingId, Guid studentId) =>
            await _context.Enrollments.FirstOrDefaultAsync(e =>
                e.CourseOfferingId == courseOfferingId && e.StudentId == studentId);

        public async Task<bool> HasAssociatedDataAsync(Guid enrollmentId) =>
            await _context.StudentAnswers.AnyAsync(a => a.EnrollmentId == enrollmentId) ||
            await _context.StudentAssessmentComponentScores.AnyAsync(s => s.EnrollmentId == enrollmentId);

        public async Task<List<Enrollment>> GetActiveTermEnrollmentsByStudentAsync(Guid studentId) =>
            await _context.Enrollments
                .Where(e => e.StudentId == studentId && e.CourseOffering.AcademicTerm.IsActive)
                .Include(e => e.CourseOffering)
                    .ThenInclude(o => o.Course)
                .Include(e => e.CourseOffering)
                    .ThenInclude(o => o.AcademicTerm)
                .Include(e => e.CourseOffering)
                    .ThenInclude(o => o.Teacher)
                .Include(e => e.CourseOffering)
                    .ThenInclude(o => o.Surveys.Where(s => s.IsActive))
                .AsNoTracking()
                .ToListAsync();

        public async Task<bool> IsEnrolledInActiveTermAsync(Guid offeringId, Guid studentId) =>
            await _context.Enrollments.AnyAsync(e =>
                e.CourseOfferingId == offeringId &&
                e.StudentId == studentId &&
                e.CourseOffering.AcademicTerm.IsActive);

        public async Task<int> GetCountByOfferingAsync(Guid courseOfferingId) =>
            await _context.Enrollments.CountAsync(e => e.CourseOfferingId == courseOfferingId);
    }
}
