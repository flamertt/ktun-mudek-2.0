using BitirmeApi.Core.EntityFramework;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.DataAccess.Concrete.EntityFramework.Context;
using BitirmeApi.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitirmeApi.DataAccess.Concrete.EntityFramework
{
    public class QuestionDal : EfRepository<Question, ProjectDbContext>, IQuestionDal
    {
        public QuestionDal(ProjectDbContext context) : base(context) { }

        public async Task<List<Question>> GetBySurveyIdAsync(Guid surveyId) =>
            await _context.Questions
                .Where(q => q.SurveyId == surveyId)
                .OrderBy(q => q.OrderIndex)
                .AsNoTracking()
                .ToListAsync();

        public async Task<Question?> GetByIdWithSurveyAsync(Guid id) =>
            await _context.Questions
                .Where(q => q.Id == id)
                .Include(q => q.Survey)
                    .ThenInclude(s => s.CourseOffering)
                .Include(q => q.CourseLearningOutcome)
                .AsNoTracking()
                .FirstOrDefaultAsync();
    }
}
