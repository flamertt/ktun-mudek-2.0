using AutoMapper;
using BitirmeApi.Business.DTO;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.Business.Helpers
{
    public class AutoMapperHelper : Profile
    {
        public AutoMapperHelper()
        {
            // ── CourseEvaluation ──────────────────────────────────────────────────
            // Denormalized alanlar (CourseCode, CourseName, AcademicTermName) direkt entity'de tutuluyor
            CreateMap<CourseEvaluation, CourseEvaluationListDto>();
            CreateMap<CourseEvaluation, CourseEvaluationDetailDto>();

            // ── Exam ─────────────────────────────────────────────────────────────
            CreateMap<Exam, ExamListDto>()
                .ForMember(d => d.QuestionCount, o => o.MapFrom(s =>
                    s.Questions != null ? s.Questions.Count : 0));
            CreateMap<Exam, ExamDetailDto>();

            // ── ExamQuestion ──────────────────────────────────────────────────────
            CreateMap<ExamQuestion, ExamQuestionDto>()
                .ForMember(d => d.OutcomeMappings, o => o.MapFrom(s => s.OutcomeMappings));

            // ── ExamQuestionOutcomeMapping ─────────────────────────────────────────
            // CloCode ve CloDescription artık entity'de denormalized tutuluyor
            CreateMap<ExamQuestionOutcomeMapping, ExamQuestionOutcomeMappingDto>();
            CreateMap<CreateExamQuestionOutcomeMappingDto, ExamQuestionOutcomeMapping>();
            CreateMap<UpdateExamQuestionOutcomeMappingDto, ExamQuestionOutcomeMapping>();

            // ── AssessmentComponent ───────────────────────────────────────────────
            CreateMap<AssessmentComponent, AssessmentComponentListDto>();
            CreateMap<AssessmentComponent, AssessmentComponentDto>()
                .ForMember(d => d.ExamType, o => o.MapFrom(s =>
                    s.Exam != null ? s.Exam.ExamType : null));
            CreateMap<CreateAssessmentComponentDto, AssessmentComponent>();
            CreateMap<UpdateAssessmentComponentDto, AssessmentComponent>();

            // ── AssessmentComponentOutcomeMapping ─────────────────────────────────
            // CloCode ve CloDescription entity'de denormalized
            CreateMap<AssessmentComponentOutcomeMapping, AssessmentComponentOutcomeMappingDto>()
                .ForMember(d => d.ComponentName, o => o.MapFrom(s =>
                    s.AssessmentComponent != null ? s.AssessmentComponent.Name : null));
            CreateMap<CreateAssessmentComponentOutcomeMappingDto, AssessmentComponentOutcomeMapping>();
            CreateMap<UpdateAssessmentComponentOutcomeMappingDto, AssessmentComponentOutcomeMapping>();

            // ── StudentAnswer ─────────────────────────────────────────────────────
            CreateMap<StudentAnswer, StudentAnswerDto>();
            CreateMap<CreateStudentAnswerDto, StudentAnswer>();
            CreateMap<UpdateStudentAnswerDto, StudentAnswer>();

            // ── StudentAssessmentComponentScore ───────────────────────────────────
            CreateMap<StudentAssessmentComponentScore, StudentAssessmentComponentScoreDto>()
                .ForMember(d => d.ComponentName, o => o.MapFrom(s =>
                    s.AssessmentComponent != null ? s.AssessmentComponent.Name : null))
                .ForMember(d => d.ComponentType, o => o.MapFrom(s =>
                    s.AssessmentComponent != null ? s.AssessmentComponent.ComponentType : null))
                .ForMember(d => d.MaxScore, o => o.MapFrom(s =>
                    s.AssessmentComponent != null ? (decimal?)s.AssessmentComponent.MaxScore : null));
            CreateMap<CreateStudentAssessmentComponentScoreDto, StudentAssessmentComponentScore>();
            CreateMap<UpdateStudentAssessmentComponentScoreDto, StudentAssessmentComponentScore>();

            // ── LetterGradeRule (program bazlı) ─────────────────────────────────
            CreateMap<LetterGradeRule, LetterGradeRuleDto>();
            CreateMap<CreateLetterGradeRuleDto, LetterGradeRule>();

            // ── Survey ─────────────────────────────────────────────────────────────
            CreateMap<Survey, SurveyListDto>()
                .ForMember(d => d.QuestionCount, o => o.MapFrom(s =>
                    s.Questions != null ? s.Questions.Count : 0))
                .ForMember(d => d.SubmissionCount, o => o.MapFrom(s =>
                    s.Submissions != null ? s.Submissions.Count : 0));

            CreateMap<Survey, SurveyDetailDto>()
                .ForMember(d => d.SubmissionCount, o => o.MapFrom(s =>
                    s.Submissions != null ? s.Submissions.Count : 0))
                .ForMember(d => d.Questions, o => o.MapFrom(s => s.Questions));

            // ── Question (Survey sorusu) ──────────────────────────────────────────
            // CloCode ve CloDescription artık Question entity'sinde denormalized
            CreateMap<Question, SurveyQuestionDto>();
        }
    }
}
