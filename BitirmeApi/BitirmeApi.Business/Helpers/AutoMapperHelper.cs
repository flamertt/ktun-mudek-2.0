using AutoMapper;
using BitirmeApi.Business.DTO;
using BitirmeApi.Entity.Entities;

namespace BitirmeApi.Business.Helpers
{
    public class AutoMapperHelper : Profile
    {
        public AutoMapperHelper()
        {
            // ── AppUser ───────────────────────────────────────────────────────────
            CreateMap<AppUser, AppUserDto>()
                .ForMember(d => d.ProgramName, o => o.MapFrom(s =>
                    s.Program != null ? s.Program.Name : null));

            CreateMap<AppUser, AppUserListDto>()
                .ForMember(d => d.ProgramName, o => o.MapFrom(s =>
                    s.Program != null ? s.Program.Name : null));

            CreateMap<CreateAppUserDto, AppUser>();
            CreateMap<UpdateAppUserDto, AppUser>();

            // ── Program ───────────────────────────────────────────────────────────
            CreateMap<ProgramEntity, ProgramEntityDto>();
            CreateMap<CreateProgramEntityDto, ProgramEntity>();
            CreateMap<UpdateProgramEntityDto, ProgramEntity>();

            CreateMap<ProgramOutcome, ProgramOutcomeDto>();
            CreateMap<CreateProgramOutcomeDto, ProgramOutcome>();
            CreateMap<UpdateProgramOutcomeDto, ProgramOutcome>();

            // ── AcademicTerm ──────────────────────────────────────────────────────
            CreateMap<AcademicTerm, AcademicTermListDto>();
            CreateMap<AcademicTerm, AcademicTermDto>()
                .ForMember(d => d.OfferingCount, o => o.MapFrom(s =>
                    s.CourseOfferings != null ? s.CourseOfferings.Count : 0));

            // ── Course (Katalog) ──────────────────────────────────────────────────
            CreateMap<Course, CourseListDto>()
                .ForMember(d => d.ProgramName, o => o.MapFrom(s =>
                    s.Program != null ? s.Program.Name : null));

            CreateMap<Course, CourseDto>()
                .ForMember(d => d.ProgramName, o => o.MapFrom(s =>
                    s.Program != null ? s.Program.Name : null));

            CreateMap<CreateCourseDto, Course>();
            CreateMap<UpdateCourseDto, Course>();

            // ── CourseOffering ────────────────────────────────────────────────────
            CreateMap<CourseOffering, CourseOfferingListDto>()
                .ForMember(d => d.CourseCode, o => o.MapFrom(s => s.Course.Code))
                .ForMember(d => d.CourseName, o => o.MapFrom(s => s.Course.Name))
                .ForMember(d => d.ProgramName, o => o.MapFrom(s =>
                    s.Course.Program != null ? s.Course.Program.Name : null))
                .ForMember(d => d.TermName, o => o.MapFrom(s => s.AcademicTerm.Name))
                .ForMember(d => d.TeacherName, o => o.MapFrom(s =>
                    s.Teacher != null ? s.Teacher.FullName : null))
                .ForMember(d => d.EnrolledCount, o => o.MapFrom(s =>
                    s.Enrollments != null ? s.Enrollments.Count : 0));

            CreateMap<CourseOffering, CourseOfferingDto>()
                .ForMember(d => d.CourseCode, o => o.MapFrom(s => s.Course.Code))
                .ForMember(d => d.CourseName, o => o.MapFrom(s => s.Course.Name))
                .ForMember(d => d.Credits, o => o.MapFrom(s => s.Course.Credits))
                .ForMember(d => d.CourseDescription, o => o.MapFrom(s => s.Course.Description))
                .ForMember(d => d.ProgramEntityId, o => o.MapFrom(s => s.Course.ProgramEntityId))
                .ForMember(d => d.ProgramName, o => o.MapFrom(s =>
                    s.Course.Program != null ? s.Course.Program.Name : null))
                .ForMember(d => d.TermName, o => o.MapFrom(s => s.AcademicTerm.Name))
                .ForMember(d => d.TeacherName, o => o.MapFrom(s =>
                    s.Teacher != null ? s.Teacher.FullName : null))
                .ForMember(d => d.EnrolledCount, o => o.MapFrom(s =>
                    s.Enrollments != null ? s.Enrollments.Count : 0))
                .ForMember(d => d.HasEvaluation, o => o.MapFrom(s =>
                    s.CourseEvaluation != null));

            // ── Enrollment ────────────────────────────────────────────────────────
            CreateMap<Enrollment, EnrollmentListDto>()
                .ForMember(d => d.StudentFullName, o => o.MapFrom(s => s.Student.FullName))
                .ForMember(d => d.StudentNumber, o => o.MapFrom(s => s.Student.StudentNumber));

            CreateMap<Enrollment, StudentEnrollmentHistoryDto>()
                .ForMember(d => d.EnrollmentId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.CourseCode, o => o.MapFrom(s => s.CourseOffering.Course.Code))
                .ForMember(d => d.CourseName, o => o.MapFrom(s => s.CourseOffering.Course.Name))
                .ForMember(d => d.TermName, o => o.MapFrom(s => s.CourseOffering.AcademicTerm.Name))
                .ForMember(d => d.TeacherName, o => o.MapFrom(s =>
                    s.CourseOffering.Teacher != null ? s.CourseOffering.Teacher.FullName : null))
                .ForMember(d => d.Section, o => o.MapFrom(s => s.CourseOffering.Section));

            // ── CourseEvaluation ──────────────────────────────────────────────────
            CreateMap<CourseEvaluation, CourseEvaluationListDto>()
                .ForMember(d => d.CourseCode, o => o.MapFrom(s => s.CourseOffering.Course.Code))
                .ForMember(d => d.CourseName, o => o.MapFrom(s => s.CourseOffering.Course.Name))
                .ForMember(d => d.TermName, o => o.MapFrom(s => s.CourseOffering.AcademicTerm.Name))
                .ForMember(d => d.TeacherName, o => o.MapFrom(s =>
                    s.CourseOffering.Teacher != null ? s.CourseOffering.Teacher.FullName : null))
                .ForMember(d => d.LastCalculatedAt, o => o.MapFrom(s => s.LastCalculatedAt))
                .ForMember(d => d.IsCalculationDirty, o => o.MapFrom(s => s.IsCalculationDirty));

            CreateMap<CourseEvaluation, CourseEvaluationDetailDto>()
                .ForMember(d => d.CourseCode, o => o.MapFrom(s => s.CourseOffering.Course.Code))
                .ForMember(d => d.CourseName, o => o.MapFrom(s => s.CourseOffering.Course.Name))
                .ForMember(d => d.TermName, o => o.MapFrom(s => s.CourseOffering.AcademicTerm.Name))
                .ForMember(d => d.Section, o => o.MapFrom(s => s.CourseOffering.Section))
                .ForMember(d => d.TeacherName, o => o.MapFrom(s =>
                    s.CourseOffering.Teacher != null ? s.CourseOffering.Teacher.FullName : null))
                .ForMember(d => d.ProgramName, o => o.MapFrom(s =>
                    s.CourseOffering.Course.Program != null ? s.CourseOffering.Course.Program.Name : null))
                .ForMember(d => d.StudentCount, o => o.MapFrom(s =>
                    s.CourseOffering.Enrollments != null ? s.CourseOffering.Enrollments.Count : 0))
                .ForMember(d => d.LastCalculatedAt, o => o.MapFrom(s => s.LastCalculatedAt))
                .ForMember(d => d.IsCalculationDirty, o => o.MapFrom(s => s.IsCalculationDirty));

            // ── CourseLearningOutcome (Katalog CLO) ───────────────────────────────
            CreateMap<CourseLearningOutcome, CourseLearningOutcomeDto>()
                .ForMember(d => d.CourseCode, o => o.MapFrom(s =>
                    s.Course != null ? s.Course.Code : null));
            CreateMap<CreateCourseLearningOutcomeDto, CourseLearningOutcome>();
            CreateMap<UpdateCourseLearningOutcomeDto, CourseLearningOutcome>();

            // ── Exam ─────────────────────────────────────────────────────────────
            CreateMap<Exam, ExamListDto>()
                .ForMember(d => d.QuestionCount, o => o.MapFrom(s =>
                    s.Questions != null ? s.Questions.Count : 0));

            CreateMap<Exam, ExamDetailDto>();

            // ── ExamQuestion ──────────────────────────────────────────────────────
            CreateMap<ExamQuestion, ExamQuestionDto>()
                .ForMember(d => d.OutcomeMappings, o => o.MapFrom(s => s.OutcomeMappings));

            // ── ExamQuestionOutcomeMapping ─────────────────────────────────────────
            CreateMap<ExamQuestionOutcomeMapping, ExamQuestionOutcomeMappingDto>()
                .ForMember(d => d.QuestionTitle, o => o.MapFrom(s =>
                    s.ExamQuestion != null ? s.ExamQuestion.Title : null))
                .ForMember(d => d.OutcomeCode, o => o.MapFrom(s =>
                    s.CourseLearningOutcome != null ? s.CourseLearningOutcome.Code : null))
                .ForMember(d => d.OutcomeDescription, o => o.MapFrom(s =>
                    s.CourseLearningOutcome != null ? s.CourseLearningOutcome.Description : null));

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
            CreateMap<AssessmentComponentOutcomeMapping, AssessmentComponentOutcomeMappingDto>()
                .ForMember(d => d.ComponentName, o => o.MapFrom(s =>
                    s.AssessmentComponent != null ? s.AssessmentComponent.Name : null))
                .ForMember(d => d.OutcomeCode, o => o.MapFrom(s =>
                    s.CourseLearningOutcome != null ? s.CourseLearningOutcome.Code : null))
                .ForMember(d => d.OutcomeDescription, o => o.MapFrom(s =>
                    s.CourseLearningOutcome != null ? s.CourseLearningOutcome.Description : null));
            CreateMap<CreateAssessmentComponentOutcomeMappingDto, AssessmentComponentOutcomeMapping>();
            CreateMap<UpdateAssessmentComponentOutcomeMappingDto, AssessmentComponentOutcomeMapping>();

            // ── StudentAssessmentComponentScore ───────────────────────────────────
            CreateMap<StudentAssessmentComponentScore, StudentAssessmentComponentScoreDto>()
                .ForMember(d => d.ComponentName, o => o.MapFrom(s =>
                    s.AssessmentComponent != null ? s.AssessmentComponent.Name : null))
                .ForMember(d => d.ComponentType, o => o.MapFrom(s =>
                    s.AssessmentComponent != null ? s.AssessmentComponent.ComponentType : null))
                .ForMember(d => d.MaxScore, o => o.MapFrom(s =>
                    s.AssessmentComponent != null ? (decimal?)s.AssessmentComponent.MaxScore : null))
                .ForMember(d => d.StudentNumber, o => o.MapFrom(s =>
                    s.Enrollment != null && s.Enrollment.Student != null ? s.Enrollment.Student.StudentNumber : null))
                .ForMember(d => d.StudentName, o => o.MapFrom(s =>
                    s.Enrollment != null && s.Enrollment.Student != null ? s.Enrollment.Student.FullName : null));
            CreateMap<CreateStudentAssessmentComponentScoreDto, StudentAssessmentComponentScore>();
            CreateMap<UpdateStudentAssessmentComponentScoreDto, StudentAssessmentComponentScore>();

            // ── LetterGradeRule ───────────────────────────────────────────────────
            CreateMap<CourseEvaluationLetterGradeRule, CourseEvaluationLetterGradeRuleDto>();
            CreateMap<CreateCourseEvaluationLetterGradeRuleDto, CourseEvaluationLetterGradeRule>();
            CreateMap<UpdateCourseEvaluationLetterGradeRuleDto, CourseEvaluationLetterGradeRule>();

            // ── CloPoMap ──────────────────────────────────────────────────────────
            CreateMap<CloPoMap, CloPoMapDto>()
                .ForMember(d => d.CloCode, o => o.MapFrom(s =>
                    s.CLO != null ? s.CLO.Code : null))
                .ForMember(d => d.PoCode, o => o.MapFrom(s =>
                    s.PO != null ? s.PO.Code : null));
            CreateMap<CreateCloPoMapDto, CloPoMap>();

            // ── Anket (Survey) ─────────────────────────────────────────────────────
            CreateMap<Survey, SurveyListDto>()
                .ForMember(d => d.QuestionCount, o => o.MapFrom(s =>
                    s.Questions != null ? s.Questions.Count : 0))
                .ForMember(d => d.SubmissionCount, o => o.MapFrom(s =>
                    s.Submissions != null ? s.Submissions.Count : 0));

            CreateMap<Survey, SurveyDetailDto>()
                .ForMember(d => d.SubmissionCount, o => o.MapFrom(s =>
                    s.Submissions != null ? s.Submissions.Count : 0))
                .ForMember(d => d.Questions, o => o.MapFrom(s => s.Questions));

            CreateMap<Question, SurveyQuestionDto>()
                .ForMember(d => d.CloCode, o => o.MapFrom(s =>
                    s.CourseLearningOutcome != null ? s.CourseLearningOutcome.Code : null))
                .ForMember(d => d.CloDescription, o => o.MapFrom(s =>
                    s.CourseLearningOutcome != null ? s.CourseLearningOutcome.Description : null));
        }
    }
}
