using BitirmeApi.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitirmeApi.DataAccess.Concrete.EntityFramework.Context
{
    public class ProjectDbContext : DbContext
    {
        public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options) { }

        // ───── Kullanıcı ─────────────────────────────────────────────────────────
        public DbSet<AppUser> Users => Set<AppUser>();

        // ───── Program ───────────────────────────────────────────────────────────
        public DbSet<ProgramEntity> Programs => Set<ProgramEntity>();
        public DbSet<ProgramOutcome> ProgramOutcomes => Set<ProgramOutcome>();

        // ───── Akademik Takvim ────────────────────────────────────────────────────
        public DbSet<AcademicTerm> AcademicTerms => Set<AcademicTerm>();

        // ───── Ders Kataloğu ─────────────────────────────────────────────────────
        public DbSet<Course> Courses => Set<Course>();

        /// <summary>Katalog CLO — TEK CLO kaynağı, dönemsel değil</summary>
        public DbSet<CourseLearningOutcome> Clos => Set<CourseLearningOutcome>();
        public DbSet<CloPoMap> CloPoMaps => Set<CloPoMap>();

        // ───── Dönemlik Ders Açılışı ─────────────────────────────────────────────
        public DbSet<CourseOffering> CourseOfferings => Set<CourseOffering>();

        // ───── Öğrenci Kaydı (TEK model) ─────────────────────────────────────────
        public DbSet<Enrollment> Enrollments => Set<Enrollment>();

        // ───── Anket Sistemi ─────────────────────────────────────────────────────
        public DbSet<Survey> Surveys => Set<Survey>();
        public DbSet<Question> Questions => Set<Question>();
        public DbSet<Submission> Submissions => Set<Submission>();
        public DbSet<Answer> Answers => Set<Answer>();

        // ───── MÜDEK Ders Değerlendirme Sistemi ──────────────────────────────────
        public DbSet<CourseEvaluation> CourseEvaluations => Set<CourseEvaluation>();
        public DbSet<Exam> Exams => Set<Exam>();
        public DbSet<ExamQuestion> ExamQuestions => Set<ExamQuestion>();
        public DbSet<ProgramOutcomeContribution> ProgramOutcomeContributions => Set<ProgramOutcomeContribution>();

        // ───── Ölçme/Değerlendirme ───────────────────────────────────────────────
        public DbSet<ExamQuestionOutcomeMapping> ExamQuestionOutcomeMappings => Set<ExamQuestionOutcomeMapping>();
        public DbSet<AssessmentComponent> AssessmentComponents => Set<AssessmentComponent>();
        public DbSet<AssessmentComponentOutcomeMapping> AssessmentComponentOutcomeMappings => Set<AssessmentComponentOutcomeMapping>();

        // ───── Öğrenci Performans Verileri (Enrollment üzerinden) ─────────────────
        public DbSet<StudentAssessmentComponentScore> StudentAssessmentComponentScores => Set<StudentAssessmentComponentScore>();
        public DbSet<StudentAnswer> StudentAnswers => Set<StudentAnswer>();

        // ───── Harf Notu Kuralları ────────────────────────────────────────────────
        public DbSet<CourseEvaluationLetterGradeRule> CourseEvaluationLetterGradeRules => Set<CourseEvaluationLetterGradeRule>();

        // ───── MÜDEK önceden hesaplanmış sonuçlar (offering başına tek snapshot) ──
        public DbSet<StudentEvaluationResult> StudentEvaluationResults => Set<StudentEvaluationResult>();
        public DbSet<ExamEvaluationResult> ExamEvaluationResults => Set<ExamEvaluationResult>();
        public DbSet<ExamQuestionEvaluationResult> ExamQuestionEvaluationResults => Set<ExamQuestionEvaluationResult>();
        public DbSet<CloEvaluationResult> CloEvaluationResults => Set<CloEvaluationResult>();
        public DbSet<ProgramOutcomeEvaluationResult> ProgramOutcomeEvaluationResults => Set<ProgramOutcomeEvaluationResult>();
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer((@"Server=(localdb)\mssqllocaldb;database=MudekDbAcademic;Trusted_Connection=True;"));
        //}

        protected override void OnModelCreating(ModelBuilder b)
        {
            // ═══════════════════════════════════════════════════════════════════════
            // KULLANICI
            // ═══════════════════════════════════════════════════════════════════════
            b.Entity<AppUser>(e =>
            {
                e.Property(x => x.FullName).IsRequired().HasMaxLength(256);
                e.Property(x => x.Email).IsRequired().HasMaxLength(256);
                e.Property(x => x.PasswordHash).IsRequired().HasMaxLength(512);
                e.Property(x => x.Role).IsRequired().HasMaxLength(50);
                e.Property(x => x.StudentNumber).HasMaxLength(64);
                e.Property(x => x.Title).HasMaxLength(256);
                e.Property(x => x.PhoneNumber).HasMaxLength(20);
                e.Property(x => x.IsActive).HasDefaultValue(true);

                e.HasIndex(x => x.Email).IsUnique();

                e.HasOne(x => x.Program)
                 .WithMany()
                 .HasForeignKey(x => x.ProgramEntityId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            // ═══════════════════════════════════════════════════════════════════════
            // PROGRAM
            // ═══════════════════════════════════════════════════════════════════════
            b.Entity<ProgramEntity>(e =>
            {
                e.Property(x => x.Name).IsRequired().HasMaxLength(256);
                e.Property(x => x.AccreditationCycleYears).HasDefaultValue(5);
            });

            b.Entity<ProgramOutcome>(e =>
            {
                e.Property(x => x.Code).IsRequired().HasMaxLength(32);
                e.Property(x => x.Title).IsRequired().HasMaxLength(256);
                e.Property(x => x.Description).IsRequired().HasMaxLength(2000);

                e.HasIndex(x => new { x.ProgramEntityId, x.Code }).IsUnique();

                e.HasOne(x => x.Program)
                 .WithMany(p => p.ProgramOutcomes)
                 .HasForeignKey(x => x.ProgramEntityId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ═══════════════════════════════════════════════════════════════════════
            // AKADEMİK DÖNEM
            // ═══════════════════════════════════════════════════════════════════════
            b.Entity<AcademicTerm>(e =>
            {
                e.Property(x => x.TermType).IsRequired().HasMaxLength(16);
                e.Property(x => x.Name).IsRequired().HasMaxLength(128);
                e.Property(x => x.IsActive).HasDefaultValue(false);

                // Aynı yıl + tip kombinasyonu bir kez tanımlanabilir
                e.HasIndex(x => new { x.StartYear, x.EndYear, x.TermType }).IsUnique();
                // İsim de benzersiz
                e.HasIndex(x => x.Name).IsUnique();
            });

            // ═══════════════════════════════════════════════════════════════════════
            // DERS KATALOĞU
            // ═══════════════════════════════════════════════════════════════════════
            b.Entity<Course>(e =>
            {
                e.Property(x => x.Code).IsRequired().HasMaxLength(32);
                e.Property(x => x.Name).IsRequired().HasMaxLength(256);
                e.Property(x => x.Description).HasMaxLength(1000);
                e.Property(x => x.IsActive).HasDefaultValue(true);

                // Aynı programda ders kodu tekrar edemez
                e.HasIndex(x => new { x.ProgramEntityId, x.Code }).IsUnique();

                e.HasOne(x => x.Program)
                 .WithMany(p => p.Courses)
                 .HasForeignKey(x => x.ProgramEntityId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // Katalog CLO — tek CLO kaynağı
            b.Entity<CourseLearningOutcome>(e =>
            {
                e.Property(x => x.Code).IsRequired().HasMaxLength(32);
                e.Property(x => x.Description).IsRequired().HasMaxLength(2000);

                e.HasIndex(x => new { x.CourseId, x.Code }).IsUnique();

                e.HasOne(x => x.Course)
                 .WithMany(c => c.Clos)
                 .HasForeignKey(x => x.CourseId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            b.Entity<CloPoMap>(e =>
            {
                e.Property(x => x.Weight).HasPrecision(5, 2);

                e.HasIndex(x => new { x.CourseLearningOutcomeId, x.ProgramOutcomeId }).IsUnique();

                e.HasOne(x => x.CLO)
                 .WithMany(c => c.Maps)
                 .HasForeignKey(x => x.CourseLearningOutcomeId)
                 .OnDelete(DeleteBehavior.NoAction);

                e.HasOne(x => x.PO)
                 .WithMany()
                 .HasForeignKey(x => x.ProgramOutcomeId)
                 .OnDelete(DeleteBehavior.NoAction);
            });

            // ═══════════════════════════════════════════════════════════════════════
            // DÖNEMLİK DERS AÇILIŞI
            // ═══════════════════════════════════════════════════════════════════════
            b.Entity<CourseOffering>(e =>
            {
                e.Property(x => x.Section).IsRequired().HasMaxLength(16).HasDefaultValue("A");
                e.Property(x => x.PassingGrade).HasPrecision(5, 2);
                e.Property(x => x.IsActive).HasDefaultValue(true);

                e.HasOne(x => x.Course)
                 .WithMany(c => c.CourseOfferings)
                 .HasForeignKey(x => x.CourseId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.AcademicTerm)
                 .WithMany(t => t.CourseOfferings)
                 .HasForeignKey(x => x.AcademicTermId)
                 .OnDelete(DeleteBehavior.Restrict); // dönem açılışı varken silinemez

                e.HasOne(x => x.Teacher)
                 .WithMany()
                 .HasForeignKey(x => x.TeacherId)
                 .OnDelete(DeleteBehavior.SetNull);

                // Unique: aynı ders + dönem + şube bir kez açılabilir
                e.HasIndex(x => new { x.CourseId, x.AcademicTermId, x.Section }).IsUnique();
            });

            // ═══════════════════════════════════════════════════════════════════════
            // ÖĞRENCİ KAYDI — TEK MODEL
            // ═══════════════════════════════════════════════════════════════════════
            b.Entity<Enrollment>(e =>
            {
                e.Property(x => x.Status).IsRequired().HasMaxLength(32).HasDefaultValue(EnrollmentStatus.Enrolled);

                e.HasOne(x => x.CourseOffering)
                 .WithMany(o => o.Enrollments)
                 .HasForeignKey(x => x.CourseOfferingId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Student)
                 .WithMany()
                 .HasForeignKey(x => x.StudentId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new { x.CourseOfferingId, x.StudentId }).IsUnique();
            });

            // ═══════════════════════════════════════════════════════════════════════
            // ANKET SİSTEMİ
            // ═══════════════════════════════════════════════════════════════════════
            b.Entity<Survey>(e =>
            {
                e.Property(x => x.Title).IsRequired().HasMaxLength(256);
                e.Property(x => x.Description).HasMaxLength(2000);
                e.Property(x => x.IsActive).HasDefaultValue(true);

                e.HasOne(x => x.CourseOffering)
                 .WithMany(c => c.Surveys)
                 .HasForeignKey(x => x.CourseOfferingId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            b.Entity<Question>(e =>
            {
                e.Property(x => x.Text).IsRequired().HasMaxLength(1000);
                e.Property(x => x.Type).HasConversion<string>().HasMaxLength(16);
                e.Property(x => x.McqOptionsCsv).HasMaxLength(2000);

                e.HasOne(x => x.Survey)
                 .WithMany(s => s.Questions)
                 .HasForeignKey(x => x.SurveyId)
                 .OnDelete(DeleteBehavior.Cascade);

                // DÖÇ eşlemesi isteğe bağlı.
                // SetNull yerine Restrict: SQL Server'da Survey→Questions CASCADE ile çakışan
                // ikinci bir cascade path oluşmaması için. CLO silinmek istenirse önce
                // bağlı anket soruları servis katmanında temizlenmelidir.
                e.HasOne(x => x.CourseLearningOutcome)
                 .WithMany(c => c.SurveyQuestions)
                 .HasForeignKey(x => x.CourseLearningOutcomeId)
                 .OnDelete(DeleteBehavior.Restrict)
                 .IsRequired(false);
            });

            b.Entity<Submission>(e =>
            {
                e.HasOne(x => x.Survey)
                 .WithMany(s => s.Submissions)
                 .HasForeignKey(x => x.SurveyId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.User)
                 .WithMany()
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new { x.SurveyId, x.UserId }).IsUnique();
            });

            b.Entity<Answer>(e =>
            {
                e.Property(x => x.ValueNumeric).HasPrecision(5, 2);

                e.HasOne(x => x.Submission)
                 .WithMany(s => s.Answers)
                 .HasForeignKey(x => x.SubmissionId)
                 .OnDelete(DeleteBehavior.NoAction);

                e.HasOne(x => x.Question)
                 .WithMany(q => q.Answers)
                 .HasForeignKey(x => x.QuestionId)
                 .OnDelete(DeleteBehavior.NoAction);

                e.HasIndex(x => new { x.QuestionId, x.SubmissionId }).IsUnique();
            });

            // ═══════════════════════════════════════════════════════════════════════
            // MÜDEK DERS DEĞERLENDİRME
            // ═══════════════════════════════════════════════════════════════════════

            b.Entity<CourseEvaluation>(e =>
            {
                // 1 CourseOffering = 1 CourseEvaluation (unique)
                e.HasOne(x => x.CourseOffering)
                 .WithOne(o => o.CourseEvaluation)
                 .HasForeignKey<CourseEvaluation>(x => x.CourseOfferingId)
                 .OnDelete(DeleteBehavior.Restrict); // offering silinirken evaluation korunsun

                e.HasIndex(x => x.CourseOfferingId).IsUnique();
            });

            b.Entity<Exam>(e =>
            {
                e.Property(x => x.ExamType).IsRequired().HasMaxLength(128);

                e.HasOne(x => x.CourseEvaluation)
                 .WithMany(c => c.Exams)
                 .HasForeignKey(x => x.CourseEvaluationId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            b.Entity<ExamQuestion>(e =>
            {
                e.Property(x => x.Title).HasMaxLength(500);
                e.Property(x => x.QuestionType).HasMaxLength(50);
                e.Property(x => x.MaxScore).HasPrecision(7, 2).IsRequired();

                e.HasOne(x => x.Exam)
                 .WithMany(ex => ex.Questions)
                 .HasForeignKey(x => x.ExamId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasIndex(x => new { x.ExamId, x.QuestionNumber }).IsUnique();
            });

            // ═══════════════════════════════════════════════════════════════════════
            // PROGRAM ÇIKTI KATKISI (Katalog CLO üzerinden, per evaluation)
            // ═══════════════════════════════════════════════════════════════════════
            b.Entity<ProgramOutcomeContribution>(e =>
            {
                e.Property(x => x.ProgramOutcomeCode).IsRequired().HasMaxLength(64);

                e.HasOne(x => x.CourseEvaluation)
                 .WithMany(ce => ce.ProgramOutcomeContributions)
                 .HasForeignKey(x => x.CourseEvaluationId)
                 .OnDelete(DeleteBehavior.Cascade);

                // Katalog CLO — tek CLO kaynağı
                e.HasOne(x => x.CourseLearningOutcome)
                 .WithMany()
                 .HasForeignKey(x => x.CourseLearningOutcomeId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new { x.CourseEvaluationId, x.CourseLearningOutcomeId, x.ProgramOutcomeCode }).IsUnique();
            });

            // ═══════════════════════════════════════════════════════════════════════
            // ÖLÇME EŞLEMELERİ (Katalog CLO üzerinden)
            // ═══════════════════════════════════════════════════════════════════════
            b.Entity<ExamQuestionOutcomeMapping>(e =>
            {
                e.Property(x => x.Weight).HasPrecision(5, 2).IsRequired();

                e.HasOne(x => x.ExamQuestion)
                 .WithMany(q => q.OutcomeMappings)
                 .HasForeignKey(x => x.ExamQuestionId)
                 .OnDelete(DeleteBehavior.Cascade);

                // Katalog CLO — tek CLO kaynağı
                e.HasOne(x => x.CourseLearningOutcome)
                 .WithMany(c => c.ExamQuestionMappings)
                 .HasForeignKey(x => x.CourseLearningOutcomeId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new { x.ExamQuestionId, x.CourseLearningOutcomeId }).IsUnique();
                e.HasCheckConstraint("CK_ExamQuestionOutcomeMapping_Weight", "[Weight] >= 0 AND [Weight] <= 1");
            });

            b.Entity<AssessmentComponent>(e =>
            {
                e.Property(x => x.Name).IsRequired().HasMaxLength(200);
                e.Property(x => x.ComponentType).IsRequired().HasMaxLength(50);
                e.Property(x => x.MaxScore).HasPrecision(7, 2).IsRequired();
                e.Property(x => x.WeightPercentage).HasPrecision(5, 2);
                e.Property(x => x.Description).HasMaxLength(1000);
                e.Property(x => x.IsActive).HasDefaultValue(true);

                e.HasOne(x => x.Exam)
                 .WithMany()
                 .HasForeignKey(x => x.ExamId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasIndex(x => new { x.ExamId, x.OrderIndex }).IsUnique();
                e.HasCheckConstraint("CK_AssessmentComponent_WeightPercentage",
                    "[WeightPercentage] IS NULL OR ([WeightPercentage] >= 0 AND [WeightPercentage] <= 100)");
            });

            b.Entity<AssessmentComponentOutcomeMapping>(e =>
            {
                e.Property(x => x.Weight).HasPrecision(5, 2).IsRequired();

                e.HasOne(x => x.AssessmentComponent)
                 .WithMany(ac => ac.OutcomeMappings)
                 .HasForeignKey(x => x.AssessmentComponentId)
                 .OnDelete(DeleteBehavior.Cascade);

                // Katalog CLO — tek CLO kaynağı
                e.HasOne(x => x.CourseLearningOutcome)
                 .WithMany(c => c.AssessmentComponentMappings)
                 .HasForeignKey(x => x.CourseLearningOutcomeId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new { x.AssessmentComponentId, x.CourseLearningOutcomeId }).IsUnique();
                e.HasCheckConstraint("CK_AssessmentComponentOutcomeMapping_Weight", "[Weight] >= 0 AND [Weight] <= 1");
            });

            // ═══════════════════════════════════════════════════════════════════════
            // ÖĞRENCİ PERFORMANS VERİLERİ (Enrollment üzerinden — tek model)
            // ═══════════════════════════════════════════════════════════════════════
            b.Entity<StudentAssessmentComponentScore>(e =>
            {
                e.Property(x => x.Score).HasPrecision(7, 2);
                e.Property(x => x.Notes).HasMaxLength(500);

                e.HasOne(x => x.AssessmentComponent)
                 .WithMany(ac => ac.StudentScores)
                 .HasForeignKey(x => x.AssessmentComponentId)
                 .OnDelete(DeleteBehavior.Cascade);

                // Enrollment — tek öğrenci kayıt modeli
                e.HasOne(x => x.Enrollment)
                 .WithMany(en => en.ComponentScores)
                 .HasForeignKey(x => x.EnrollmentId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new { x.AssessmentComponentId, x.EnrollmentId }).IsUnique();
            });

            b.Entity<StudentAnswer>(e =>
            {
                e.Property(x => x.Score).HasPrecision(7, 2).IsRequired();

                e.HasOne(x => x.ExamQuestion)
                 .WithMany(q => q.StudentAnswers)
                 .HasForeignKey(x => x.ExamQuestionId)
                 .OnDelete(DeleteBehavior.Cascade);

                // Enrollment — tek öğrenci kayıt modeli
                e.HasOne(x => x.Enrollment)
                 .WithMany(en => en.StudentAnswers)
                 .HasForeignKey(x => x.EnrollmentId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new { x.ExamQuestionId, x.EnrollmentId }).IsUnique();
            });

            // ═══════════════════════════════════════════════════════════════════════
            // HARF NOTU KURALLARI
            // ═══════════════════════════════════════════════════════════════════════
            b.Entity<CourseEvaluationLetterGradeRule>(e =>
            {
                e.Property(x => x.LetterGrade).IsRequired().HasMaxLength(5);
                e.Property(x => x.MinScore).HasPrecision(5, 2).IsRequired();
                e.Property(x => x.MaxScore).HasPrecision(5, 2).IsRequired();
                e.Property(x => x.MinimumFinalScore).HasPrecision(5, 2);
                e.Property(x => x.Description).HasMaxLength(500);

                e.HasOne(x => x.CourseEvaluation)
                 .WithMany(ce => ce.LetterGradeRules)
                 .HasForeignKey(x => x.CourseEvaluationId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasIndex(x => new { x.CourseEvaluationId, x.LetterGrade }).IsUnique();
                e.HasCheckConstraint("CK_LetterGradeRule_ScoreRange", "[MaxScore] >= [MinScore]");
                e.HasCheckConstraint("CK_LetterGradeRule_ScoreBounds", "[MinScore] >= 0 AND [MaxScore] <= 100");
            });

            // ═══════════════════════════════════════════════════════════════════════
            // MÜDEK HESAP SONUÇLARI (snapshot — versioning yok)
            // ═══════════════════════════════════════════════════════════════════════

            b.Entity<StudentEvaluationResult>(e =>
            {
                e.Property(x => x.MidtermScore).HasPrecision(7, 2);
                e.Property(x => x.FinalScore).HasPrecision(7, 2);
                e.Property(x => x.MakeupScore).HasPrecision(7, 2);
                e.Property(x => x.SuccessGrade).HasPrecision(7, 2);

                e.HasIndex(x => new { x.CourseOfferingId, x.EnrollmentId }).IsUnique();

                // CourseOffering üzerinden CASCADE + Enrollment üzerinden CASCADE aynı tabloda
                // SQL Server'da "multiple cascade paths" hatasına yol açar. Tek zincir: Offering → Enrollment → bu satır.
                e.HasOne(x => x.CourseOffering)
                 .WithMany(o => o.StudentEvaluationResults)
                 .HasForeignKey(x => x.CourseOfferingId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Enrollment)
                 .WithMany(en => en.EvaluationResults)
                 .HasForeignKey(x => x.EnrollmentId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            b.Entity<ExamEvaluationResult>(e =>
            {
                e.Property(x => x.MaxTotalScore).HasPrecision(9, 4);
                e.Property(x => x.MinTotalScore).HasPrecision(9, 4);
                e.Property(x => x.AverageTotalScore).HasPrecision(9, 4);

                e.HasIndex(x => new { x.CourseOfferingId, x.ExamId }).IsUnique();

                e.HasOne(x => x.CourseOffering)
                 .WithMany(o => o.ExamEvaluationResults)
                 .HasForeignKey(x => x.CourseOfferingId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Exam)
                 .WithMany()
                 .HasForeignKey(x => x.ExamId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            b.Entity<ExamQuestionEvaluationResult>(e =>
            {
                e.Property(x => x.MaxScore).HasPrecision(7, 2);
                e.Property(x => x.AverageScore).HasPrecision(9, 4);
                e.Property(x => x.AchievementRate).HasPrecision(9, 6);

                e.HasIndex(x => new { x.CourseOfferingId, x.ExamQuestionId })
                 .IsUnique()
                 .HasFilter("[ExamQuestionId] IS NOT NULL");

                e.HasIndex(x => new { x.CourseOfferingId, x.AssessmentComponentId })
                 .IsUnique()
                 .HasFilter("[AssessmentComponentId] IS NOT NULL");

                e.HasCheckConstraint("CK_ExamQuestionEvalResult_SingleTarget",
                    "([ExamQuestionId] IS NOT NULL AND [AssessmentComponentId] IS NULL) OR ([ExamQuestionId] IS NULL AND [AssessmentComponentId] IS NOT NULL)");

                e.HasOne(x => x.CourseOffering)
                 .WithMany(o => o.ExamQuestionEvaluationResults)
                 .HasForeignKey(x => x.CourseOfferingId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Exam)
                 .WithMany()
                 .HasForeignKey(x => x.ExamId)
                 .OnDelete(DeleteBehavior.Cascade);

                // ExamId ile CASCADE zaten sınav silinince satırı temizler; ExamQuestion / Component
                // üzerinden ikinci CASCADE yolu SQL Server'da "multiple cascade paths" üretir.
                e.HasOne(x => x.ExamQuestion)
                 .WithMany()
                 .HasForeignKey(x => x.ExamQuestionId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.AssessmentComponent)
                 .WithMany()
                 .HasForeignKey(x => x.AssessmentComponentId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            b.Entity<CloEvaluationResult>(e =>
            {
                e.Property(x => x.AchievementScore).HasPrecision(9, 6);
                e.Property(x => x.CombinedAchievementScore).HasPrecision(9, 6);
                e.Property(x => x.SurveyScore).HasPrecision(9, 6);
                e.Property(x => x.SurveyDifference).HasPrecision(9, 6);

                e.HasIndex(x => new { x.CourseOfferingId, x.CourseLearningOutcomeId, x.ResultType }).IsUnique();

                e.HasOne(x => x.CourseOffering)
                 .WithMany(o => o.CloEvaluationResults)
                 .HasForeignKey(x => x.CourseOfferingId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.CourseLearningOutcome)
                 .WithMany()
                 .HasForeignKey(x => x.CourseLearningOutcomeId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Exam)
                 .WithMany()
                 .HasForeignKey(x => x.ExamId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            b.Entity<ProgramOutcomeEvaluationResult>(e =>
            {
                e.Property(x => x.AchievementScore).HasPrecision(9, 6);

                e.HasIndex(x => new { x.CourseOfferingId, x.ProgramOutcomeId }).IsUnique();

                e.HasOne(x => x.CourseOffering)
                 .WithMany(o => o.ProgramOutcomeEvaluationResults)
                 .HasForeignKey(x => x.CourseOfferingId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.ProgramOutcome)
                 .WithMany()
                 .HasForeignKey(x => x.ProgramOutcomeId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
