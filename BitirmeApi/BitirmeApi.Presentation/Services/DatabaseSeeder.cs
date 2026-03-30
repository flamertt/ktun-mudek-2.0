using BitirmeApi.DataAccess.Concrete.EntityFramework.Context;
using BitirmeApi.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitirmeApi.Presentation.Services
{
    public class DatabaseSeeder
    {
        private const string SharedPasswordHash = "A6xnQhbz4Vx2HuGl4lXwZ5U2I8iziLRFnhP5eNfIRvQ=";

        public static async Task SeedTestUsersAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ProjectDbContext>();

            try
            {
                // Veritabanı şeması migration ile senin tarafından uygulanır; burada EnsureCreated/Migrate yok.

                var now = DateTime.UtcNow;
                var rng = new Random(20260330);

                // 1) Program
                var program = await context.Programs.FirstOrDefaultAsync(p => p.Name == "Bilgisayar Mühendisliği");
                if (program == null)
                {
                    program = new ProgramEntity
                    {
                        Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                        Name = "Bilgisayar Mühendisliği",
                        AccreditationCycleYears = 5
                    };
                    context.Programs.Add(program);
                    await context.SaveChangesAsync();
                }

                // 2) Admin + Öğretmen + 40 öğrenci
                var admin = await EnsureUserAsync(
                    context,
                    "admin@ktun.edu.tr",
                    "Sistem Yöneticisi",
                    "Admin",
                    program.Id,
                    studentNumber: null,
                    title: "Yönetici",
                    phone: "+90 332 000 0001");

                var teacher = await EnsureUserAsync(
                    context,
                    "mesut.gunduz@ktun.edu.tr",
                    "Mesut Gündüz",
                    "Teacher",
                    program.Id,
                    studentNumber: null,
                    title: "Doç. Dr.",
                    phone: "+90 332 000 0002",
                    lookupEmails: new[] { "selim.karahan@ktun.edu.tr" });

                var studentNames = new List<(string FullName, string StudentNo)>
                {
                    ("Ahmet Yılmaz", "20190001"), ("Mehmet Demir", "20190002"), ("Ayşe Kaya", "20190003"), ("Fatma Şahin", "20190004"),
                    ("Mustafa Çelik", "20190005"), ("Emine Arslan", "20190006"), ("Ali Koç", "20190007"), ("Zeynep Aydın", "20190008"),
                    ("Murat Yıldırım", "20190009"), ("Elif Kılıç", "20190010"), ("Burak Kurt", "20190011"), ("Ceren Özkan", "20190012"),
                    ("Onur Güneş", "20190013"), ("Ece Yavuz", "20190014"), ("Hakan Polat", "20190015"), ("Derya Aksoy", "20190016"),
                    ("Serkan Kaplan", "20190017"), ("Buse Erdem", "20190018"), ("Can Korkmaz", "20190019"), ("Melis Tunç", "20190020"),
                    ("Okan Uçar", "20190021"), ("Seda Çetin", "20190022"), ("Taha Şimşek", "20190023"), ("Aslı Özdemir", "20190024"),
                    ("Eren Topal", "20190025"), ("Nazlı Acar", "20190026"), ("Kaan Ateş", "20190027"), ("İrem Şen", "20190028"),
                    ("Yusuf Bozkurt", "20190029"), ("Damla Öz", "20190030"), ("Barış Sezer", "20190031"), ("Tuğba Candan", "20190032"),
                    ("Umut Duman", "20190033"), ("Merve Erkan", "20190034"), ("Sinan Baş", "20190035"), ("Pelin Yaman", "20190036"),
                    ("Emir Şimşir", "20190037"), ("Nisa Çoban", "20190038"), ("Furkan Avcı", "20190039"), ("Gizem Ünver", "20190040")
                };

                var students = new List<AppUser>();
                foreach (var s in studentNames)
                {
                    var email = $"{s.StudentNo}@ogr.ktun.edu.tr";
                    var st = await EnsureUserAsync(
                        context,
                        email,
                        s.FullName,
                        "Student",
                        program.Id,
                        studentNumber: s.StudentNo,
                        title: null,
                        phone: null);
                    students.Add(st);
                }

                // 3) Program çıktıları (PO)
                var poDefinitions = new List<(string Code, string Title, string Desc)>
                {
                    ("PÇ1", "Matematik ve mühendislik temelleri", "Matematik, temel bilim ve mühendislik bilgisini uygular."),
                    ("PÇ2", "Analiz ve modelleme", "Karmaşık mühendislik problemlerini analiz eder ve modeller."),
                    ("PÇ3", "Sistem tasarımı", "İhtiyaçlara uygun sistem, bileşen ve süreç tasarlar."),
                    ("PÇ4", "Deney ve veri analizi", "Deney tasarlar, veri toplar, sonuçları yorumlar."),
                    ("PÇ5", "Modern araç kullanımı", "Mühendislik uygulamalarında modern araçları etkin kullanır."),
                    ("PÇ6", "Mesleki etik ve takım çalışması", "Etik sorumluluk bilinci ve takım çalışması yetkinliği gösterir.")
                };

                foreach (var po in poDefinitions)
                {
                    if (!await context.ProgramOutcomes.AnyAsync(x => x.ProgramEntityId == program.Id && x.Code == po.Code))
                    {
                        context.ProgramOutcomes.Add(new ProgramOutcome
                        {
                            Id = Guid.NewGuid(),
                            ProgramEntityId = program.Id,
                            Code = po.Code,
                            Title = po.Title,
                            Description = po.Desc
                        });
                    }
                }
                await context.SaveChangesAsync();

                // 4) Ders
                var course = await context.Courses.FirstOrDefaultAsync(c => c.ProgramEntityId == program.Id && c.Code == "BLM401");
                if (course == null)
                {
                    course = new Course
                    {
                        Id = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                        ProgramEntityId = program.Id,
                        Code = "BLM401",
                        Name = "Yazılım Mühendisliği",
                        Credits = 6,
                        Description = "Yazılım yaşam döngüsü, gereksinim, tasarım, test ve kalite konularını kapsar.",
                        IsActive = true,
                        CreatedAt = now
                    };
                    context.Courses.Add(course);
                    await context.SaveChangesAsync();
                }

                // 5) CLO (DÖÇ)
                var cloDefinitions = new List<(string Code, string Desc, int Order)>
                {
                    ("DÖÇ1", "Yazılım gereksinimlerini analiz eder.", 1),
                    ("DÖÇ2", "Nesne yönelimli tasarım ilkelerini uygular.", 2),
                    ("DÖÇ3", "Test senaryoları hazırlar ve doğrulama yapar.", 3),
                    ("DÖÇ4", "Sürüm kontrolü ve takım süreçlerini uygular.", 4),
                    ("DÖÇ5", "Yazılım kalitesi metriklerini yorumlar.", 5),
                    ("DÖÇ6", "Mimari alternatifleri teknik olarak karşılaştırır.", 6)
                };

                foreach (var clo in cloDefinitions)
                {
                    if (!await context.Clos.AnyAsync(x => x.CourseId == course.Id && x.Code == clo.Code))
                    {
                        context.Clos.Add(new CourseLearningOutcome
                        {
                            Id = Guid.NewGuid(),
                            CourseId = course.Id,
                            Code = clo.Code,
                            Description = clo.Desc,
                            OrderIndex = clo.Order
                        });
                    }
                }
                await context.SaveChangesAsync();

                var clos = await context.Clos.Where(c => c.CourseId == course.Id).OrderBy(c => c.OrderIndex).ToListAsync();
                var pos = await context.ProgramOutcomes.Where(p => p.ProgramEntityId == program.Id).OrderBy(p => p.Code).ToListAsync();

                // 6) CLO -> PO map
                var cloPoWeights = new Dictionary<(int clo, int po), decimal>
                {
                    [(1,1)] = 0.40m, [(1,2)] = 0.60m,
                    [(2,2)] = 0.50m, [(2,3)] = 0.50m,
                    [(3,3)] = 0.40m, [(3,4)] = 0.60m,
                    [(4,5)] = 1.00m,
                    [(5,5)] = 0.60m, [(5,6)] = 0.40m,
                    [(6,1)] = 0.30m, [(6,3)] = 0.70m
                };

                foreach (var item in cloPoWeights)
                {
                    var clo = clos[item.Key.clo - 1];
                    var po = pos[item.Key.po - 1];
                    if (!await context.CloPoMaps.AnyAsync(m => m.CourseLearningOutcomeId == clo.Id && m.ProgramOutcomeId == po.Id))
                    {
                        context.CloPoMaps.Add(new CloPoMap
                        {
                            Id = Guid.NewGuid(),
                            CourseLearningOutcomeId = clo.Id,
                            ProgramOutcomeId = po.Id,
                            Weight = item.Value
                        });
                    }
                }
                await context.SaveChangesAsync();

                // 7) Dönem
                var term = await context.AcademicTerms.FirstOrDefaultAsync(t => t.Name == "2025-2026 Güz");
                if (term == null)
                {
                    term = new AcademicTerm
                    {
                        Id = Guid.Parse("30000000-0000-0000-0000-000000000001"),
                        StartYear = 2025,
                        EndYear = 2026,
                        TermType = TermType.Guz,
                        Name = "2025-2026 Güz",
                        IsActive = true,
                        CreatedAt = now
                    };
                    context.AcademicTerms.Add(term);
                    await context.SaveChangesAsync();
                }

                // 8) Offering + öğretmen atama
                var offering = await context.CourseOfferings
                    .FirstOrDefaultAsync(o => o.CourseId == course.Id && o.AcademicTermId == term.Id && o.Section == "A");
                if (offering == null)
                {
                    offering = new CourseOffering
                    {
                        Id = Guid.Parse("40000000-0000-0000-0000-000000000001"),
                        CourseId = course.Id,
                        AcademicTermId = term.Id,
                        TeacherId = teacher.Id,
                        Section = "A",
                        PassingGrade = 50,
                        Quota = 60,
                        IsActive = true,
                        CreatedAt = now
                    };
                    context.CourseOfferings.Add(offering);
                    await context.SaveChangesAsync();
                }
                else if (offering.TeacherId != teacher.Id)
                {
                    offering.TeacherId = teacher.Id;
                    offering.UpdatedAt = now;
                    await context.SaveChangesAsync();
                }

                // 9) Enrollment (40 öğrenci)
                foreach (var st in students)
                {
                    if (!await context.Enrollments.AnyAsync(e => e.CourseOfferingId == offering.Id && e.StudentId == st.Id))
                    {
                        context.Enrollments.Add(new Enrollment
                        {
                            Id = Guid.NewGuid(),
                            CourseOfferingId = offering.Id,
                            StudentId = st.Id,
                            Status = EnrollmentStatus.Enrolled,
                            EnrolledAt = now
                        });
                    }
                }
                await context.SaveChangesAsync();

                var enrollments = await context.Enrollments
                    .Where(e => e.CourseOfferingId == offering.Id)
                    .OrderBy(e => e.Student!.StudentNumber)
                    .Include(e => e.Student)
                    .ToListAsync();

                // 10) CourseEvaluation
                var evaluation = await context.CourseEvaluations.FirstOrDefaultAsync(e => e.CourseOfferingId == offering.Id);
                if (evaluation == null)
                {
                    evaluation = new CourseEvaluation
                    {
                        Id = Guid.Parse("50000000-0000-0000-0000-000000000001"),
                        CourseOfferingId = offering.Id,
                        CreatedDate = now,
                        IsCalculationDirty = true,
                        StudentFeedbackEvaluation = "Seed veri: anket kısmı bilinçli olarak boş bırakıldı.",
                        ProgramOutcomeEvaluation = "Seed veri: ders PÇ katkıları örnek ağırlıklarla tanımlandı.",
                        GeneralEvaluation = "Seed veri: MÜDEK hesap zincirinin test edilmesi için oluşturuldu.",
                        ImprovementSuggestions = "Seed veri: örnek iyileştirme alanları."
                    };
                    context.CourseEvaluations.Add(evaluation);
                    await context.SaveChangesAsync();
                }

                // 11) Sınavlar
                var midterm = await EnsureExamAsync(context, evaluation.Id, "Vize", 40, 1);
                var final = await EnsureExamAsync(context, evaluation.Id, "Final", 60, 2);
                var makeup = await EnsureExamAsync(context, evaluation.Id, "Bütünleme", 60, 3);

                // Vize quiz: MaxScore 100, sınav içi ağırlık %20; yazılı 10×10=100 üzerinden %80 ile birleşir.
                var quiz = await context.AssessmentComponents
                    .FirstOrDefaultAsync(c => c.ExamId == midterm.Id && c.OrderIndex == 100);
                if (quiz == null)
                {
                    quiz = new AssessmentComponent
                    {
                        Id = Guid.NewGuid(),
                        ExamId = midterm.Id,
                        Name = "Vize Quiz",
                        ComponentType = "Quiz",
                        MaxScore = 100,
                        WeightPercentage = 20,
                        OrderIndex = 100,
                        Description = "Vize haftasında yapılan kısa sınav (100 üzerinden; vize toplamında %20)",
                        IsActive = true,
                        CreatedAt = now
                    };
                    context.AssessmentComponents.Add(quiz);
                    await context.SaveChangesAsync();
                }
                else
                {
                    quiz.MaxScore = 100;
                    quiz.WeightPercentage = 20;
                    await context.SaveChangesAsync();
                }

                // 12) Sorular — yazılı toplam 100 (10 soru × 10 puan)
                const int writtenQuestionCount = 10;
                var midtermQuestions = await EnsureQuestionsAsync(context, midterm.Id, writtenQuestionCount, "Vize");
                var finalQuestions = await EnsureQuestionsAsync(context, final.Id, writtenQuestionCount, "Final");
                var makeupQuestions = await EnsureQuestionsAsync(context, makeup.Id, writtenQuestionCount, "Bütünleme");

                // 13) Mapping (soru->CLO ve component->CLO)
                await EnsureQuestionMappingsAsync(context, midtermQuestions, clos);
                await EnsureQuestionMappingsAsync(context, finalQuestions, clos);
                await EnsureQuestionMappingsAsync(context, makeupQuestions, clos);

                if (!await context.AssessmentComponentOutcomeMappings.AnyAsync(x => x.AssessmentComponentId == quiz.Id))
                {
                    context.AssessmentComponentOutcomeMappings.AddRange(
                        new AssessmentComponentOutcomeMapping
                        {
                            Id = Guid.NewGuid(),
                            AssessmentComponentId = quiz.Id,
                            CourseLearningOutcomeId = clos[1].Id,
                            Weight = 0.50m,
                            CreatedAt = now
                        },
                        new AssessmentComponentOutcomeMapping
                        {
                            Id = Guid.NewGuid(),
                            AssessmentComponentId = quiz.Id,
                            CourseLearningOutcomeId = clos[2].Id,
                            Weight = 0.50m,
                            CreatedAt = now
                        });
                    await context.SaveChangesAsync();
                }

                // 14) Harf notu kuralları
                if (!await context.CourseEvaluationLetterGradeRules.AnyAsync(r => r.CourseEvaluationId == evaluation.Id))
                {
                    context.CourseEvaluationLetterGradeRules.AddRange(
                        GradeRule(evaluation.Id, "AA", 90, 100, true, 50),
                        GradeRule(evaluation.Id, "BA", 85, 89.99m, true, 50),
                        GradeRule(evaluation.Id, "BB", 80, 84.99m, true, 50),
                        GradeRule(evaluation.Id, "CB", 70, 79.99m, true, 45),
                        GradeRule(evaluation.Id, "CC", 60, 69.99m, true, 45),
                        GradeRule(evaluation.Id, "DC", 50, 59.99m, true, 40),
                        GradeRule(evaluation.Id, "DD", 40, 49.99m, false, 40),
                        GradeRule(evaluation.Id, "FF", 0, 39.99m, false, null));
                    await context.SaveChangesAsync();
                }

                // 15) Öğrenci puanları (soru notları + quiz score)
                // Yazılı ham toplamlar (her sınav 100 üzerinden soru toplamı)
                await EnsureStudentAnswersAsync(context, enrollments, midtermQuestions, 35, 95, rng);
                await EnsureStudentAnswersAsync(context, enrollments, finalQuestions, 30, 98, rng);
                await EnsureStudentAnswersAsync(context, enrollments, makeupQuestions, 25, 95, rng, makeupOnlyForBottomHalf: true);
                await EnsureQuizScoresAsync(context, enrollments, quiz, rng);

                Console.WriteLine("✓ Seed tamamlandı: Program, kullanıcılar, ders, DÖÇ/PÇ, offering, sınavlar, sorular ve öğrenci notları eklendi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Seed data eklenirken hata oluştu: {ex.Message}");
                throw;
            }
        }

        private static async Task<AppUser> EnsureUserAsync(
            ProjectDbContext context,
            string email,
            string fullName,
            string role,
            Guid? programId,
            string? studentNumber,
            string? title,
            string? phone,
            IEnumerable<string>? lookupEmails = null)
        {
            var emailsToLookup = new List<string> { email };
            if (lookupEmails != null)
                emailsToLookup.AddRange(lookupEmails);

            var normalizedEmails = emailsToLookup
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .Select(e => e.Trim().ToLower())
                .Distinct()
                .ToList();

            var user = await context.Users.FirstOrDefaultAsync(u => normalizedEmails.Contains(u.Email.ToLower()));
            if (user != null)
            {
                var hasChanges =
                    user.Email != email ||
                    user.FullName != fullName ||
                    user.Role != role ||
                    user.ProgramEntityId != programId ||
                    user.StudentNumber != studentNumber ||
                    user.Title != title ||
                    user.PhoneNumber != phone ||
                    user.IsActive == false;

                if (hasChanges)
                {
                    user.Email = email;
                    user.FullName = fullName;
                    user.Role = role;
                    user.ProgramEntityId = programId;
                    user.StudentNumber = studentNumber;
                    user.Title = title;
                    user.PhoneNumber = phone;
                    user.IsActive = true;
                    user.UpdatedAt = DateTime.UtcNow;
                    await context.SaveChangesAsync();
                }

                return user;
            }

            user = new AppUser
            {
                Id = Guid.NewGuid(),
                Email = email,
                FullName = fullName,
                PasswordHash = SharedPasswordHash,
                Role = role,
                ProgramEntityId = programId,
                StudentNumber = studentNumber,
                Title = title,
                PhoneNumber = phone,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }

        private static async Task<Exam> EnsureExamAsync(ProjectDbContext context, Guid evaluationId, string examType, double weight, int order)
        {
            var exam = await context.Exams.FirstOrDefaultAsync(e => e.CourseEvaluationId == evaluationId && e.ExamType == examType);
            if (exam != null) return exam;

            exam = new Exam
            {
                Id = Guid.NewGuid(),
                CourseEvaluationId = evaluationId,
                ExamType = examType,
                WeightPercentage = weight,
                OrderIndex = order
            };
            context.Exams.Add(exam);
            await context.SaveChangesAsync();
            return exam;
        }

        private static async Task<List<ExamQuestion>> EnsureQuestionsAsync(ProjectDbContext context, Guid examId, int count, string examTitle)
        {
            var result = new List<ExamQuestion>();
            for (var i = 1; i <= count; i++)
            {
                var q = await context.ExamQuestions.FirstOrDefaultAsync(x => x.ExamId == examId && x.QuestionNumber == i);
                if (q == null)
                {
                    q = new ExamQuestion
                    {
                        Id = Guid.NewGuid(),
                        ExamId = examId,
                        QuestionNumber = i,
                        MaxScore = 10,
                        Title = $"{examTitle} Soru {i}",
                        Description = $"Seed soru {i}: yazılım mühendisliği konu başlığı",
                        QuestionType = "WrittenQuestion"
                    };
                    context.ExamQuestions.Add(q);
                    await context.SaveChangesAsync();
                }
                result.Add(q);
            }
            return result.OrderBy(x => x.QuestionNumber).ToList();
        }

        private static async Task EnsureQuestionMappingsAsync(ProjectDbContext context, List<ExamQuestion> questions, List<CourseLearningOutcome> clos)
        {
            foreach (var q in questions)
            {
                var cloA = clos[(q.QuestionNumber - 1) % clos.Count];
                var cloB = clos[q.QuestionNumber % clos.Count];

                if (!await context.ExamQuestionOutcomeMappings.AnyAsync(m => m.ExamQuestionId == q.Id && m.CourseLearningOutcomeId == cloA.Id))
                {
                    context.ExamQuestionOutcomeMappings.Add(new ExamQuestionOutcomeMapping
                    {
                        Id = Guid.NewGuid(),
                        ExamQuestionId = q.Id,
                        CourseLearningOutcomeId = cloA.Id,
                        Weight = 0.60m,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                if (!await context.ExamQuestionOutcomeMappings.AnyAsync(m => m.ExamQuestionId == q.Id && m.CourseLearningOutcomeId == cloB.Id))
                {
                    context.ExamQuestionOutcomeMappings.Add(new ExamQuestionOutcomeMapping
                    {
                        Id = Guid.NewGuid(),
                        ExamQuestionId = q.Id,
                        CourseLearningOutcomeId = cloB.Id,
                        Weight = 0.40m,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
            await context.SaveChangesAsync();
        }

        private static async Task EnsureStudentAnswersAsync(
            ProjectDbContext context,
            List<Enrollment> enrollments,
            List<ExamQuestion> questions,
            int minTotal,
            int maxTotal,
            Random rng,
            bool makeupOnlyForBottomHalf = false)
        {
            var orderedEnrollments = enrollments.OrderBy(e => e.Student!.StudentNumber).ToList();
            var startIndex = makeupOnlyForBottomHalf ? orderedEnrollments.Count / 2 : 0;

            for (var i = startIndex; i < orderedEnrollments.Count; i++)
            {
                var en = orderedEnrollments[i];
                var targetTotal = rng.Next(minTotal, maxTotal + 1);
                var remaining = targetTotal;

                for (var qIndex = 0; qIndex < questions.Count; qIndex++)
                {
                    var q = questions[qIndex];
                    var remainingQuestions = questions.Count - qIndex;
                    var maxThis = Math.Min((int)q.MaxScore, remaining);
                    var minThis = Math.Max(0, remaining - (remainingQuestions - 1) * (int)q.MaxScore);
                    var score = remainingQuestions == 1 ? remaining : rng.Next(minThis, maxThis + 1);
                    remaining -= score;

                    if (!await context.StudentAnswers.AnyAsync(a => a.ExamQuestionId == q.Id && a.EnrollmentId == en.Id))
                    {
                        context.StudentAnswers.Add(new StudentAnswer
                        {
                            Id = Guid.NewGuid(),
                            ExamQuestionId = q.Id,
                            EnrollmentId = en.Id,
                            Score = score
                        });
                    }
                }
            }
            await context.SaveChangesAsync();
        }

        private static async Task EnsureQuizScoresAsync(ProjectDbContext context, List<Enrollment> enrollments, AssessmentComponent quiz, Random rng)
        {
            foreach (var en in enrollments)
            {
                if (!await context.StudentAssessmentComponentScores.AnyAsync(s =>
                        s.AssessmentComponentId == quiz.Id && s.EnrollmentId == en.Id))
                {
                    context.StudentAssessmentComponentScores.Add(new StudentAssessmentComponentScore
                    {
                        Id = Guid.NewGuid(),
                        AssessmentComponentId = quiz.Id,
                        EnrollmentId = en.Id,
                        Score = rng.Next(35, 101),
                        Notes = "Seed quiz puanı (100 üzerinden)",
                        EvaluatedBy = "Selim Karahan",
                        EvaluatedAt = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
            await context.SaveChangesAsync();
        }

        private static CourseEvaluationLetterGradeRule GradeRule(
            Guid evaluationId,
            string letter,
            decimal min,
            decimal max,
            bool isPassing,
            decimal? minFinal) =>
            new()
            {
                Id = Guid.NewGuid(),
                CourseEvaluationId = evaluationId,
                LetterGrade = letter,
                MinScore = min,
                MaxScore = max,
                IsPassing = isPassing,
                MinimumFinalScore = minFinal,
                Description = $"{letter} seviye aralığı",
                CreatedAt = DateTime.UtcNow
            };
    }
}
