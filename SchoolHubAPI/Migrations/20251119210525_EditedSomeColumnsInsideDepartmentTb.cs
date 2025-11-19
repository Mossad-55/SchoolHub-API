using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolHubAPI.Migrations
{
    /// <inheritdoc />
    public partial class EditedSomeColumnsInsideDepartmentTb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdateDate",
                table: "Departments",
                newName: "UpdatedDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "Departments",
                newName: "UpdateDate");
        }
    }
}
