using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolHubAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedSubmissionsTb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Submissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmittedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Grade = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssignmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GradedByTeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Submissions_Assignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Assignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Submissions_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Submissions_Teachers_GradedByTeacherId",
                        column: x => x.GradedByTeacherId,
                        principalTable: "Teachers",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_AssignmentId",
                table: "Submissions",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_GradedByTeacherId",
                table: "Submissions",
                column: "GradedByTeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_StudentId",
                table: "Submissions",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Submissions");
        }
    }
}
