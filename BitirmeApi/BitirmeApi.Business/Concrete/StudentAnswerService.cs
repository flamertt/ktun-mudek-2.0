using BitirmeApi.Business.Abstract;
using BitirmeApi.Business.DTO;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.Business.Concrete
{
    public class StudentAnswerService:IStudentAnswerService
    {
        private readonly IStudentAnswerDal _studentAnswerDal;
        private readonly IExamQuestionDal _questionDal;
        private readonly IEnrollmentDal _enrollmentDal;
        public StudentAnswerService(
            IStudentAnswerDal studentAnswerDal,
            IExamQuestionDal questionDal,
            IEnrollmentDal enrollmentDal)
        {
            _studentAnswerDal = studentAnswerDal;
            _questionDal = questionDal;
            _enrollmentDal = enrollmentDal;
        }

        public async Task<List<StudentAnswerDto>> GetByQuestionForTeacherAsync(Guid questionId, Guid teacherId)
        {
            var question = await EnsureQuestionOwnershipAsync(questionId, teacherId);
            var answers = await _studentAnswerDal.GetByQuestionIdWithDetailsAsync(questionId);
            return answers.Select(a => new StudentAnswerDto
            {
                Id = a.Id,
                ExamQuestionId = a.ExamQuestionId,
                EnrollmentId = a.EnrollmentId,
                Score = a.Score,
                StudentNumber = a.Enrollment?.Student?.StudentNumber,
                StudentName = a.Enrollment?.Student?.FullName
            }).ToList();
        }

        public async Task<StudentAnswerDto> AddForTeacherAsync(CreateStudentAnswerDto dto, Guid teacherId)
        {
            var question = await EnsureQuestionOwnershipAsync(dto.ExamQuestionId, teacherId);
            var enrollment = await ValidateEnrollmentForQuestionAsync(dto.EnrollmentId, question);
            if (dto.Score < 0 || dto.Score > question.MaxScore)
                throw new InvalidOperationException($"Score 0 ile {question.MaxScore} arasında olmalıdır.");
            if (await _studentAnswerDal.ExistsAsync(dto.ExamQuestionId, dto.EnrollmentId))
                throw new InvalidOperationException("Bu soru ve öğrenci kaydı için cevap zaten mevcut.");

            var entity = new StudentAnswer
            {
                Id = Guid.NewGuid(),
                ExamQuestionId = dto.ExamQuestionId,
                EnrollmentId = dto.EnrollmentId,
                Score = dto.Score
            };
            _studentAnswerDal.Add(entity);
            await _studentAnswerDal.SaveChangesAsync();
            return new StudentAnswerDto
            {
                Id = entity.Id,
                ExamQuestionId = entity.ExamQuestionId,
                EnrollmentId = entity.EnrollmentId,
                Score = entity.Score,
                StudentNumber = enrollment.Student?.StudentNumber,
                StudentName = enrollment.Student?.FullName
            };
        }

        public async Task<BulkOperationResultDto<Guid>> AddBulkForTeacherAsync(Guid questionId, List<BulkStudentAnswerItemDto> items, Guid teacherId)
        {
            var result = new BulkOperationResultDto<Guid>();
            foreach (var item in items)
            {
                try
                {
                    await AddForTeacherAsync(new CreateStudentAnswerDto
                    {
                        ExamQuestionId = questionId,
                        EnrollmentId = item.EnrollmentId,
                        Score = item.Score
                    }, teacherId);
                    result.Success.Add(item.EnrollmentId);
                }
                catch (Exception ex)
                {
                    result.Failed.Add(item.EnrollmentId);
                    result.Errors.Add($"{item.EnrollmentId}: {ex.Message}");
                }
            }
            return result;
        }

        public async Task<StudentAnswerDto> UpdateForTeacherAsync(UpdateStudentAnswerDto dto, Guid teacherId)
        {
            var snapshot = await _studentAnswerDal.GetByIdWithOwnershipAsync(dto.Id)
                ?? throw new KeyNotFoundException("Cevap bulunamadı.");
            if (snapshot.ExamQuestion?.Exam?.CourseEvaluation?.CourseOffering?.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bu cevap sizin dersinize ait değil.");
            if (dto.Score < 0 || dto.Score > snapshot.ExamQuestion.MaxScore)
                throw new InvalidOperationException($"Score 0 ile {snapshot.ExamQuestion.MaxScore} arasında olmalıdır.");

            var tracked = await _studentAnswerDal.GetAsync(a => a.Id == dto.Id)
                ?? throw new KeyNotFoundException("Cevap bulunamadı.");
            tracked.Score = dto.Score;
            _studentAnswerDal.Update(tracked);
            await _studentAnswerDal.SaveChangesAsync();
            return new StudentAnswerDto
            {
                Id = tracked.Id,
                ExamQuestionId = tracked.ExamQuestionId,
                EnrollmentId = tracked.EnrollmentId,
                Score = tracked.Score
            };
        }

        public async Task DeleteForTeacherAsync(Guid id, Guid teacherId)
        {
            var snapshot = await _studentAnswerDal.GetByIdWithOwnershipAsync(id)
                ?? throw new KeyNotFoundException("Cevap bulunamadı.");
            if (snapshot.ExamQuestion?.Exam?.CourseEvaluation?.CourseOffering?.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bu cevap sizin dersinize ait değil.");
            var tracked = await _studentAnswerDal.GetAsync(a => a.Id == id)
                ?? throw new KeyNotFoundException("Cevap bulunamadı.");
            _studentAnswerDal.Delete(tracked);
            await _studentAnswerDal.SaveChangesAsync();
        }

        private async Task<ExamQuestion> EnsureQuestionOwnershipAsync(Guid questionId, Guid teacherId)
        {
            var question = await _questionDal.GetByIdWithOwnershipAsync(questionId)
                ?? throw new KeyNotFoundException("Soru bulunamadı.");
            if (question.Exam?.CourseEvaluation?.CourseOffering?.TeacherId != teacherId)
                throw new UnauthorizedAccessException("Bu soru sizin dersinize ait değil.");
            return question;
        }

        private async Task<Enrollment> ValidateEnrollmentForQuestionAsync(Guid enrollmentId, ExamQuestion question)
        {
            var enrollment = await _enrollmentDal.GetAsync(e => e.Id == enrollmentId)
                ?? throw new KeyNotFoundException("Enrollment bulunamadı.");
            var offeringId = question.Exam?.CourseEvaluation?.CourseOfferingId;
            if (enrollment.CourseOfferingId != offeringId)
                throw new InvalidOperationException("Enrollment bu sorunun ders açılışına ait değil.");
            return enrollment;
        }
    }
}
