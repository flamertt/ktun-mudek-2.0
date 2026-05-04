using System.ComponentModel.DataAnnotations;

namespace BitirmeApi.Business.DTO
{
    public class StudentCourseDto
    {
        public int ExternalCourseOfferingId { get; set; }
        public int ExternalCourseId { get; set; }
        public string CourseCode { get; set; } = default!;
        public string CourseName { get; set; } = default!;
        public int ExternalProgramId { get; set; }
        public int ActiveSurveyCount { get; set; }
    }

    public class StudentSurveyListDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public int QuestionCount { get; set; }
        public bool HasSubmitted { get; set; }
    }

    public class StudentSurveyDetailDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public bool HasSubmitted { get; set; }
        public List<StudentSurveyQuestionDto> Questions { get; set; } = new();
    }

    public class StudentSurveyQuestionDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = default!;
        public int OrderIndex { get; set; }
        public bool IsRequired { get; set; }
        public int ScaleMin { get; set; }
        public int ScaleMax { get; set; }
    }

    public class SubmitSurveyDto
    {
        [Required, MinLength(1, ErrorMessage = "En az bir cevap gereklidir.")]
        public List<SurveyAnswerInputDto> Answers { get; set; } = new();
    }

    public class SurveyAnswerInputDto
    {
        [Required]
        public Guid QuestionId { get; set; }

        [Required, Range(0, 10)]
        public decimal ValueNumeric { get; set; }
    }

    public class StudentSubmissionResultDto
    {
        public Guid SubmissionId { get; set; }
        public Guid SurveyId { get; set; }
        public int AnsweredQuestions { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
