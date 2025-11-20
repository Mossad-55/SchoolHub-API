using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolHubAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedIndexInsideCoursesTb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Courses_DepartmentId",
                table: "Courses");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Courses",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_DepartmentId_Code",
                table: "Courses",
                columns: new[] { "DepartmentId", "Code" },
                unique: true,
                filter: "[Code] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Courses_DepartmentId_Code",
                table: "Courses");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_DepartmentId",
                table: "Courses",
                column: "DepartmentId");
        }
    }
}
