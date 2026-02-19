using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeedbackTrack.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserProfileView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_UserProfileView')
                    DROP VIEW vw_UserProfileView;
            ");

            migrationBuilder.Sql(@"
                CREATE VIEW vw_UserProfileView AS
                SELECT u.Id AS UserId, u.Name AS FullName, u.Email, r.RoleName, d.DepartmentName, u.CreatedDate, u.IsActive
                FROM TUsers u
                LEFT JOIN TRoles r ON u.RoleId = r.Id
                LEFT JOIN TDepartments d ON u.DepartmentId = d.Id;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_UserProfileView;");
        }
    }
}
