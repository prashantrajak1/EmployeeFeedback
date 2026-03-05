using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeedbackTrack.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedFinanceAndOperations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "IF NOT EXISTS (SELECT 1 FROM TDepartments WHERE DepartmentName = 'Finance') " +
                "BEGIN INSERT INTO TDepartments (DepartmentName) VALUES ('Finance') END"
            );

            migrationBuilder.Sql(
                "IF NOT EXISTS (SELECT 1 FROM TDepartments WHERE DepartmentName = 'Operations') " +
                "BEGIN INSERT INTO TDepartments (DepartmentName) VALUES ('Operations') END"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM TDepartments WHERE DepartmentName IN ('Finance', 'Operations')");
        }
    }
}
