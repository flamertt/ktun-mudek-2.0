using BitirmeApi.Core.EntityFramework;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.DataAccess.Concrete.EntityFramework.Context;
using BitirmeApi.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitirmeApi.DataAccess.Concrete.EntityFramework
{
    public class SubmissionDal : EfRepository<Submission, ProjectDbContext>, ISubmissionDal
    {
        public SubmissionDal(ProjectDbContext context) : base(context) { }

        public async Task<List<Submission>> GetBySurveyIdWithAnswersAsync(Guid surveyId) =>
            await _context.Submissions
                .Where(s => s.SurveyId == surveyId)
                .Include(s => s.Answers)
                .AsNoTracking()
                .ToListAsync();

        public async Task<int> CountBySurveyIdAsync(Guid surveyId) =>
            await _context.Submissions.CountAsync(s => s.SurveyId == surveyId);

        public async Task<bool> HasStudentSubmittedAsync(Guid surveyId, Guid userId) =>
            await _context.Submissions.AnyAsync(s => s.SurveyId == surveyId && s.UserId == userId);
    }
}
