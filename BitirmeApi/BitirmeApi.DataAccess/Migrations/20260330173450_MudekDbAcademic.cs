using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BitirmeApi.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MudekDbAcademic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AcademicTerms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartYear = table.Column<int>(type: "int", nullable: false),
                    EndYear = table.Column<int>(type: "int", nullable: false),
                    TermType = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicTerms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Programs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    AccreditationCycleYears = table.Column<int>(type: "int", nullable: false, defaultValue: 5)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Programs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProgramEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Credits = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_Programs_ProgramEntityId",
                        column: x => x.ProgramEntityId,
                        principalTable: "Programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgramOutcomes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProgramEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramOutcomes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgramOutcomes_Programs_ProgramEntityId",
                        column: x => x.ProgramEntityId,
                        principalTable: "Programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StudentNumber = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ProgramEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Programs_ProgramEntityId",
                        column: x => x.ProgramEntityId,
                        principalTable: "Programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Clos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clos_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseOfferings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcademicTermId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Section = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false, defaultValue: "A"),
                    PassingGrade = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    Quota = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseOfferings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseOfferings_AcademicTerms_AcademicTermId",
                        column: x => x.AcademicTermId,
                        principalTable: "AcademicTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseOfferings_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseOfferings_Users_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CloPoMaps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseLearningOutcomeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProgramOutcomeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CloPoMaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CloPoMaps_Clos_CourseLearningOutcomeId",
                        column: x => x.CourseLearningOutcomeId,
                        principalTable: "Clos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CloPoMaps_ProgramOutcomes_ProgramOutcomeId",
                        column: x => x.ProgramOutcomeId,
                        principalTable: "ProgramOutcomes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CourseEvaluations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseOfferingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastCalculatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCalculationDirty = table.Column<bool>(type: "bit", nullable: false),
                    StudentFeedbackEvaluation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProgramOutcomeEvaluation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GeneralEvaluation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImprovementSuggestions = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseEvaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseEvaluations_CourseOfferings_CourseOfferingId",
                        column: x => x.CourseOfferingId,
                        principalTable: "CourseOfferings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Enrollments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseOfferingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false, defaultValue: "Enrolled"),
                    EnrolledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Enrollments_CourseOfferings_CourseOfferingId",
                        column: x => x.CourseOfferingId,
                        principalTable: "CourseOfferings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Enrollments_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProgramOutcomeEvaluationResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseOfferingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProgramOutcomeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AchievementScore = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramOutcomeEvaluationResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgramOutcomeEvaluationResults_CourseOfferings_CourseOfferingId",
                        column: x => x.CourseOfferingId,
                        principalTable: "CourseOfferings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProgramOutcomeEvaluationResults_ProgramOutcomes_ProgramOutcomeId",
                        column: x => x.ProgramOutcomeId,
                        principalTable: "ProgramOutcomes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Surveys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseOfferingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Surveys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Surveys_CourseOfferings_CourseOfferingId",
                        column: x => x.CourseOfferingId,
                        principalTable: "CourseOfferings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseEvaluationLetterGradeRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseEvaluationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LetterGrade = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    MinScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    MaxScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    IsPassing = table.Column<bool>(type: "bit", nullable: false),
                    MinimumFinalScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseEvaluationLetterGradeRules", x => x.Id);
                    table.CheckConstraint("CK_LetterGradeRule_ScoreBounds", "[MinScore] >= 0 AND [MaxScore] <= 100");
                    table.CheckConstraint("CK_LetterGradeRule_ScoreRange", "[MaxScore] >= [MinScore]");
                    table.ForeignKey(
                        name: "FK_CourseEvaluationLetterGradeRules_CourseEvaluations_CourseEvaluationId",
                        column: x => x.CourseEvaluationId,
                        principalTable: "CourseEvaluations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Exams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseEvaluationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    WeightPercentage = table.Column<double>(type: "float", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exams_CourseEvaluations_CourseEvaluationId",
                        column: x => x.CourseEvaluationId,
                        principalTable: "CourseEvaluations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgramOutcomeContributions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseEvaluationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseLearningOutcomeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProgramOutcomeCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ContributionLevel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramOutcomeContributions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgramOutcomeContributions_Clos_CourseLearningOutcomeId",
                        column: x => x.CourseLearningOutcomeId,
                        principalTable: "Clos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProgramOutcomeContributions_CourseEvaluations_CourseEvaluationId",
                        column: x => x.CourseEvaluationId,
                        principalTable: "CourseEvaluations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentEvaluationResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseOfferingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnrollmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MidtermScore = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: true),
                    FinalScore = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: true),
                    MakeupScore = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: true),
                    UsedExamType = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    SuccessGrade = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: true),
                    LetterGrade = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    IsPassed = table.Column<bool>(type: "bit", nullable: false),
                    IncludedInStatistics = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentEvaluationResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentEvaluationResults_CourseOfferings_CourseOfferingId",
                        column: x => x.CourseOfferingId,
                        principalTable: "CourseOfferings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentEvaluationResults_Enrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "Enrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SurveyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    ScaleMin = table.Column<int>(type: "int", nullable: false),
                    ScaleMax = table.Column<int>(type: "int", nullable: false),
                    Options = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    McqOptionsCsv = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Surveys_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "Surveys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Submissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SurveyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IncludeInStatistics = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Submissions_Surveys_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "Surveys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Submissions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentComponents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ComponentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaxScore = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: false),
                    WeightPercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentComponents", x => x.Id);
                    table.CheckConstraint("CK_AssessmentComponent_WeightPercentage", "[WeightPercentage] IS NULL OR ([WeightPercentage] >= 0 AND [WeightPercentage] <= 100)");
                    table.ForeignKey(
                        name: "FK_AssessmentComponents_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CloEvaluationResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseOfferingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseLearningOutcomeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResultType = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AchievementScore = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    CombinedAchievementScore = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    SurveyScore = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    SurveyDifference = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CloEvaluationResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CloEvaluationResults_Clos_CourseLearningOutcomeId",
                        column: x => x.CourseLearningOutcomeId,
                        principalTable: "Clos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CloEvaluationResults_CourseOfferings_CourseOfferingId",
                        column: x => x.CourseOfferingId,
                        principalTable: "CourseOfferings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CloEvaluationResults_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamEvaluationResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseOfferingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParticipantCount = table.Column<int>(type: "int", nullable: false),
                    IncludedStudentCount = table.Column<int>(type: "int", nullable: false),
                    PerfectScoreCount = table.Column<int>(type: "int", nullable: false),
                    MaxTotalScore = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    MinTotalScore = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    AverageTotalScore = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamEvaluationResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamEvaluationResults_CourseOfferings_CourseOfferingId",
                        column: x => x.CourseOfferingId,
                        principalTable: "CourseOfferings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamEvaluationResults_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionNumber = table.Column<int>(type: "int", nullable: false),
                    MaxScore = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuestionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamQuestions_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ValueNumeric = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    ValueText = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Answers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Answers_Submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submissions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AssessmentComponentOutcomeMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssessmentComponentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseLearningOutcomeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentComponentOutcomeMappings", x => x.Id);
                    table.CheckConstraint("CK_AssessmentComponentOutcomeMapping_Weight", "[Weight] >= 0 AND [Weight] <= 1");
                    table.ForeignKey(
                        name: "FK_AssessmentComponentOutcomeMappings_AssessmentComponents_AssessmentComponentId",
                        column: x => x.AssessmentComponentId,
                        principalTable: "AssessmentComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssessmentComponentOutcomeMappings_Clos_CourseLearningOutcomeId",
                        column: x => x.CourseLearningOutcomeId,
                        principalTable: "Clos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentAssessmentComponentScores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssessmentComponentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnrollmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Score = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EvaluatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EvaluatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAssessmentComponentScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentAssessmentComponentScores_AssessmentComponents_AssessmentComponentId",
                        column: x => x.AssessmentComponentId,
                        principalTable: "AssessmentComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentAssessmentComponentScores_Enrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "Enrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamQuestionEvaluationResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseOfferingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamQuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AssessmentComponentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    QuestionNumber = table.Column<int>(type: "int", nullable: false),
                    MaxScore = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: false),
                    AverageScore = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: true),
                    AchievementRate = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    IncludedStudentCount = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamQuestionEvaluationResults", x => x.Id);
                    table.CheckConstraint("CK_ExamQuestionEvalResult_SingleTarget", "([ExamQuestionId] IS NOT NULL AND [AssessmentComponentId] IS NULL) OR ([ExamQuestionId] IS NULL AND [AssessmentComponentId] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_ExamQuestionEvaluationResults_AssessmentComponents_AssessmentComponentId",
                        column: x => x.AssessmentComponentId,
                        principalTable: "AssessmentComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamQuestionEvaluationResults_CourseOfferings_CourseOfferingId",
                        column: x => x.CourseOfferingId,
                        principalTable: "CourseOfferings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamQuestionEvaluationResults_ExamQuestions_ExamQuestionId",
                        column: x => x.ExamQuestionId,
                        principalTable: "ExamQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamQuestionEvaluationResults_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamQuestionOutcomeMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamQuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseLearningOutcomeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamQuestionOutcomeMappings", x => x.Id);
                    table.CheckConstraint("CK_ExamQuestionOutcomeMapping_Weight", "[Weight] >= 0 AND [Weight] <= 1");
                    table.ForeignKey(
                        name: "FK_ExamQuestionOutcomeMappings_Clos_CourseLearningOutcomeId",
                        column: x => x.CourseLearningOutcomeId,
                        principalTable: "Clos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamQuestionOutcomeMappings_ExamQuestions_ExamQuestionId",
                        column: x => x.ExamQuestionId,
                        principalTable: "ExamQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentAnswers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamQuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnrollmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Score = table.Column<decimal>(type: "decimal(7,2)", precision: 7, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentAnswers_Enrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "Enrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentAnswers_ExamQuestions_ExamQuestionId",
                        column: x => x.ExamQuestionId,
                        principalTable: "ExamQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicTerms_Name",
                table: "AcademicTerms",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AcademicTerms_StartYear_EndYear_TermType",
                table: "AcademicTerms",
                columns: new[] { "StartYear", "EndYear", "TermType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Answers_QuestionId_SubmissionId",
                table: "Answers",
                columns: new[] { "QuestionId", "SubmissionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Answers_SubmissionId",
                table: "Answers",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentComponentOutcomeMappings_AssessmentComponentId_CourseLearningOutcomeId",
                table: "AssessmentComponentOutcomeMappings",
                columns: new[] { "AssessmentComponentId", "CourseLearningOutcomeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentComponentOutcomeMappings_CourseLearningOutcomeId",
                table: "AssessmentComponentOutcomeMappings",
                column: "CourseLearningOutcomeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentComponents_ExamId_OrderIndex",
                table: "AssessmentComponents",
                columns: new[] { "ExamId", "OrderIndex" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CloEvaluationResults_CourseLearningOutcomeId",
                table: "CloEvaluationResults",
                column: "CourseLearningOutcomeId");

            migrationBuilder.CreateIndex(
                name: "IX_CloEvaluationResults_CourseOfferingId_CourseLearningOutcomeId_ResultType",
                table: "CloEvaluationResults",
                columns: new[] { "CourseOfferingId", "CourseLearningOutcomeId", "ResultType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CloEvaluationResults_ExamId",
                table: "CloEvaluationResults",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_CloPoMaps_CourseLearningOutcomeId_ProgramOutcomeId",
                table: "CloPoMaps",
                columns: new[] { "CourseLearningOutcomeId", "ProgramOutcomeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CloPoMaps_ProgramOutcomeId",
                table: "CloPoMaps",
                column: "ProgramOutcomeId");

            migrationBuilder.CreateIndex(
                name: "IX_Clos_CourseId_Code",
                table: "Clos",
                columns: new[] { "CourseId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseEvaluationLetterGradeRules_CourseEvaluationId_LetterGrade",
                table: "CourseEvaluationLetterGradeRules",
                columns: new[] { "CourseEvaluationId", "LetterGrade" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseEvaluations_CourseOfferingId",
                table: "CourseEvaluations",
                column: "CourseOfferingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseOfferings_AcademicTermId",
                table: "CourseOfferings",
                column: "AcademicTermId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseOfferings_CourseId_AcademicTermId_Section",
                table: "CourseOfferings",
                columns: new[] { "CourseId", "AcademicTermId", "Section" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseOfferings_TeacherId",
                table: "CourseOfferings",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_ProgramEntityId_Code",
                table: "Courses",
                columns: new[] { "ProgramEntityId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_CourseOfferingId_StudentId",
                table: "Enrollments",
                columns: new[] { "CourseOfferingId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentId",
                table: "Enrollments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamEvaluationResults_CourseOfferingId_ExamId",
                table: "ExamEvaluationResults",
                columns: new[] { "CourseOfferingId", "ExamId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamEvaluationResults_ExamId",
                table: "ExamEvaluationResults",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamQuestionEvaluationResults_AssessmentComponentId",
                table: "ExamQuestionEvaluationResults",
                column: "AssessmentComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamQuestionEvaluationResults_CourseOfferingId_AssessmentComponentId",
                table: "ExamQuestionEvaluationResults",
                columns: new[] { "CourseOfferingId", "AssessmentComponentId" },
                unique: true,
                filter: "[AssessmentComponentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ExamQuestionEvaluationResults_CourseOfferingId_ExamQuestionId",
                table: "ExamQuestionEvaluationResults",
                columns: new[] { "CourseOfferingId", "ExamQuestionId" },
                unique: true,
                filter: "[ExamQuestionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ExamQuestionEvaluationResults_ExamId",
                table: "ExamQuestionEvaluationResults",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamQuestionEvaluationResults_ExamQuestionId",
                table: "ExamQuestionEvaluationResults",
                column: "ExamQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamQuestionOutcomeMappings_CourseLearningOutcomeId",
                table: "ExamQuestionOutcomeMappings",
                column: "CourseLearningOutcomeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamQuestionOutcomeMappings_ExamQuestionId_CourseLearningOutcomeId",
                table: "ExamQuestionOutcomeMappings",
                columns: new[] { "ExamQuestionId", "CourseLearningOutcomeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamQuestions_ExamId_QuestionNumber",
                table: "ExamQuestions",
                columns: new[] { "ExamId", "QuestionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Exams_CourseEvaluationId",
                table: "Exams",
                column: "CourseEvaluationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramOutcomeContributions_CourseEvaluationId_CourseLearningOutcomeId_ProgramOutcomeCode",
                table: "ProgramOutcomeContributions",
                columns: new[] { "CourseEvaluationId", "CourseLearningOutcomeId", "ProgramOutcomeCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProgramOutcomeContributions_CourseLearningOutcomeId",
                table: "ProgramOutcomeContributions",
                column: "CourseLearningOutcomeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramOutcomeEvaluationResults_CourseOfferingId_ProgramOutcomeId",
                table: "ProgramOutcomeEvaluationResults",
                columns: new[] { "CourseOfferingId", "ProgramOutcomeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProgramOutcomeEvaluationResults_ProgramOutcomeId",
                table: "ProgramOutcomeEvaluationResults",
                column: "ProgramOutcomeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramOutcomes_ProgramEntityId_Code",
                table: "ProgramOutcomes",
                columns: new[] { "ProgramEntityId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_SurveyId",
                table: "Questions",
                column: "SurveyId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_EnrollmentId",
                table: "StudentAnswers",
                column: "EnrollmentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_ExamQuestionId_EnrollmentId",
                table: "StudentAnswers",
                columns: new[] { "ExamQuestionId", "EnrollmentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentAssessmentComponentScores_AssessmentComponentId_EnrollmentId",
                table: "StudentAssessmentComponentScores",
                columns: new[] { "AssessmentComponentId", "EnrollmentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentAssessmentComponentScores_EnrollmentId",
                table: "StudentAssessmentComponentScores",
                column: "EnrollmentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentEvaluationResults_CourseOfferingId_EnrollmentId",
                table: "StudentEvaluationResults",
                columns: new[] { "CourseOfferingId", "EnrollmentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentEvaluationResults_EnrollmentId",
                table: "StudentEvaluationResults",
                column: "EnrollmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_SurveyId_UserId",
                table: "Submissions",
                columns: new[] { "SurveyId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_UserId",
                table: "Submissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Surveys_CourseOfferingId",
                table: "Surveys",
                column: "CourseOfferingId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ProgramEntityId",
                table: "Users",
                column: "ProgramEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "AssessmentComponentOutcomeMappings");

            migrationBuilder.DropTable(
                name: "CloEvaluationResults");

            migrationBuilder.DropTable(
                name: "CloPoMaps");

            migrationBuilder.DropTable(
                name: "CourseEvaluationLetterGradeRules");

            migrationBuilder.DropTable(
                name: "ExamEvaluationResults");

            migrationBuilder.DropTable(
                name: "ExamQuestionEvaluationResults");

            migrationBuilder.DropTable(
                name: "ExamQuestionOutcomeMappings");

            migrationBuilder.DropTable(
                name: "ProgramOutcomeContributions");

            migrationBuilder.DropTable(
                name: "ProgramOutcomeEvaluationResults");

            migrationBuilder.DropTable(
                name: "StudentAnswers");

            migrationBuilder.DropTable(
                name: "StudentAssessmentComponentScores");

            migrationBuilder.DropTable(
                name: "StudentEvaluationResults");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Submissions");

            migrationBuilder.DropTable(
                name: "Clos");

            migrationBuilder.DropTable(
                name: "ProgramOutcomes");

            migrationBuilder.DropTable(
                name: "ExamQuestions");

            migrationBuilder.DropTable(
                name: "AssessmentComponents");

            migrationBuilder.DropTable(
                name: "Enrollments");

            migrationBuilder.DropTable(
                name: "Surveys");

            migrationBuilder.DropTable(
                name: "Exams");

            migrationBuilder.DropTable(
                name: "CourseEvaluations");

            migrationBuilder.DropTable(
                name: "CourseOfferings");

            migrationBuilder.DropTable(
                name: "AcademicTerms");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Programs");
        }
    }
}
