using BitirmeApi.Business.Abstract;
using BitirmeApi.Business.Concrete;
using BitirmeApi.Business.Helpers;
using BitirmeApi.DataAccess.Abstract;
using BitirmeApi.DataAccess.Concrete.EntityFramework;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BitirmeApi.Business.ServiceRegistration
{
    public static class BusinessStartup
    {
        public static void BusinessRegister(this IServiceCollection services)
        {
            services.AddAutoMapper(configAction => configAction.AddMaps(Assembly.GetExecutingAssembly()));

            // ── Kullanıcı ──────────────────────────────────────────────────────────
            services.AddScoped<IAppUserDal, AppUserDal>();
            services.AddScoped<IAppUserService, AppUserService>();

            // ── Auth & JWT ─────────────────────────────────────────────────────────
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IAuthService, AuthService>();

            // ── Program ────────────────────────────────────────────────────────────
            services.AddScoped<IProgramEntityDal, ProgramEntityDal>();
            services.AddScoped<IProgramEntityService, ProgramEntityService>();
            services.AddScoped<IProgramOutcomeDal, ProgramOutcomeDal>();
            services.AddScoped<IProgramOutcomeService, ProgramOutcomeService>();

            // ── Ders Kataloğu ──────────────────────────────────────────────────────
            services.AddScoped<ICourseDal, CourseDal>();
            services.AddScoped<ICourseService, CourseService>();

            // Katalog CLO (tek CLO kaynağı — CourseLearningOutcomeEntity kaldırıldı)
            services.AddScoped<ICourseLearningOutcomeDal, CourseLearningOutcomeDal>();
            services.AddScoped<ICourseLearningOutcomeService, CourseLearningOutcomeService>();

            services.AddScoped<ICloPoMapDal, CloPoMapDal>();
            services.AddScoped<ICloPoMapService, CloPoMapService>();

            // ── Akademik Dönem + Ders Açılışı + Kayıt ─────────────────────────────
            services.AddScoped<IAcademicTermDal, AcademicTermDal>();
            services.AddScoped<IAcademicTermService, AcademicTermService>();

            services.AddScoped<ICourseOfferingDal, CourseOfferingDal>();
            services.AddScoped<ICourseOfferingService, CourseOfferingService>();

            // Tek öğrenci kayıt modeli — StudentEnrollment kaldırıldı
            services.AddScoped<IEnrollmentDal, EnrollmentDal>();
            services.AddScoped<IEnrollmentService, EnrollmentService>();

            // ── MÜDEK Değerlendirme ────────────────────────────────────────────────
            // Yeni: CourseEvaluation → CourseOffering (1:1)
            services.AddScoped<ICourseEvaluationDal, CourseEvaluationDal>();
            services.AddScoped<ICourseEvaluationService, CourseEvaluationService>();

            services.AddScoped<IMudekEvaluationCalculatorService, MudekEvaluationCalculatorService>();

            services.AddScoped<IExamDal, ExamDal>();
            services.AddScoped<IExamService, ExamService>();

            services.AddScoped<IExamQuestionDal, ExamQuestionDal>();
            services.AddScoped<IExamQuestionService, ExamQuestionService>();

            services.AddScoped<IExamQuestionOutcomeMappingDal, ExamQuestionOutcomeMappingDal>();
            services.AddScoped<IExamQuestionOutcomeMappingService, ExamQuestionOutcomeMappingService>();

            services.AddScoped<IAssessmentComponentDal, AssessmentComponentDal>();
            services.AddScoped<IAssessmentComponentService, AssessmentComponentService>();

            services.AddScoped<IAssessmentComponentOutcomeMappingDal, AssessmentComponentOutcomeMappingDal>();
            services.AddScoped<IAssessmentComponentOutcomeMappingService, AssessmentComponentOutcomeMappingService>();

            // Öğrenci skor tabloları artık EnrollmentId kullanıyor
            services.AddScoped<IStudentAssessmentComponentScoreDal, StudentAssessmentComponentScoreDal>();
            services.AddScoped<IStudentAssessmentComponentScoreService, StudentAssessmentComponentScoreService>();

            services.AddScoped<IStudentAnswerDal, StudentAnswerDal>();
            services.AddScoped<IStudentAnswerService, StudentAnswerService>();

            services.AddScoped<ICourseEvaluationLetterGradeRuleDal, CourseEvaluationLetterGradeRuleDal>();
            services.AddScoped<ICourseEvaluationLetterGradeRuleService, CourseEvaluationLetterGradeRuleService>();

            services.AddScoped<IProgramLetterGradeRuleDal, ProgramLetterGradeRuleDal>();
            services.AddScoped<IProgramLetterGradeRuleService, ProgramLetterGradeRuleService>();

            // Katkı düzeyleri artık catalog CLO + CourseEvaluation FK üzerinden
            services.AddScoped<IProgramOutcomeContributionDal, ProgramOutcomeContributionDal>();
            services.AddScoped<IProgramOutcomeContributionService, ProgramOutcomeContributionService>();

            // ── Anket Sistemi ──────────────────────────────────────────────────────
            services.AddScoped<ISurveyDal, SurveyDal>();
            services.AddScoped<ISurveyService, SurveyService>();

            services.AddScoped<IQuestionDal, QuestionDal>();
            services.AddScoped<IQuestionService, QuestionService>();

            services.AddScoped<ISubmissionDal, SubmissionDal>();
            services.AddScoped<ISubmissionService, SubmissionService>();

            services.AddScoped<IAnswerDal, AnswerDal>();
            services.AddScoped<IAnswerService, AnswerService>();

            // Anket sonuç hesaplaması için yardımcı DAL'lar
            services.AddScoped<ICloEvaluationResultDal, CloEvaluationResultDal>();
            services.AddScoped<IStudentEvaluationResultDal, StudentEvaluationResultDal>();

            // ── Öğrenci Anket Servisi ──────────────────────────────────────────────
            services.AddScoped<IStudentSurveyService, StudentSurveyService>();
        }
    }
}
