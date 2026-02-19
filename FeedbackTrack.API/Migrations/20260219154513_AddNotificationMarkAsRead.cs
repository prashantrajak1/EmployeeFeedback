using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeedbackTrack.API.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationMarkAsRead : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE PROCEDURE usp_Notification_MarkAsRead
                    @NotificationId INT
                AS
                BEGIN
                    UPDATE TNotifications SET IsRead = 1 WHERE Id = @NotificationId;
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS usp_Notification_MarkAsRead;");
        }
    }
}
