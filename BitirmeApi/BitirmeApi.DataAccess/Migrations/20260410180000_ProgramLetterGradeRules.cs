using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BitirmeApi.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ProgramLetterGradeRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProgramLetterGradeRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProgramEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LetterGrade = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    MinScore = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    MaxScore = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    IsPassing = table.Column<bool>(type: "bit", nullable: false),
                    MinimumFinalScore = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramLetterGradeRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgramLetterGradeRules_Programs_ProgramEntityId",
                        column: x => x.ProgramEntityId,
                        principalTable: "Programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.CheckConstraint("CK_ProgramLetterGradeRule_ScoreRange", "[MaxScore] >= [MinScore]");
                    table.CheckConstraint("CK_ProgramLetterGradeRule_ScoreBounds", "[MinScore] >= 0 AND [MaxScore] <= 100");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProgramLetterGradeRules_ProgramEntityId_LetterGrade",
                table: "ProgramLetterGradeRules",
                columns: new[] { "ProgramEntityId", "LetterGrade" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ProgramLetterGradeRules");
        }
    }
}
