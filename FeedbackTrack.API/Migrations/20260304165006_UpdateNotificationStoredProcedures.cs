using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeedbackTrack.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNotificationStoredProcedures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE usp_Notification_Delete
                    @Id INT
                AS
                BEGIN
                    DELETE FROM TNotifications WHERE Id = @Id;
                END;
            ");

            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE usp_Notification_MarkAllRead
                    @UserId INT
                AS
                BEGIN
                    UPDATE TNotifications SET IsRead = 1 WHERE UserId = @UserId;
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS usp_Notification_Delete;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS usp_Notification_MarkAllRead;");
        }
    }
}
